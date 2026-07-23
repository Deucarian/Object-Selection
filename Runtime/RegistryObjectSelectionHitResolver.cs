using System;
using UnityEngine;

namespace Deucarian.ObjectSelection
{
    public sealed class RegistryObjectSelectionHitResolver<TKey> :
        IObjectSelectionHitResolver<TKey>
    {
        private readonly IReadOnlyObjectSelectionRegistry<TKey> _registry;

        public RegistryObjectSelectionHitResolver(
            IReadOnlyObjectSelectionRegistry<TKey> registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public bool TryResolve(RaycastHit hit, out TKey key)
        {
            key = default(TKey);

            if (hit.collider != null && _registry.TryGetKey(hit.collider, out key))
            {
                return true;
            }

            if (hit.transform != null &&
                _registry.TryGetKey(hit.transform.gameObject, out key))
            {
                return true;
            }

            return hit.rigidbody != null && _registry.TryGetKey(hit.rigidbody, out key);
        }
    }
}
