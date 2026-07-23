using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Tracks current and previous selection by stable key.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class ObjectSelectionService<TKey> :
        IReadOnlyObjectSelection<TKey>,
        IObjectSelectionCommands<TKey>
    {
        private readonly IReadOnlyObjectSelectionRegistry<TKey> _selectionRegistry;
        private readonly ObjectSelectionRegistry<TKey> _registry;
        private bool _hasSelection;
        private TKey _currentKey;
        private bool _hasPreviousSelection;
        private TKey _previousKey;
        private Object _previousObject;

        /// <summary>
        /// Creates a selection service backed by an object selection registry.
        /// </summary>
        /// <param name="registry">The registry used to resolve selected keys.</param>
        public ObjectSelectionService(ObjectSelectionRegistry<TKey> registry)
            : this((IReadOnlyObjectSelectionRegistry<TKey>)registry)
        {
            _registry = registry;
        }

        public ObjectSelectionService(IReadOnlyObjectSelectionRegistry<TKey> registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            _selectionRegistry = registry;
            _registry = registry as ObjectSelectionRegistry<TKey>;
        }

        /// <summary>
        /// Raised when selection changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs<TKey>> SelectionChanged;

        /// <summary>
        /// Gets the registry used by the selection service.
        /// </summary>
        public ObjectSelectionRegistry<TKey> Registry
        {
            get { return _registry; }
        }

        public IReadOnlyObjectSelectionRegistry<TKey> SelectionRegistry
        {
            get { return _selectionRegistry; }
        }

        /// <summary>
        /// Gets whether a current selection exists.
        /// </summary>
        public bool HasSelection
        {
            get { return _hasSelection; }
        }

        /// <summary>
        /// Gets whether a previous selection has been recorded.
        /// </summary>
        public bool HasPreviousSelection
        {
            get { return _hasPreviousSelection; }
        }

        /// <summary>
        /// Gets the current selected key, or the default key value when no selection exists.
        /// </summary>
        public TKey CurrentKey
        {
            get { return _hasSelection ? _currentKey : default(TKey); }
        }

        /// <summary>
        /// Gets the previous selected key, or the default key value when no previous selection exists.
        /// </summary>
        public TKey PreviousKey
        {
            get { return _hasPreviousSelection ? _previousKey : default(TKey); }
        }

        /// <summary>
        /// Gets the current selected Unity object, or <c>null</c> when no live object can be resolved.
        /// </summary>
        public Object CurrentObject
        {
            get
            {
                if (!_hasSelection)
                {
                    return null;
                }

                Object targetObject;
                return _selectionRegistry.TryGetObject(_currentKey, out targetObject) ? targetObject : null;
            }
        }

        /// <summary>
        /// Gets the previously selected Unity object, or <c>null</c> when it has been destroyed.
        /// </summary>
        public Object PreviousObject
        {
            get
            {
                return _hasPreviousSelection && !UnityObjectUtility.IsDestroyed(_previousObject)
                    ? _previousObject
                    : null;
            }
        }

        /// <summary>
        /// Selects an existing registered key.
        /// </summary>
        /// <param name="key">The key to select.</param>
        /// <param name="reason">The reason for the selection change.</param>
        /// <param name="forceEvent">When true, raises an event even if the key is already selected.</param>
        public void Select(
            TKey key,
            SelectionChangeReason reason = SelectionChangeReason.Programmatic,
            bool forceEvent = false)
        {
            if (!TrySelect(key, reason, forceEvent))
            {
                throw new KeyNotFoundException("The selected key does not exist in the object selection registry.");
            }
        }

        /// <summary>
        /// Attempts to select an existing registered key.
        /// </summary>
        /// <param name="key">The key to select.</param>
        /// <param name="reason">The reason for the selection change.</param>
        /// <param name="forceEvent">When true, raises an event even if the key is already selected.</param>
        /// <returns><c>true</c> when the key exists and was accepted.</returns>
        public bool TrySelect(
            TKey key,
            SelectionChangeReason reason = SelectionChangeReason.Programmatic,
            bool forceEvent = false)
        {
            if (ReferenceEquals(key, null))
            {
                return false;
            }

            Object currentObject;
            if (!_selectionRegistry.TryGetObject(key, out currentObject))
            {
                return false;
            }

            if (_hasSelection &&
                EqualityComparer<TKey>.Default.Equals(_currentKey, key) &&
                !forceEvent)
            {
                return true;
            }

            bool hadPreviousSelection = _hasSelection;
            TKey previousKey = CurrentKey;
            Object previousObject = CurrentObject;

            _hasPreviousSelection = hadPreviousSelection;
            _previousKey = previousKey;
            _previousObject = previousObject;

            _currentKey = key;
            _hasSelection = true;

            RaiseSelectionChanged(
                hadPreviousSelection,
                previousKey,
                previousObject,
                true,
                _currentKey,
                currentObject,
                reason);

            return true;
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        /// <param name="reason">The reason for the selection change.</param>
        /// <param name="forceEvent">When true, raises an event even if no key is selected.</param>
        public void ClearSelection(
            SelectionChangeReason reason = SelectionChangeReason.Cleared,
            bool forceEvent = false)
        {
            if (!_hasSelection && !forceEvent)
            {
                return;
            }

            bool hadPreviousSelection = _hasSelection;
            TKey previousKey = CurrentKey;
            Object previousObject = CurrentObject;

            _hasPreviousSelection = hadPreviousSelection;
            _previousKey = previousKey;
            _previousObject = previousObject;

            _currentKey = default(TKey);
            _hasSelection = false;

            RaiseSelectionChanged(
                hadPreviousSelection,
                previousKey,
                previousObject,
                false,
                default(TKey),
                null,
                reason);
        }

        private void RaiseSelectionChanged(
            bool hadPreviousSelection,
            TKey previousKey,
            Object previousObject,
            bool hasSelection,
            TKey currentKey,
            Object currentObject,
            SelectionChangeReason reason)
        {
            SelectionChanged?.Invoke(
                this,
                new SelectionChangedEventArgs<TKey>(
                    hadPreviousSelection,
                    previousKey,
                    previousObject,
                    hasSelection,
                    currentKey,
                    currentObject,
                    reason));
        }
    }
}
