using System.Collections.Generic;

namespace Itibsoft.PanelManager
{
    public static class CollectionExtensions
    {
        public static void AddOrCreateNew<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            dictionary.AddOrCreateNew(key, value, out _);
        }
        
        public static void AddOrCreateNew<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value, out List<TValue> values)
        {
            if(dictionary.TryGetValue(key, out var bots))
            {
                bots.Add(value);
            }
            else
            {
                bots = new List<TValue> { value };
                dictionary.Add(key, bots);
            }

            values = bots;
        }
        
        public static List<TValue> GetOrCreateNew<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            if(dictionary.TryGetValue(key, out var bots))
            {
                return bots;
            }

            bots = new List<TValue>();
            dictionary.Add(key, bots);

            return bots;
        }
    }
}