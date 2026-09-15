using System;
using UnityEngine;

namespace Deucarian.ObjectSelection.Samples.DefinitionWorkflow
{
    /// <summary>Small caller example. The configured scene hosts own services and resource lifetimes.</summary>
    public sealed class ObjectSelectionWorkflow : MonoBehaviour
    {
        [SerializeField] private Selectable target;
        private string status = "Ready. Choose an action below.";
        public string Status => status;
        public void Select() { target.Select(); status = "Selected the registered sample object."; }
        public void Clear() { target.Deselect(); status = "Selection cleared."; }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(24, 24, Math.Min(540, Screen.width - 48), Screen.height - 48), GUI.skin.box);
            GUILayout.Label("Object-Selection — definition workflow");
            GUILayout.Label("Selectable registers a real scene object while enabled. Its handle belongs to the host scope; no invented global ID is needed.");
            GUILayout.Space(12);
            if (GUILayout.Button("Select", GUILayout.Height(32))) { try { Select(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Clear selection", GUILayout.Height(32))) { try { Clear(); } catch (Exception error) { status = error.Message; } }
            GUILayout.Space(12);
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
