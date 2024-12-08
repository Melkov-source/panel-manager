using System;
using System.Linq;

namespace Itibsoft.PanelManager
{
    public static class TypeExtensions
    {
        public static ushort GetStableHash(this Type controller)
        {
            var key = controller.FullName;

            if (key == default)
            {
                return 0;
            }

            unchecked
            {
                return (ushort)key.Aggregate(23, (current, c) => current * 31 + c);
            }
        }
    }
}