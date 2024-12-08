#if EXTENJECT
using UnityEngine;
using UnityEngine.EventSystems;

namespace Itibsoft.PanelManager.External
{
    [JetBrains.Annotations.UsedImplicitly]
    public class PanelManagerInstaller : Zenject.Installer<PanelDispatcher, Transform, PanelManagerInstaller>
    {
        private readonly PanelDispatcher _panelDispatcherPrefab;
        private readonly Transform _group;

        public PanelManagerInstaller(Transform group, PanelDispatcher panelDispatcher = default)
        {
            _panelDispatcherPrefab = panelDispatcher;
            _group = group;
        }

        public override void InstallBindings()
        {
            Container
                .Bind<IPanelManager>()
                .FromIFactory(factory => factory
                    .To<PanelManager.Factory>()
                    .FromSubContainerResolve()
                    .ByMethod(InstallPanelManagerFactory)
                    .AsSingle())
                .AsSingle()
                .NonLazy();
        }

        private void InstallPanelManagerFactory(Zenject.DiContainer container)
        {
            if (_panelDispatcherPrefab == default)
            {
                var eventSystem = EventSystem.current;

                var panelDispatcherBuilder = PanelDispatcherBuilder.Create();

                if (eventSystem)
                {
                    panelDispatcherBuilder.SetEventSystem(eventSystem);
                }
                
                var panelDispatcherInstance = panelDispatcherBuilder.Build();
                panelDispatcherInstance.transform.SetParent(_group);
                
                container.Bind<PanelDispatcher>()
                    .FromInstance(panelDispatcherInstance)
                    .AsSingle()
                    .Lazy();
            }
            else
            {
                container.Bind<PanelDispatcher>()
                    .FromComponentInNewPrefab(_panelDispatcherPrefab)
                    .UnderTransform(_group)
                    .AsSingle()
                    .Lazy();
            }

            container
                .Bind<IPanelControllerFactory>()
                .To<ExtenjectPanelControllerFactory>()
                .AsSingle()
                .Lazy();

            container
                .BindFactory<PanelManager, PanelManager.Factory>()
                .AsSingle()
                .Lazy();
        }
    }
}

#endif