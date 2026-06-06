using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Tests
{
    public sealed class ObjectSelectionVisualControllerTests
    {
        [Test]
        public void SelectedVisualIsAppliedToCurrentObject()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                using (new ObjectSelectionVisualController<string>(fixture.Selection, visual))
                {
                    fixture.Selection.Select("cube");

                    Assert.AreEqual(new[] { "cube" }, visual.SelectedKeys);
                    Assert.AreSame(fixture.Cube, visual.SelectedTargets[0]);
                }
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void DeselectedVisualIsAppliedToPreviousObject()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                using (new ObjectSelectionVisualController<string>(fixture.Selection, visual))
                {
                    fixture.Selection.Select("cube");
                    fixture.Selection.Select("sphere");

                    Assert.AreEqual(new[] { "cube" }, visual.DeselectedKeys);
                    Assert.AreSame(fixture.Cube, visual.DeselectedTargets[0]);
                    Assert.AreEqual(new[] { "cube", "sphere" }, visual.SelectedKeys);
                }
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void SameKeySelectionDoesNotReapplyVisuals()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                using (new ObjectSelectionVisualController<string>(fixture.Selection, visual))
                {
                    fixture.Selection.Select("cube");
                    fixture.Selection.Select("cube");

                    Assert.AreEqual(1, visual.SelectedKeys.Count);
                    Assert.AreEqual(0, visual.DeselectedKeys.Count);
                }
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void ClearingSelectionDeselectsPreviousObject()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                using (new ObjectSelectionVisualController<string>(fixture.Selection, visual))
                {
                    fixture.Selection.Select("cube");
                    fixture.Selection.ClearSelection();

                    Assert.AreEqual(new[] { "cube" }, visual.DeselectedKeys);
                    Assert.AreSame(fixture.Cube, visual.DeselectedTargets[0]);
                }
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void DisposeUnsubscribesFromSelectionChanges()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                var controller = new ObjectSelectionVisualController<string>(fixture.Selection, visual);

                controller.Dispose();
                fixture.Selection.Select("cube");

                Assert.AreEqual(0, visual.SelectedKeys.Count);
                Assert.AreEqual(0, visual.DeselectedKeys.Count);
            }
            finally
            {
                fixture.Dispose();
            }
        }

        [Test]
        public void DestroyedTargetsAreSafe()
        {
            var fixture = new SelectionFixture();

            try
            {
                var visual = new RecordingSelectionVisual();
                using (new ObjectSelectionVisualController<string>(fixture.Selection, visual))
                {
                    fixture.Selection.Select("cube");
                    Object.DestroyImmediate(fixture.Cube);

                    Assert.DoesNotThrow(() => fixture.Selection.ClearSelection());
                    Assert.AreEqual(1, visual.SelectedKeys.Count);
                    Assert.AreEqual(0, visual.DeselectedKeys.Count);
                }
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
            public GameObject Cube { get; private set; }
            public GameObject Sphere { get; private set; }

            public void Dispose()
            {
                if (Cube != null)
                {
                    Object.DestroyImmediate(Cube);
                    Cube = null;
                }

                if (Sphere != null)
                {
                    Object.DestroyImmediate(Sphere);
                    Sphere = null;
                }
            }
        }

        private sealed class RecordingSelectionVisual : IObjectSelectionVisual<string>
        {
            public readonly List<string> SelectedKeys = new List<string>();
            public readonly List<Object> SelectedTargets = new List<Object>();
            public readonly List<string> DeselectedKeys = new List<string>();
            public readonly List<Object> DeselectedTargets = new List<Object>();

            public void ApplySelected(string key, Object target)
            {
                SelectedKeys.Add(key);
                SelectedTargets.Add(target);
            }

            public void ApplyDeselected(string key, Object target)
            {
                DeselectedKeys.Add(key);
                DeselectedTargets.Add(target);
            }
        }
    }
}
