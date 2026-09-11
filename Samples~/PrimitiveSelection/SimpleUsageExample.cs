using UnityEngine;

namespace Deucarian.ObjectSelection.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private SelectionHost selection;
        public void SelectObject(string id) => selection.Select(id);
        public void ClearSelection() => selection.Clear();
    }
}
