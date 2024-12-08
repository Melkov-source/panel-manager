#if ADDRESSABLES

using UnityEngine.AddressableAssets;

namespace Itibsoft.PanelManager.External
{
    public class AddressablesPanelFactory : IPanelFactory
    {
        public IPanel Create(PanelAttribute meta)
        {
            var panel = Addressables
                .InstantiateAsync(meta.AssetId)
                .WaitForCompletion()
                .GetComponent<PanelBase>();

            return panel;
        }
    }
}

#endif