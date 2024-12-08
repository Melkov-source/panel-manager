namespace Itibsoft.PanelManager
{
    public abstract class PanelControllerBase<TPanel> : IPanelController<TPanel>, IPanelControllerProcessor where TPanel : IPanel
    {
        ushort IPanelControllerProcessor.Hash => _hash;
        public TPanel Panel { get; private set; }
        protected IPanelManager Manager { get; private set; }

        private IPanelManagerProcessor _managerProcessor;
        private IPanelFactory _panelFactory;
        private readonly ushort _hash;

        protected PanelControllerBase()
        {
            _hash = GetType().GetStableHash();
        }

        public void Open() => _managerProcessor.Open(this, Panel);
        public void Close() => _managerProcessor.Close(this, Panel);
        public void Release() => _managerProcessor.Release(this, Panel);
        public IPanel GetPanel() => Panel;

        protected virtual void OnLoad() { }
        protected virtual void Initialize() { }
        protected virtual void OnOpenBefore() { }
        protected virtual void OnOpenAfter() { }
        protected virtual void OnCloseBefore() { }
        protected virtual void OnCloseAfter() { }
        protected virtual void OnUnload() { }

        void IPanelControllerProcessor.Setup(IPanelManager manager, IPanelFactory factory)
        {
            Manager = manager;
            _managerProcessor = (IPanelManagerProcessor)manager;
            _panelFactory = factory;
        }
        
        void IPanelControllerProcessor.Load()
        {
            var type = GetType();
            var meta = PanelReflector.GetMeta(type);

            var info = new PanelInfo
            {
                PanelType = meta.PanelType,
                Order = meta.Order,
                AssetId = meta.AssetId
            };

            Panel = (TPanel)_panelFactory.Create(meta);

            var processor = (IPanelProcessor)Panel;
            
            processor.Setup(info);
            
            OnLoad();
        }
        
        void IPanelControllerProcessor.Initialize() => Initialize();
        void IPanelControllerProcessor.OpenBefore() => OnOpenBefore();
        void IPanelControllerProcessor.OpenAfter() => OnOpenAfter();
        void IPanelControllerProcessor.CloseBefore() => OnCloseBefore();
        void IPanelControllerProcessor.CloseAfter() => OnCloseAfter();
        
        void IPanelControllerProcessor.Unload()
        {
            Panel.SetActive(false);

            var panelGameObject = Panel.GetGameObject();
            
#if ADDRESSABLES
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(panelGameObject);
#else
            UnityEngine.Object.DestroyImmediate(panelGameObject);
            UnityEngine.Resources.UnloadUnusedAssets();
#endif
            
            OnUnload();
        }
    }
}