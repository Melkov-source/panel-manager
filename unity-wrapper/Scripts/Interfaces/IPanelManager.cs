using System;

namespace Itibsoft.PanelManager
{
	public interface IPanelManager
	{
		public PanelDispatcher PanelDispatcher { get; }
		public TPanelController LoadPanel<TPanelController>() where TPanelController : IPanelController;
		public void OpenPanels(params Type[] typeControllers);
		public void ClosePanels(params Type[] typeControllers);
	}
	
	public interface IPanelManagerProcessor
	{
		public void Open(IPanelControllerProcessor processor, IPanel panel);
		public void Close(IPanelControllerProcessor processor, IPanel panel);
		public void Release(IPanelControllerProcessor processor, IPanel panel);
	}
}
