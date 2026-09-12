# Object-Selection: definition workflow

Selectable registers a real scene object while enabled. Its handle belongs to the host scope; no invented global ID is needed.

## Try the package sample

1. Install this package and its declared dependencies. In Package Manager, import
   **Definition Workflow** from Samples.
2. Open the imported `DefinitionWorkflow.unity` scene and enter Play mode.
3. Use its buttons to exercise select, clear selection.
4. Inspect the configured hosts and triggers, then open
   [ObjectSelectionWorkflow.cs](Runtime/ObjectSelectionWorkflow.cs). It is the caller
   example; any Sample...Setup component is the one-time application composition.

## Use your own types and scope

This package's keys, enums or handles describe C# contracts and runtime objects.
They do not require a global content asset. Reuse a central typed key declaration
where the sample defines one; select that same key in serialized fields.
Payload types, handlers, storage policies and provider composition remain explicit
C# so the compiler can check the contract.

The sample separates caller code from startup composition. Reuse package hosts
and components; adapt only the application-specific data or provider. Runtime
handles come from their owning scope and must not be fabricated or transferred
to a different scope.

## Code and Inspector calls

The sample demonstrates these actions:

- **Select**: `ObjectSelectionWorkflow.Select()`.
- **Clear selection**: `ObjectSelectionWorkflow.Clear()`.

For a Unity button or event, assign the relevant package trigger component and
select its public void method. For ordinary C#, call the host/service's typed
method and inspect its returned result. Domain failures such as unavailable
services, an expired offer or an invalid target remain observable outcomes.
Missing setup reports the required host, definition or binding instead of silently
creating another service.

See the [shared authoring guide](https://github.com/Deucarian/Editor/blob/develop/Documentation~/DefinitionAuthoring.md) for code-first creation, generated
assembly references, ownership, conflicts, deletion and troubleshooting.
