using UnityEngine;

namespace Deucarian.ObjectSelection.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private SelectionHost selection;
        public void SelectObject(SelectionHandle handle) => selection.Select(handle);
        public void ClearSelection() => selection.Clear();
    }
}
