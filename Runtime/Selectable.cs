using System;
using UnityEngine;
using UnityEngine.Events;
namespace Deucarian.ObjectSelection
{
    /// <summary>Registers this object while enabled. The host owns selection state.</summary>
    public sealed class Selectable : MonoBehaviour
    {
        [SerializeField] private SelectionHost host;
        [SerializeField] private UnityEvent selected = new UnityEvent();
        [SerializeField] private UnityEvent deselected = new UnityEvent();
        private SelectionHandle handle;
        private bool wasSelected;
        public SelectionHandle Handle => handle ?? throw new InvalidOperationException("Enable Selectable and assign its SelectionHost before requesting its handle.");
        private void OnEnable()
        {
            if (host == null) throw new InvalidOperationException("Assign a SelectionHost to Selectable.");
            handle = host.Register(gameObject);
            host.SelectionChanged += Changed;
        }
        public void Select() => host.Select(Handle);
        public void Deselect() { if (host != null && host.Selection.HasSelection && host.Selection.CurrentKey == handle) host.Clear(); }
        private void Changed(object sender, SelectionChangedEventArgs<SelectionHandle> args)
        {
            bool value = host.Selection.HasSelection && host.Selection.CurrentKey == handle;
            if (value == wasSelected) return;
            wasSelected = value;
            if (value) selected.Invoke(); else deselected.Invoke();
        }
        private void OnDisable()
        {
            if (host != null) { host.Unregister(handle); host.SelectionChanged -= Changed; }
            handle = null; wasSelected = false;
        }
    }
}
