using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection
{
    public interface IReadOnlyObjectSelectionRegistry<TKey>
    {
        int Count { get; }
        IReadOnlyCollection<TKey> Keys { get; }

        bool ContainsKey(TKey key);
        bool TryGetSelectable(TKey key, out ISelectableObject<TKey> selectable);
        bool TryGetObject(TKey key, out Object targetObject);
        bool TryGetGameObject(TKey key, out GameObject gameObject);
        bool TryGetKey(Object unityObject, out TKey key);
        bool TryGetKey(GameObject gameObject, out TKey key);
        bool TryGetKey(Component component, out TKey key);
    }
}
