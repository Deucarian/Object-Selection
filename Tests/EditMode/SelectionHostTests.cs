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
                a.Register("item", target); b.Register("item", target);
                a.Select("item");
                Assert.That(b.Selection.HasSelection, Is.False);
                b.Select("item");
                a.Unregister("item");
                Assert.That(a.Selection.HasSelection, Is.False);
                Assert.That(b.Selection.CurrentKey, Is.EqualTo("item"));
            }
            finally
            {
                Object.DestroyImmediate(first); Object.DestroyImmediate(second); Object.DestroyImmediate(target);
            }
        }
    }
}
