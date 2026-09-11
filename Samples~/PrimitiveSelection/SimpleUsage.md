# Simple usage

Add a SelectionHost for each scene/viewport. Register each selectable once with host.Register(id, gameObject), then unregister it when removing the object. ConfigureVisual accepts the existing IObjectSelectionVisual<string> strategies, such as renderer tint or transform scale. SelectionChanged exposes the existing event, and Selection is the read-only selection port for raycast/UI adapters. Duplicate IDs are rejected. Unregister clears a selected object, and destroyed registrations are cleaned on LateUpdate. The host never destroys selectable objects.

Import the **Simple Usage** sample from Unity Package Manager. Its caller script is:

```csharp
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
```
