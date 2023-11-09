using System;
using System.Collections.Generic;
using System.Reflection;

namespace Itibsoft.PanelManager
{
#if EXTENJECT
    [JetBrains.Annotations.UsedImplicitly]
#endif
    public class PanelManager : IPanelManager
    {
        private readonly IPanelControllerFactory _panelControllerFactory;
        private readonly PanelDispatcher _panelDispatcher;

        private readonly Dictionary<ushort, IPanelController> _panelsCashed = new();

#if EXTENJECT
        public PanelManager(IPanelControllerFactory panelControllerFactory, PanelDispatcher panelDispatcher)
        {
            _panelControllerFactory = panelControllerFactory;
            _panelDispatcher = panelDispatcher;
        }
#else
        public PanelManager(IPanelControllerFactory panelControllerFactory, PanelDispatcher panelDispatcher)
        {
            _panelControllerFactory = panelControllerFactory;
            _panelDispatcher = panelDispatcher;

            if (_panelControllerFactory == default)
            {
#if ADDRESSABLES
                var panelFactory = new External.AddressablesPanelFactory();
#else
                var panelFactory = new ResourcesPanelFactory();
#endif

                _panelControllerFactory = new PanelControllerFactory(panelFactory);
            }

            _panelDispatcher ??= PanelDispatcher.Create();
        }
#endif

        #region Public API (Methods)

        public TPanelController LoadPanel<TPanelController>() where TPanelController : IPanelController
        {
            var type = typeof(TPanelController);
            var hash = type.GetStableHash();
            var meta = GetMeta<TPanelController>();

            if (_panelsCashed.TryGetValue(hash, out var controller))
            {
                return (TPanelController)controller;
            }

            controller = _panelControllerFactory.Create<TPanelController>(meta);

            var panel = controller.GetPanel();
            
            panel.SetMeta(meta);

            _panelDispatcher.Cache(panel);

            InvokeConstructorMethod(panel);

            controller.RegisterCallback<OpenPanelCallback>(OnHandleOpenPanel);
            controller.RegisterCallback<ClosePanelCallback>(OnHandleClosePanel);
            controller.RegisterCallback<ReleasePanelCallback>(OnReleasePanelHandle);

            _panelsCashed.Add(hash, controller);

            return (TPanelController)controller;
        }

        private void OnReleasePanelHandle(ReleasePanelCallback callback)
        {
            var controller = callback.PanelController;
            var panel = controller.GetPanel();

            var type = controller.GetType();
            var hash = type.GetStableHash();

            panel.Dispose();
            panel.SetActive(false);

            controller.Dispose();

            var panelGameObject = panel.GetGameObject();

#if ADDRESSABLES
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(panelGameObject);
#else
            UnityEngine.Object.DestroyImmediate(panelGameObject);
            UnityEngine.Resources.UnloadUnusedAssets();
#endif

            _panelsCashed.Remove(hash);
        }

        #endregion

        #region Private API (Methods)

        #region Callback Handlers

        private void OnHandleOpenPanel(OpenPanelCallback callback)
        {
            var panel = callback.Panel;
            
            switch (panel.Meta.PanelType)
            {
                case PanelType.Window:
                    _panelDispatcher.SetWindow(panel);
                    break;
                case PanelType.Overlay:
                    _panelDispatcher.SetOverlay(panel);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            InvokeOnOpenMethod(panel);
        }

        private void OnHandleClosePanel(ClosePanelCallback callback)
        {
            var panel = callback.Panel;

            InvokeOnCloseMethod(panel);

            _panelDispatcher.Cache(panel);
        }

        #endregion

        #region Reflection Utils

        private PanelAttribute GetMeta<TPanelController>() where TPanelController : IPanelController
        {
            var type = typeof(TPanelController);
            var meta = type.GetCustomAttribute<PanelAttribute>();

            if (meta == default)
            {
                throw new Exception($"Not found Attribute.Panel for controller: {type.Name}");
            }

            return meta;
        }

        private void InvokeConstructorMethod(IPanel panel) => InvokeMethodForName(panel, "Constructor");
        private void InvokeOnOpenMethod(IPanel panel) => InvokeMethodForName(panel, "OnOpen");
        private void InvokeOnCloseMethod(IPanel panel) => InvokeMethodForName(panel, "OnClose");

        private void InvokeMethodForName(object instance, string nameMethod)
        {
            const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var method = instance.GetType().GetMethod(nameMethod, BINDING_FLAGS);

            if (method != default)
            {
                method.Invoke(instance, default);
            }
        }

        #endregion

        #endregion
    }
}