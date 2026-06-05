using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Tests
{
    public sealed class ObjectSelectionRegistryTests
    {
        [Test]
        public void RegisterWorks()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                Assert.AreEqual(1, registry.Count);
                Assert.IsTrue(registry.ContainsKey("cube"));
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void UnregisterWorks()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                Assert.IsTrue(registry.Unregister("cube"));
                Assert.AreEqual(0, registry.Count);
                Assert.IsFalse(registry.ContainsKey("cube"));
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void ResolvesKeyToObject()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                Object resolved;
                Assert.IsTrue(registry.TryGetObject("cube", out resolved));
                Assert.AreSame(cube, resolved);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void ResolvesObjectToKey()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                string key;
                Assert.IsTrue(registry.TryGetKey(cube, out key));
                Assert.AreEqual("cube", key);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void ResolvesComponentToKeyThroughGameObject()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                string key;
                Assert.IsTrue(registry.TryGetKey(cube.GetComponent<Collider>(), out key));
                Assert.AreEqual("cube", key);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void DestroyedObjectHandlingBehavesSafely()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            registry.Register(new SelectableObject<string>("cube", cube));
            Object.DestroyImmediate(cube);

            Object resolved;
            string key;

            Assert.IsFalse(registry.TryGetObject("cube", out resolved));
            Assert.IsFalse(registry.TryGetKey(cube, out key));
            Assert.AreEqual(1, registry.Count);
            Assert.AreEqual(1, registry.RemoveDestroyedEntries());
            Assert.AreEqual(0, registry.Count);
        }

        [Test]
        public void DuplicateObjectWithDifferentKeyThrows()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));

                Assert.Throws<InvalidOperationException>(
                    () => registry.Register(new SelectableObject<string>("duplicate", cube)));
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }
    }
}
