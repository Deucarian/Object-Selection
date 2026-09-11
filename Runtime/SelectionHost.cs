using System;
using UnityEngine;

namespace Deucarian.ObjectSelection
{
    /// <summary>Owns selection and optional highlighting for one scene or viewport.</summary>
    [DisallowMultipleComponent]
    public sealed class SelectionHost : MonoBehaviour
    {
        private readonly ObjectSelectionRegistry<string> registry = new ObjectSelectionRegistry<string>();
        private ObjectSelectionService<string> selection;
        private ObjectSelectionVisualController<string> visuals;
        private bool destroyed;
        private ObjectSelectionService<string> Service
        {
            get
            {
                if (destroyed) throw new ObjectDisposedException(nameof(SelectionHost));
                return selection ?? (selection = new ObjectSelectionService<string>(registry));
            }
        }

        public IReadOnlyObjectSelection<string> Selection => Service;
        public event EventHandler<SelectionChangedEventArgs<string>> SelectionChanged
        {
            add { Service.SelectionChanged += value; }
            remove { if (selection != null) selection.SelectionChanged -= value; }
        }

        public void Register(string id, UnityEngine.Object target)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A selection ID is required.", nameof(id));
            var service = Service;
            if (registry.ContainsKey(id)) throw new InvalidOperationException("This selection ID is already registered.");
            registry.Register(new SelectableObject<string>(id, target));
        }
        public void Unregister(string id)
        {
            if (Service.HasSelection && Service.CurrentKey == id) Clear();
            registry.Unregister(id);
        }
        public void ConfigureVisual(IObjectSelectionVisual<string> visual)
        {
            var service = Service;
            visuals?.Dispose();
            visuals = visual == null ? null : new ObjectSelectionVisualController<string>(service, visual);
        }
        public void Select(string id) => Service.Select(id);
        public void Clear() => Service.ClearSelection();
        private void LateUpdate()
        {
            registry.RemoveDestroyedEntries();
            if (selection != null && selection.HasSelection && selection.CurrentObject == null) selection.ClearSelection();
        }
        private void OnDestroy()
        {
            destroyed = true;
            visuals?.Dispose();
            registry.Clear();
            selection = null;
        }
    }
}
