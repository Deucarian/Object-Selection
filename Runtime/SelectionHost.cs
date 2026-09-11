using System;
using UnityEngine;

namespace Deucarian.ObjectSelection
{
    /// <summary>Owns selection and optional highlighting for one scene or viewport.</summary>
    [DisallowMultipleComponent]
    public sealed class SelectionHost : MonoBehaviour
    {
        private readonly ObjectSelectionRegistry<SelectionHandle> registry = new ObjectSelectionRegistry<SelectionHandle>();
        private ObjectSelectionService<SelectionHandle> selection;
        private ObjectSelectionVisualController<SelectionHandle> visuals;
        private bool destroyed;
        private ObjectSelectionService<SelectionHandle> Service
        {
            get
            {
                if (destroyed) throw new ObjectDisposedException(nameof(SelectionHost));
                return selection ?? (selection = new ObjectSelectionService<SelectionHandle>(registry));
            }
        }

        public IReadOnlyObjectSelection<SelectionHandle> Selection => Service;
        public event EventHandler<SelectionChangedEventArgs<SelectionHandle>> SelectionChanged
        {
            add { Service.SelectionChanged += value; }
            remove { if (selection != null) selection.SelectionChanged -= value; }
        }

        public SelectionHandle Register(UnityEngine.Object target)
        {
            var service = Service;
            if (target == null) throw new ArgumentNullException(nameof(target), "Register an existing object before selecting it.");
            if (registry.TryGetKey(target, out var existing)) return existing;
            var handle = new SelectionHandle(this);
            registry.Register(new SelectableObject<SelectionHandle>(handle, target));
            return handle;
        }
        public bool Contains(SelectionHandle handle) => !destroyed && handle != null && handle.BelongsTo(this) &&
            registry.TryGetObject(handle, out var target) && target != null;
        public void Unregister(SelectionHandle handle)
        {
            if (destroyed || handle == null || !handle.BelongsTo(this)) return;
            if (Service.HasSelection && Service.CurrentKey == handle) Clear();
            registry.Unregister(handle);
        }
        public void ConfigureVisual(IObjectSelectionVisual<SelectionHandle> visual)
        {
            var service = Service;
            visuals?.Dispose();
            visuals = visual == null ? null : new ObjectSelectionVisualController<SelectionHandle>(service, visual);
        }
        public SelectionRequestResult Select(SelectionHandle handle)
        {
            var service = Service;
            if (handle == null) throw new ArgumentNullException(nameof(handle), "Use the SelectionHandle returned by Register or supplied by this scope's selection state.");
            if (!handle.BelongsTo(this)) return SelectionRequestResult.ForeignScope;
            if (!Contains(handle)) return SelectionRequestResult.Unavailable;
            service.Select(handle);
            return SelectionRequestResult.Selected;
        }
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
