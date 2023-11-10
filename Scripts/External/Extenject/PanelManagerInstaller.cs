﻿using UnityEngine;

#if EXTENJECT

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
                    .ByMethod(InstallPanelManagerFactory))
                .AsSingle()
                .NonLazy();
        }

        private void InstallPanelManagerFactory(Zenject.DiContainer container)
        {
            if (_panelDispatcherPrefab == default)
            {
                var panelDispatcherInstance = PanelDispatcher.Create();
                panelDispatcherInstance.transform.SetParent(_group);
                
                Container.Bind<PanelDispatcher>()
                    .FromInstance(panelDispatcherInstance)
                    .AsSingle()
                    .Lazy();
            }
            else
            {
                Container.Bind<PanelDispatcher>()
                    .FromComponentInNewPrefab(_panelDispatcherPrefab)
                    .UnderTransform(_group)
                    .AsSingle()
                    .Lazy();
            }
            
            container
                .Bind<IPanelFactory>()
#if ADDRESSABLES
                .To<AddressablesPanelFactory>()
#else
                .To<ResourcesPanelFactory>()
#endif
                .AsSingle()
                .Lazy();

            container
                .Bind<IPanelControllerFactory>()
                .To<External.ExtenjectPanelControllerFactory>()
                .AsSingle()
                .Lazy();
        }
    }
}

#endif