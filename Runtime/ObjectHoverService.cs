using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection
{
    /// <summary>
    /// Tracks hover state independently from selection state.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class ObjectHoverService<TKey>
    {
        private readonly ObjectSelectionRegistry<TKey> _registry;
        private bool _hasHover;
        private TKey _currentHoveredKey;

        /// <summary>
        /// Creates a hover service backed by an object selection registry.
        /// </summary>
        /// <param name="registry">The registry used to resolve hovered keys.</param>
        public ObjectHoverService(ObjectSelectionRegistry<TKey> registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            _registry = registry;
        }

        /// <summary>
        /// Raised when hover starts for a key.
        /// </summary>
        public event EventHandler<HoverChangedEventArgs<TKey>> HoverStarted;

        /// <summary>
        /// Raised when hover ends for a key.
        /// </summary>
        public event EventHandler<HoverChangedEventArgs<TKey>> HoverEnded;

        /// <summary>
        /// Raised when hover state changes.
        /// </summary>
        public event EventHandler<HoverChangedEventArgs<TKey>> HoverChanged;

        /// <summary>
        /// Gets whether a current hover exists.
        /// </summary>
        public bool HasHover
        {
            get { return _hasHover; }
        }

        /// <summary>
        /// Gets the current hovered key, or the default key value when no hover exists.
        /// </summary>
        public TKey CurrentHoveredKey
        {
            get { return _hasHover ? _currentHoveredKey : default(TKey); }
        }

        /// <summary>
        /// Gets the current hovered Unity object, or <c>null</c> when no live object can be resolved.
        /// </summary>
        public Object CurrentHoveredObject
        {
            get
            {
                if (!_hasHover)
                {
                    return null;
                }

                Object targetObject;
                return _registry.TryGetObject(_currentHoveredKey, out targetObject) ? targetObject : null;
            }
        }

        /// <summary>
        /// Starts hover for an existing registered key.
        /// </summary>
        /// <param name="key">The key to hover.</param>
        /// <param name="reason">The reason for the hover change.</param>
        public void SetHover(TKey key, SelectionChangeReason reason = SelectionChangeReason.Hover)
        {
            if (!TrySetHover(key, reason))
            {
                throw new KeyNotFoundException("The hovered key does not exist in the object selection registry.");
            }
        }

        /// <summary>
        /// Attempts to start hover for an existing registered key.
        /// </summary>
        /// <param name="key">The key to hover.</param>
        /// <param name="reason">The reason for the hover change.</param>
        /// <returns><c>true</c> when the key exists and was accepted.</returns>
        public bool TrySetHover(TKey key, SelectionChangeReason reason = SelectionChangeReason.Hover)
        {
            if (ReferenceEquals(key, null))
            {
                return false;
            }

            Object currentObject;
            if (!_registry.TryGetObject(key, out currentObject))
            {
                return false;
            }

            if (_hasHover && EqualityComparer<TKey>.Default.Equals(_currentHoveredKey, key))
            {
                return true;
            }

            bool hadPreviousHover = _hasHover;
            TKey previousKey = CurrentHoveredKey;
            Object previousObject = CurrentHoveredObject;

            _currentHoveredKey = key;
            _hasHover = true;

            if (hadPreviousHover)
            {
                HoverEnded?.Invoke(
                    this,
                    new HoverChangedEventArgs<TKey>(
                        true,
                        previousKey,
                        previousObject,
                        false,
                        default(TKey),
                        null,
                        reason));
            }

            var args = new HoverChangedEventArgs<TKey>(
                hadPreviousHover,
                previousKey,
                previousObject,
                true,
                _currentHoveredKey,
                currentObject,
                reason);

            HoverStarted?.Invoke(this, args);
            HoverChanged?.Invoke(this, args);
            return true;
        }

        /// <summary>
        /// Clears the current hover state.
        /// </summary>
        /// <param name="reason">The reason for the hover change.</param>
        public void ClearHover(SelectionChangeReason reason = SelectionChangeReason.Cleared)
        {
            if (!_hasHover)
            {
                return;
            }

            TKey previousKey = _currentHoveredKey;
            Object previousObject = CurrentHoveredObject;

            _currentHoveredKey = default(TKey);
            _hasHover = false;

            var args = new HoverChangedEventArgs<TKey>(
                true,
                previousKey,
                previousObject,
                false,
                default(TKey),
                null,
                reason);

            HoverEnded?.Invoke(this, args);
            HoverChanged?.Invoke(this, args);
        }
    }
}
