using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Deucarian.ObjectSelection.Tests
{
    public sealed class ObjectSelectionServiceTests
    {
        [Test]
        public void SelectUpdatesCurrentKey()
        {
            var fixture = new SelectionFixture();

            try
            {
                fixture.Selection.Select("cube");

                Assert.IsTrue(fixture.Selection.HasSelection);
                Assert.AreEqual("cube", fixture.Selection.CurrentKey);
                Assert.AreSame(fixture.Cube, fixture.Selection.CurrentObject);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void PreviousKeyTrackingWorks()
        {
            var fixture = new SelectionFixture();

            try
            {
                fixture.Selection.Select("cube");
                fixture.Selection.Select("sphere");

                Assert.IsTrue(fixture.Selection.HasPreviousSelection);
                Assert.AreEqual("sphere", fixture.Selection.CurrentKey);
                Assert.AreEqual("cube", fixture.Selection.PreviousKey);
                Assert.AreSame(fixture.Cube, fixture.Selection.PreviousObject);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void ClearSelectionWorks()
        {
            var fixture = new SelectionFixture();

            try
            {
                fixture.Selection.Select("cube");
                fixture.Selection.ClearSelection();

                Assert.IsFalse(fixture.Selection.HasSelection);
                Assert.IsTrue(fixture.Selection.HasPreviousSelection);
                Assert.AreEqual("cube", fixture.Selection.PreviousKey);
                Assert.AreSame(fixture.Cube, fixture.Selection.PreviousObject);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void SameKeySelectionIsIdempotent()
        {
            var fixture = new SelectionFixture();

            try
            {
                var events = new List<SelectionChangedEventArgs<string>>();
                fixture.Selection.SelectionChanged += (_, args) => events.Add(args);

                fixture.Selection.Select("cube");
                fixture.Selection.Select("cube");

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual("cube", events[0].CurrentKey);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void SelectionChangedEventContainsCorrectValues()
        {
            var fixture = new SelectionFixture();

            try
            {
                var events = new List<SelectionChangedEventArgs<string>>();
                fixture.Selection.SelectionChanged += (_, args) => events.Add(args);

                fixture.Selection.Select("cube");
                fixture.Selection.Select("sphere", SelectionChangeReason.Programmatic);

                Assert.AreEqual(2, events.Count);
                Assert.IsFalse(events[0].HadPreviousSelection);
                Assert.IsTrue(events[0].HasSelection);
                Assert.AreEqual("cube", events[0].CurrentKey);
                Assert.AreSame(fixture.Cube, events[0].CurrentObject);

                Assert.IsTrue(events[1].HadPreviousSelection);
                Assert.AreEqual("cube", events[1].PreviousKey);
                Assert.AreSame(fixture.Cube, events[1].PreviousObject);
                Assert.AreEqual("sphere", events[1].CurrentKey);
                Assert.AreSame(fixture.Sphere, events[1].CurrentObject);
                Assert.AreEqual(SelectionChangeReason.Programmatic, events[1].Reason);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void TrySelectMissingKeyReturnsFalse()
        {
            var fixture = new SelectionFixture();

            try
            {
                Assert.IsFalse(fixture.Selection.TrySelect("missing"));
                Assert.IsFalse(fixture.Selection.HasSelection);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        private sealed class SelectionFixture
        {
            public SelectionFixture()
            {
                Registry = new ObjectSelectionRegistry<string>();
                Cube = new GameObject("Cube");
                Sphere = new GameObject("Sphere");

                Registry.Register(new SelectableObject<string>("cube", Cube));
                Registry.Register(new SelectableObject<string>("sphere", Sphere));

                Selection = new ObjectSelectionService<string>(Registry);
            }

            public ObjectSelectionRegistry<string> Registry { get; }
            public ObjectSelectionService<string> Selection { get; }
            public GameObject Cube { get; }
            public GameObject Sphere { get; }

            public void Dispose()
            {
                Object.DestroyImmediate(Cube);
                Object.DestroyImmediate(Sphere);
            }
        }
    }
}
