using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Provides previous and current hover data for hover events.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class HoverChangedEventArgs<TKey> : EventArgs
    {
        /// <summary>
        /// Creates hover changed event data.
        /// </summary>
        public HoverChangedEventArgs(
            bool hadPreviousHover,
            TKey previousKey,
            Object previousObject,
            bool hasHover,
            TKey currentKey,
            Object currentObject,
            SelectionChangeReason reason)
        {
            HadPreviousHover = hadPreviousHover;
            PreviousKey = previousKey;
            PreviousObject = UnityObjectUtility.IsDestroyed(previousObject) ? null : previousObject;
            HasHover = hasHover;
            CurrentKey = currentKey;
            CurrentObject = UnityObjectUtility.IsDestroyed(currentObject) ? null : currentObject;
            Reason = reason;
        }

        /// <summary>
        /// Gets whether the event had a previous hovered key.
        /// </summary>
        public bool HadPreviousHover { get; }

        /// <summary>
        /// Gets the previous hovered key.
        /// </summary>
        public TKey PreviousKey { get; }

        /// <summary>
        /// Gets the previous hovered Unity object.
        /// </summary>
        public Object PreviousObject { get; }

        /// <summary>
        /// Gets whether the event has a current hovered key.
        /// </summary>
        public bool HasHover { get; }

        /// <summary>
        /// Gets the current hovered key.
        /// </summary>
        public TKey CurrentKey { get; }

        /// <summary>
        /// Gets the current hovered Unity object.
        /// </summary>
        public Object CurrentObject { get; }

        /// <summary>
        /// Gets the reason supplied by the caller that changed hover state.
        /// </summary>
        public SelectionChangeReason Reason { get; }
    }
}
