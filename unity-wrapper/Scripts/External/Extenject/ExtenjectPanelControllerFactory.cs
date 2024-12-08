#if EXTENJECT 

using System;
using JetBrains.Annotations;
using Zenject;

namespace Itibsoft.PanelManager.External
{
    [UsedImplicitly]
    public class ExtenjectPanelControllerFactory : PanelControllerFactory
    {
        [Inject] private readonly DiContainer _diContainer;

        public override object CreateInstance(Type type, params object[] arguments)
        {
            return _diContainer.Instantiate(type, arguments);
        }
    }
}

#endif