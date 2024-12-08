#if REFLEX

using System;
using Reflex.Core;
using Reflex.Injectors;

namespace Itibsoft.PanelManager.External
{
    public class ReflexPanelControllerFactory : PanelControllerFactory
    {
        private Container _container;

        public void SetContainer(Container container)
        {
            _container = container;
        }

        public override object CreateInstance(Type type, params object[] arguments)
        {
            var controller = Activator.CreateInstance(type, arguments);
            AttributeInjector.Inject(controller, _container);
            return controller;
        }
    }
}

#endif