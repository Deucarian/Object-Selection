namespace Deucarian.ObjectSelection
{
    public interface IObjectSelectionCommands<TKey>
    {
        void Select(
            TKey key,
            SelectionChangeReason reason = SelectionChangeReason.Programmatic,
            bool forceEvent = false);

        bool TrySelect(
            TKey key,
            SelectionChangeReason reason = SelectionChangeReason.Programmatic,
            bool forceEvent = false);

        void ClearSelection(
            SelectionChangeReason reason = SelectionChangeReason.Cleared,
            bool forceEvent = false);
    }
}
