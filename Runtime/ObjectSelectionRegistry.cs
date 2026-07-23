using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    /// <summary>
    /// Stores the mappings between stable selection keys and Unity objects.
    /// </summary>
    /// <typeparam name="TKey">The stable selection key type.</typeparam>
    public sealed class ObjectSelectionRegistry<TKey> : IObjectSelectionRegistry<TKey>
    {
        private readonly Dictionary<TKey, ISelectableObject<TKey>> _selectables =
            new Dictionary<TKey, ISelectableObject<TKey>>();
        private readonly Dictionary<Object, TKey> _keysByObject = new Dictionary<Object, TKey>();

        /// <summary>
        /// Gets the number of registered keys.
        /// </summary>
        public int Count
        {
            get { return _selectables.Count; }
        }

        /// <summary>
        /// Gets the registered keys.
        /// </summary>
        public IReadOnlyCollection<TKey> Keys
        {
            get { return _selectables.Keys; }
        }

        /// <summary>
        /// Registers or replaces a selectable object for its key.
        /// </summary>
        /// <param name="selectable">The selectable object to register.</param>
        public void Register(ISelectableObject<TKey> selectable)
        {
            if (selectable == null)
            {
                throw new ArgumentNullException(nameof(selectable));
            }

            ValidateKey(selectable.Id);

            if (UnityObjectUtility.IsDestroyed(selectable.TargetObject))
            {
                throw new ArgumentException("Selectable target object must not be null or destroyed.", nameof(selectable));
            }

            EnsureObjectIsAvailable(selectable.TargetObject, selectable.Id);
            EnsureObjectIsAvailable(selectable.SourceGameObject, selectable.Id);

            ISelectableObject<TKey> previous;
            if (_selectables.TryGetValue(selectable.Id, out previous))
            {
                RemoveObjectMappings(previous);
            }

            _selectables[selectable.Id] = selectable;
            AddObjectMapping(selectable.TargetObject, selectable.Id);
            AddObjectMapping(selectable.SourceGameObject, selectable.Id);
        }

        /// <summary>
        /// Unregisters a key and its Unity object mappings.
        /// </summary>
        /// <param name="key">The key to unregister.</param>
        /// <returns><c>true</c> when a key was removed.</returns>
        public bool Unregister(TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                return false;
            }

            ISelectableObject<TKey> selectable;
            if (!_selectables.TryGetValue(key, out selectable))
            {
                return false;
            }

            RemoveObjectMappings(selectable);
            return _selectables.Remove(key);
        }

        /// <summary>
        /// Removes all registered keys and Unity object mappings.
        /// </summary>
        public void Clear()
        {
            _selectables.Clear();
            _keysByObject.Clear();
        }

        /// <summary>
        /// Removes registrations whose target Unity object has been destroyed.
        /// </summary>
        /// <returns>The number of removed entries.</returns>
        public int RemoveDestroyedEntries()
        {
            var removedKeys = new List<TKey>();

            foreach (KeyValuePair<TKey, ISelectableObject<TKey>> pair in _selectables)
            {
                if (IsSelectableDestroyed(pair.Value))
                {
                    removedKeys.Add(pair.Key);
                }
            }

            for (int i = 0; i < removedKeys.Count; i++)
            {
                Unregister(removedKeys[i]);
            }

            RebuildObjectMappings();
            return removedKeys.Count;
        }

        /// <summary>
        /// Determines whether the key is registered and points to a live target object.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> when a live selectable exists for the key.</returns>
        public bool ContainsKey(TKey key)
        {
            ISelectableObject<TKey> selectable;
            return TryGetSelectable(key, out selectable);
        }

        /// <summary>
        /// Resolves a key to the registered selectable object.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="selectable">The registered selectable object.</param>
        /// <returns><c>true</c> when the key exists and its target object is live.</returns>
        public bool TryGetSelectable(TKey key, out ISelectableObject<TKey> selectable)
        {
            selectable = null;

            if (ReferenceEquals(key, null))
            {
                return false;
            }

            ISelectableObject<TKey> registered;
            if (!_selectables.TryGetValue(key, out registered))
            {
                return false;
            }

            if (IsSelectableDestroyed(registered))
            {
                return false;
            }

            selectable = registered;
            return true;
        }

        /// <summary>
        /// Resolves a key to the registered Unity target object.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="targetObject">The registered target object.</param>
        /// <returns><c>true</c> when the key exists and its target object is live.</returns>
        public bool TryGetObject(TKey key, out Object targetObject)
        {
            targetObject = null;

            ISelectableObject<TKey> selectable;
            if (!TryGetSelectable(key, out selectable))
            {
                return false;
            }

            targetObject = selectable.TargetObject;
            return true;
        }

        /// <summary>
        /// Resolves a key to the registered or inferred GameObject.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="gameObject">The registered or inferred GameObject.</param>
        /// <returns><c>true</c> when a live GameObject can be resolved.</returns>
        public bool TryGetGameObject(TKey key, out GameObject gameObject)
        {
            gameObject = null;

            ISelectableObject<TKey> selectable;
            if (!TryGetSelectable(key, out selectable))
            {
                return false;
            }

            if (!UnityObjectUtility.IsDestroyed(selectable.SourceGameObject))
            {
                gameObject = selectable.SourceGameObject;
                return true;
            }

            gameObject = UnityObjectUtility.GetGameObject(selectable.TargetObject);
            return gameObject != null;
        }

        /// <summary>
        /// Resolves a Unity object to a registered key.
        /// </summary>
        /// <param name="unityObject">The Unity object to resolve.</param>
        /// <param name="key">The registered key.</param>
        /// <returns><c>true</c> when the object or its GameObject maps to a live selectable.</returns>
        public bool TryGetKey(Object unityObject, out TKey key)
        {
            key = default(TKey);

            if (UnityObjectUtility.IsDestroyed(unityObject))
            {
                return false;
            }

            if (TryGetLiveKeyFromObjectMap(unityObject, out key))
            {
                return true;
            }

            Component component = unityObject as Component;
            if (component != null && !UnityObjectUtility.IsDestroyed(component.gameObject))
            {
                return TryGetLiveKeyFromObjectMap(component.gameObject, out key);
            }

            return false;
        }

        /// <summary>
        /// Resolves a GameObject to a registered key.
        /// </summary>
        /// <param name="gameObject">The GameObject to resolve.</param>
        /// <param name="key">The registered key.</param>
        /// <returns><c>true</c> when the GameObject maps to a live selectable.</returns>
        public bool TryGetKey(GameObject gameObject, out TKey key)
        {
            return TryGetKey((Object)gameObject, out key);
        }

        /// <summary>
        /// Resolves a Component to a registered key, falling back to its GameObject mapping.
        /// </summary>
        /// <param name="component">The Component to resolve.</param>
        /// <param name="key">The registered key.</param>
        /// <returns><c>true</c> when the Component or its GameObject maps to a live selectable.</returns>
        public bool TryGetKey(Component component, out TKey key)
        {
            return TryGetKey((Object)component, out key);
        }

        private static void ValidateKey(TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        private static bool IsSelectableDestroyed(ISelectableObject<TKey> selectable)
        {
            return selectable == null || UnityObjectUtility.IsDestroyed(selectable.TargetObject);
        }

        private void AddObjectMapping(Object unityObject, TKey key)
        {
            if (UnityObjectUtility.IsDestroyed(unityObject))
            {
                return;
            }

            _keysByObject[unityObject] = key;
        }

        private void RemoveObjectMappings(ISelectableObject<TKey> selectable)
        {
            if (selectable == null)
            {
                return;
            }

            RemoveObjectMapping(selectable.TargetObject, selectable.Id);
            RemoveObjectMapping(selectable.SourceGameObject, selectable.Id);
        }

        private void RemoveObjectMapping(Object unityObject, TKey key)
        {
            if (ReferenceEquals(unityObject, null))
            {
                return;
            }

            TKey mappedKey;
            if (_keysByObject.TryGetValue(unityObject, out mappedKey) &&
                EqualityComparer<TKey>.Default.Equals(mappedKey, key))
            {
                _keysByObject.Remove(unityObject);
            }
        }

        private void EnsureObjectIsAvailable(Object unityObject, TKey key)
        {
            if (UnityObjectUtility.IsDestroyed(unityObject))
            {
                return;
            }

            TKey existingKey;
            if (!_keysByObject.TryGetValue(unityObject, out existingKey))
            {
                return;
            }

            if (!EqualityComparer<TKey>.Default.Equals(existingKey, key))
            {
                throw new InvalidOperationException("The Unity object is already registered with a different selection key.");
            }
        }

        private bool TryGetLiveKeyFromObjectMap(Object unityObject, out TKey key)
        {
            key = default(TKey);

            TKey mappedKey;
            if (!_keysByObject.TryGetValue(unityObject, out mappedKey))
            {
                return false;
            }

            ISelectableObject<TKey> selectable;
            if (!TryGetSelectable(mappedKey, out selectable))
            {
                return false;
            }

            key = mappedKey;
            return true;
        }

        private void RebuildObjectMappings()
        {
            _keysByObject.Clear();

            foreach (KeyValuePair<TKey, ISelectableObject<TKey>> pair in _selectables)
            {
                if (IsSelectableDestroyed(pair.Value))
                {
                    continue;
                }

                AddObjectMapping(pair.Value.TargetObject, pair.Key);
                AddObjectMapping(pair.Value.SourceGameObject, pair.Key);
            }
        }
    }
}
