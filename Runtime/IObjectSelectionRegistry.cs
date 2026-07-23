namespace Deucarian.ObjectSelection
{
    public interface IObjectSelectionRegistry<TKey> : IReadOnlyObjectSelectionRegistry<TKey>
    {
        void Register(ISelectableObject<TKey> selectable);
        bool Unregister(TKey key);
        void Clear();
        int RemoveDestroyedEntries();
    }
}
