using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.ObjectSelection.Tests
{
    public sealed class ObjectSelectionCompositionTests
    {
        [Test]
        public void SelectionServiceAcceptsAnAbstractRegistry()
        {
            GameObject target = new GameObject("Abstract Registry Target");
            try
            {
                var registry = new StubRegistry(3, target);
                var service = new ObjectSelectionService<int>(registry);

                Assert.IsTrue(service.TrySelect(3));
                Assert.AreSame(target, service.CurrentObject);
                Assert.AreSame(registry, service.SelectionRegistry);
            }
            finally
            {
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void RaycastInteractorComposesHitResolutionAndSelectionCommands()
        {
            var resolver = new StubHitResolver(12);
            var commands = new RecordingCommands();
            var interactor = new ObjectSelectionRaycastInteractor<int>(resolver, commands);

            bool selected = interactor.TrySelect(default(RaycastHit));

            Assert.IsTrue(selected);
            Assert.AreEqual(12, commands.SelectedKey);
            Assert.AreEqual(SelectionChangeReason.Raycast, commands.Reason);
        }

        private sealed class StubHitResolver : IObjectSelectionHitResolver<int>
        {
            private readonly int _key;

            public StubHitResolver(int key)
            {
                _key = key;
            }

            public bool TryResolve(RaycastHit hit, out int key)
            {
                key = _key;
                return true;
            }
        }

        private sealed class RecordingCommands : IObjectSelectionCommands<int>
        {
            public int SelectedKey { get; private set; }
            public SelectionChangeReason Reason { get; private set; }

            public void Select(
                int key,
                SelectionChangeReason reason = SelectionChangeReason.Programmatic,
                bool forceEvent = false)
            {
                SelectedKey = key;
                Reason = reason;
            }

            public bool TrySelect(
                int key,
                SelectionChangeReason reason = SelectionChangeReason.Programmatic,
                bool forceEvent = false)
            {
                Select(key, reason, forceEvent);
                return true;
            }

            public void ClearSelection(
                SelectionChangeReason reason = SelectionChangeReason.Cleared,
                bool forceEvent = false)
            {
                Reason = reason;
            }
        }

        private sealed class StubRegistry : IReadOnlyObjectSelectionRegistry<int>
        {
            private readonly int _key;
            private readonly Object _target;
            private readonly ISelectableObject<int> _selectable;

            public StubRegistry(int key, GameObject target)
            {
                _key = key;
                _target = target;
                _selectable = new SelectableObject<int>(key, target);
            }

            public int Count => 1;
            public IReadOnlyCollection<int> Keys => new[] { _key };

            public bool ContainsKey(int key)
            {
                return key == _key;
            }

            public bool TryGetSelectable(int key, out ISelectableObject<int> selectable)
            {
                selectable = key == _key ? _selectable : null;
                return selectable != null;
            }

            public bool TryGetObject(int key, out Object targetObject)
            {
                targetObject = key == _key ? _target : null;
                return targetObject != null;
            }

            public bool TryGetGameObject(int key, out GameObject gameObject)
            {
                gameObject = key == _key ? _target as GameObject : null;
                return gameObject != null;
            }

            public bool TryGetKey(Object unityObject, out int key)
            {
                key = ReferenceEquals(unityObject, _target) ? _key : default(int);
                return key == _key;
            }

            public bool TryGetKey(GameObject gameObject, out int key)
            {
                return TryGetKey((Object)gameObject, out key);
            }

            public bool TryGetKey(Component component, out int key)
            {
                return TryGetKey(component != null ? component.gameObject : null, out key);
            }
        }
    }
}
