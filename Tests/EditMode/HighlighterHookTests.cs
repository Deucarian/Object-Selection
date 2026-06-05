using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JorisHoef.ObjectSelection.Tests
{
    public sealed class HighlighterHookTests
    {
        [Test]
        public void HighlightHooksCanSubscribeToSelectionChanges()
        {
            var registry = new ObjectSelectionRegistry<string>();
            var cube = new GameObject("Cube");

            try
            {
                registry.Register(new SelectableObject<string>("cube", cube));
                var selection = new ObjectSelectionService<string>(registry);
                var highlighter = new RecordingHighlighter();

                selection.SelectionChanged += (_, args) => highlighter.OnSelectionChanged(args);
                selection.Select("cube");

                Assert.AreEqual(1, highlighter.Changes.Count);
                Assert.AreEqual("cube", highlighter.Changes[0].CurrentKey);
                Assert.AreSame(cube, highlighter.Changes[0].CurrentObject);
            }
            finally
            {
                Object.DestroyImmediate(cube);
            }
        }

        private sealed class RecordingHighlighter : IObjectSelectionHighlighter<string>
        {
            public readonly List<SelectionChangedEventArgs<string>> Changes =
                new List<SelectionChangedEventArgs<string>>();

            public void OnSelectionChanged(SelectionChangedEventArgs<string> args)
            {
                Changes.Add(args);
            }
        }
    }
}
