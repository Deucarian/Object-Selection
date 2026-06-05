using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Tests
{
    public sealed class ObjectHoverServiceTests
    {
        [Test]
        public void HoverStartedAndEndedAreSeparateFromSelection()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));
                var hover = new ObjectHoverService<string>(registry);
                var selection = new ObjectSelectionService<string>(registry);
                var started = new List<HoverChangedEventArgs<string>>();
                var ended = new List<HoverChangedEventArgs<string>>();

                hover.HoverStarted += (_, args) => started.Add(args);
                hover.HoverEnded += (_, args) => ended.Add(args);

                hover.SetHover("cube");
                hover.ClearHover();

                Assert.IsFalse(selection.HasSelection);
                Assert.AreEqual(1, started.Count);
                Assert.AreEqual("cube", started[0].CurrentKey);
                Assert.AreSame(cube, started[0].CurrentObject);
                Assert.AreEqual(1, ended.Count);
                Assert.AreEqual("cube", ended[0].PreviousKey);
                Assert.AreSame(cube, ended[0].PreviousObject);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        [Test]
        public void SameKeyHoverIsIdempotent()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));
                var hover = new ObjectHoverService<string>(registry);
                int startedCount = 0;
                hover.HoverStarted += (_, __) => startedCount++;

                hover.SetHover("cube");
                hover.SetHover("cube");

                Assert.AreEqual(1, startedCount);
                Assert.IsTrue(hover.HasHover);
                Assert.AreEqual("cube", hover.CurrentHoveredKey);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }
    }
}
