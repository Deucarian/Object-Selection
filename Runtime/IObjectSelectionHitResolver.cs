using UnityEngine;

namespace Deucarian.ObjectSelection
{
    public interface IObjectSelectionHitResolver<TKey>
    {
        bool TryResolve(RaycastHit hit, out TKey key);
    }
}
