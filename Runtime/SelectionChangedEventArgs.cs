using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Provides previous and current selection data for selection change events.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class SelectionChangedEventArgs<TKey> : EventArgs
    {
        /// <summary>
        /// Creates selection changed event data.
        /// </summary>
        public SelectionChangedEventArgs(
            bool hadPreviousSelection,
            TKey previousKey,
            Object previousObject,
            bool hasSelection,
            TKey currentKey,
            Object currentObject,
            SelectionChangeReason reason)
        {
            HadPreviousSelection = hadPreviousSelection;
            PreviousKey = previousKey;
            PreviousObject = UnityObjectUtility.IsDestroyed(previousObject) ? null : previousObject;
            HasSelection = hasSelection;
            CurrentKey = currentKey;
            CurrentObject = UnityObjectUtility.IsDestroyed(currentObject) ? null : currentObject;
            Reason = reason;
        }

        /// <summary>
        /// Gets whether the event had a previous selection.
        /// </summary>
        public bool HadPreviousSelection { get; }

        /// <summary>
        /// Gets the previous selected key.
        /// </summary>
        public TKey PreviousKey { get; }

        /// <summary>
        /// Gets the previous selected Unity object.
        /// </summary>
        public Object PreviousObject { get; }

        /// <summary>
        /// Gets whether the event has a current selection.
        /// </summary>
        public bool HasSelection { get; }

        /// <summary>
        /// Gets the current selected key.
        /// </summary>
        public TKey CurrentKey { get; }

        /// <summary>
        /// Gets the current selected Unity object.
        /// </summary>
        public Object CurrentObject { get; }

        /// <summary>
        /// Gets the reason supplied by the caller that changed selection.
        /// </summary>
        public SelectionChangeReason Reason { get; }
    }
}
