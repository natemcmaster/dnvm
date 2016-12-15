using System.Collections.Generic;

namespace System.Linq
{
    static class CollectionExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection)
            => collection ?? Enumerable.Empty<T>();
    }
}