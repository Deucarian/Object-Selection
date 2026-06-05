namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Describes the source of a selection or hover state change.
    /// </summary>
    public enum SelectionChangeReason
    {
        /// <summary>
        /// The change came from code or another non-specific source.
        /// </summary>
        Programmatic = 0,

        /// <summary>
        /// The change came from a physics raycast input adapter.
        /// </summary>
        Raycast = 1,

        /// <summary>
        /// The change came from hover tracking.
        /// </summary>
        Hover = 2,

        /// <summary>
        /// The current selection or hover state was cleared.
        /// </summary>
        Cleared = 3
    }
}
