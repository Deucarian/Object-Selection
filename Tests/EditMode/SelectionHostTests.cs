using NUnit.Framework;
using UnityEngine;

namespace Deucarian.ObjectSelection.Tests
{
    public sealed class SelectionHostTests
    {
        [Test]
        public void HostsHaveIndependentSelectionAndUnregisterClearsOnlyItsOwnSelection()
        {
            var first = new GameObject("first");
            var second = new GameObject("second");
            var target = new GameObject("target");
            try
            {
                var a = first.AddComponent<SelectionHost>();
                var b = second.AddComponent<SelectionHost>();
                var firstHandle = a.Register(target); var secondHandle = b.Register(target);
                a.Select(firstHandle);
                Assert.That(b.Selection.HasSelection, Is.False);
                Assert.That(b.Select(firstHandle), Is.EqualTo(SelectionRequestResult.ForeignScope));
                b.Select(secondHandle);
                a.Unregister(firstHandle);
                Assert.That(a.Selection.HasSelection, Is.False);
                Assert.That(b.Selection.CurrentKey, Is.SameAs(secondHandle));
                Assert.That(a.Select(firstHandle), Is.EqualTo(SelectionRequestResult.Unavailable));
            }
            finally
            {
                Object.DestroyImmediate(first); Object.DestroyImmediate(second); Object.DestroyImmediate(target);
            }
        }
    }
}
