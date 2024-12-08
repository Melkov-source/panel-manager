using System;

namespace Itibsoft.PanelManager
{
    public interface IPanelControllerFactory
    {
        public TPanelController Create<TPanelController>(PanelAttribute meta) where TPanelController : IPanelController;
        public IPanelController Create(Type typePanelController, PanelAttribute meta);
    }
}