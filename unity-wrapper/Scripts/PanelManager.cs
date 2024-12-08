using System;
using System.Collections.Generic;

namespace Itibsoft.PanelManager
{
#if EXTENJECT
    [JetBrains.Annotations.UsedImplicitly]
#endif
    public class PanelManager : IPanelManager, IPanelManagerProcessor
    {
        #region Fields

        #region Public Fields

        public PanelDispatcher PanelDispatcher { get; }

        #endregion

        #region Private Fields

        private readonly IPanelControllerFactory _panelControllerFactory;
        private readonly IPanelFactory _panelFactory;

        private readonly Dictionary<ushort, IPanelController> _panelsCashed = new();

        #endregion

        #endregion;

        #region Initialize

        public PanelManager(IPanelControllerFactory panelControllerFactory, PanelDispatcher panelDispatcher)
        {
            _panelControllerFactory = panelControllerFactory;
            
#if ADDRESSABLES
            _panelFactory = new External.AddressablesPanelFactory();
#else
            _panelFactory = new ResourcesPanelFactory();
#endif

            if (panelDispatcher == default)
            {
                panelDispatcher = PanelDispatcherBuilder.Create().Build();
            }

            PanelDispatcher = panelDispatcher;
        }

        #endregion

        #region Public API (Methods)

        public TPanelController LoadPanel<TPanelController>() where TPanelController : IPanelController
        {
            var type = typeof(TPanelController);

            var controller = LoadController(type);

            return (TPanelController)controller;
        }

        public void ClosePanels(params Type[] typeControllers)
        {
            for (int index = 0, count = typeControllers.Length; index < count; index++)
            {
                var typeController = typeControllers[index];

                var controller = LoadController(typeController);
                controller.Close();
            }
        }

        public void OpenPanels(params Type[] typeControllers)
        {
            for (int index = 0, count = typeControllers.Length; index < count; index++)
            {
                var typeController = typeControllers[index];

                var controller = LoadController(typeController);
                controller.Open();
            }
        }

        private IPanelController LoadController(Type typePanelController)
        {
            var meta = PanelReflector.GetMeta(typePanelController);
            var hash = typePanelController.GetStableHash();

            if (_panelsCashed.TryGetValue(hash, out var controller))
            {
                return controller;
            }

            controller = _panelControllerFactory.Create(typePanelController, meta);

            var processor = (IPanelControllerProcessor)controller;

            processor.Setup(this, _panelFactory);
            processor.Load();
            processor.Initialize();

            var panel = controller.GetPanel();

            PanelDispatcher.Cache(panel);

            _panelsCashed.Add(hash, controller);

            return controller;
        }

        #endregion

        void IPanelManagerProcessor.Open(IPanelControllerProcessor processor, IPanel panel)
        {
            processor.OpenBefore();
            PanelDispatcher.Activate(panel);
            processor.OpenAfter();
        }

        void IPanelManagerProcessor.Close(IPanelControllerProcessor processor, IPanel panel)
        {
            processor.CloseBefore();
            PanelDispatcher.Cache(panel);
            processor.CloseAfter();
        }

        void IPanelManagerProcessor.Release(IPanelControllerProcessor processor, IPanel panel)
        {
            PanelDispatcher.Remove(panel);

            processor.Unload();

            _panelsCashed.Remove(processor.Hash);
        }

#if EXTENJECT
        [JetBrains.Annotations.UsedImplicitly]
        public class Factory : Zenject.PlaceholderFactory<PanelManager>
        {
        }
#endif
    }
}