namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Defines a hook for consumer-owned selection highlighting.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public interface IObjectSelectionHighlighter<TKey>
    {
        /// <summary>
        /// Applies a visual response to a selection change.
        /// </summary>
        /// <param name="args">The selection change data.</param>
        void OnSelectionChanged(SelectionChangedEventArgs<TKey> args);
    }
}
