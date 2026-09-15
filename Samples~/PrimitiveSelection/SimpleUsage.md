# Simple usage

Add a SelectionHost for each scene/viewport. Register each selectable once with host.Register(id, gameObject), then unregister it when removing the object. ConfigureVisual accepts the existing IObjectSelectionVisual<string> strategies, such as renderer tint or transform scale. SelectionChanged exposes the existing event, and Selection is the read-only selection port for raycast/UI adapters. Duplicate IDs are rejected. Unregister clears a selected object, and destroyed registrations are cleaned on LateUpdate. The host never destroys selectable objects.

Import the **Simple Usage** sample from Unity Package Manager. Its caller script is:

Definition fields now use named, domain-specific keys. Select an existing definition from the Inspector dropdown or pass the same named key in code. Declare each project key once in a marked key set; ordinary caller methods do not accept raw IDs. Generated keys for asset-authored definitions require no asset reference in the caller. Owner-issued selection and row handles represent runtime instances.

```csharp
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
```
