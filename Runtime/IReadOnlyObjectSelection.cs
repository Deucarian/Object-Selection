using System;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    public interface IReadOnlyObjectSelection<TKey>
    {
        event EventHandler<SelectionChangedEventArgs<TKey>> SelectionChanged;

        IReadOnlyObjectSelectionRegistry<TKey> SelectionRegistry { get; }
        bool HasSelection { get; }
        TKey CurrentKey { get; }
        Object CurrentObject { get; }
    }
}
