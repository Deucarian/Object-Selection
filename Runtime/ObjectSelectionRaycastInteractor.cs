using System;
using UnityEngine;

namespace Deucarian.ObjectSelection
{
    public sealed class ObjectSelectionRaycastInteractor<TKey>
    {
        private readonly IObjectSelectionHitResolver<TKey> _hitResolver;
        private readonly IObjectSelectionCommands<TKey> _commands;

        public ObjectSelectionRaycastInteractor(
            IObjectSelectionHitResolver<TKey> hitResolver,
            IObjectSelectionCommands<TKey> commands)
        {
            _hitResolver = hitResolver ?? throw new ArgumentNullException(nameof(hitResolver));
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public bool TrySelect(
            RaycastHit hit,
            SelectionChangeReason reason = SelectionChangeReason.Raycast)
        {
            return _hitResolver.TryResolve(hit, out TKey key) &&
                   _commands.TrySelect(key, reason);
        }

        public void Clear(
            SelectionChangeReason reason = SelectionChangeReason.Cleared)
        {
            _commands.ClearSelection(reason);
        }
    }
}
