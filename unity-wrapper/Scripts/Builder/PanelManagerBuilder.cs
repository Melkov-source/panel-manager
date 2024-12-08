namespace Itibsoft.PanelManager
{
    public class PanelManagerBuilder
    {
        private PanelDispatcher _panelDispatcher;
        private IPanelControllerFactory _panelControllerFactory;
        
        public static PanelManagerBuilder Create()
        {
            return new PanelManagerBuilder();
        }

        public PanelManagerBuilder SetPanelDispatcher(PanelDispatcher panelDispatcher)
        {
            _panelDispatcher = panelDispatcher;

            return this;
        }

        public PanelManagerBuilder SetPanelControllerFactory(IPanelControllerFactory panelControllerFactory)
        {
            _panelControllerFactory = panelControllerFactory;

            return this;
        }

        public IPanelManager Build()
        {
            _panelControllerFactory ??= new PanelControllerFactory();
            
            var panelManager = new PanelManager(_panelControllerFactory, _panelDispatcher);
            return panelManager;
        }
    }
}