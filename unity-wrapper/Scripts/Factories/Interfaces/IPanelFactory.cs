namespace Itibsoft.PanelManager
{
    public interface IPanelFactory
    {
        public IPanel Create(PanelAttribute meta);
    }
}