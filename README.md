# Deucarian Object Selection

## Overview

Deucarian Object Selection is a standalone Unity runtime package for selecting scene or Unity objects by stable keys.

The package keeps the selection core independent from input systems. `ObjectSelectionService<TKey>` owns selection state, while click raycasts, XR interactions, hotkeys, network messages, UI buttons, editor tools, or application code can all select the same way: call `Select(key)` or `TrySelect(key)`.

Package ID: `com.deucarian.object-selection`

## Installation

Install the package through Unity Package Manager with a Git URL:

```json
{
  "dependencies": {
    "com.deucarian.object-selection": "https://github.com/Deucarian/ObjectSelection.git#main"
  }
}
```

For development builds, use:

```json
"com.deucarian.object-selection": "https://github.com/Deucarian/ObjectSelection.git#develop"
```

The package requires Unity `2021.3` or newer and depends on Unity's built-in `com.unity.modules.physics` module for the raycast input adapter.

For local development, reference the package by file path from a separate Unity test project:

```json
"com.deucarian.object-selection": "file:C:/Repositories/ObjectSelection"
```

## Core Concepts

Selection identity is always `TKey`. `GameObject`, `Component`, and `UnityEngine.Object` references are payloads that can be resolved through the registry, but they are not the identity.

`ISelectableObject<TKey>` is the scene/object contract. It exposes an `Id`, a selected `TargetObject`, and an optional `SourceGameObject` for scene interaction.

`ObjectSelectionRegistry<TKey>` maps keys to selectable objects and supports fast lookup from keys, Unity objects, game objects, and components. It treats destroyed Unity objects safely and exposes explicit stale-entry cleanup.

`ObjectSelectionService<TKey>` tracks the current and previous selected key/object and raises `SelectionChanged` when the selection changes. Selecting the same key twice is idempotent by default.

Input adapters do not own state. `RaycastSelectionController<TKey>` converts mouse raycasts into keyed selection commands, and consumers can write additional adapters for XR, hotkeys, networking, UI, AI, or editor tools.

Hover is separate. `ObjectHoverService<TKey>` tracks current hover state and raises hover start/end/change events without changing selection state.

Visuals are extension points. Selection and hover services decide what key is selected or hovered; visual strategies decide how that state looks. Runtime exposes `IObjectSelectionVisual<TKey>`, `IObjectHoverVisual<TKey>`, and small controller adapters that subscribe to service events and call the visual strategy. Advanced visuals such as DOTween, custom tweens, outlines, shader effects, Animator states, or VFX belong in consumers or future packages without changing selection state architecture.

`IObjectSelectionHighlighter<TKey>` remains available as a low-level event hook for existing consumers.

## Public API

- `ISelectableObject<TKey>`: selectable object contract.
- `SelectableObject<TKey>`: simple immutable selectable object implementation.
- `ObjectSelectionRegistry<TKey>`: key/object registry and resolver.
- `ObjectSelectionService<TKey>`: current and previous selection state.
- `SelectionChangedEventArgs<TKey>`: previous and current selection event payload.
- `SelectionChangeReason`: simple reason enum for programmatic, raycast, hover, and clear operations.
- `ObjectHoverService<TKey>`: hover state tracking independent from selection.
- `HoverChangedEventArgs<TKey>`: hover event payload.
- `RaycastSelectionController<TKey>`: mouse-to-raycast selection adapter.
- `IObjectSelectionVisual<TKey>`: strategy contract for selected and deselected visuals.
- `ObjectSelectionVisualController<TKey>`: subscribes to selection changes and applies an `IObjectSelectionVisual<TKey>`.
- `IObjectHoverVisual<TKey>`: strategy contract for hovered and unhovered visuals.
- `ObjectHoverVisualController<TKey>`: subscribes to hover changes and applies an `IObjectHoverVisual<TKey>`.
- `RendererTintSelectionVisual<TKey>`: dependency-free renderer tint selection visual.
- `TransformScaleSelectionVisual<TKey>`: dependency-free transform scale selection visual.
- `IObjectSelectionHighlighter<TKey>`: selection highlight hook interface.

Basic workflow:

```csharp
using Deucarian.ObjectSelection;
using UnityEngine;

var registry = new ObjectSelectionRegistry<string>();
var selection = new ObjectSelectionService<string>(registry);

GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
registry.Register(new SelectableObject<string>("cube", cube));

selection.SelectionChanged += (_, args) =>
{
    Debug.Log($"Selected {args.CurrentKey}; previous was {args.PreviousKey}");
};

selection.Select("cube");
selection.ClearSelection();
```

Selection visuals are attached separately from state:

```csharp
var visual = new RendererTintSelectionVisual<string>(Color.yellow);
using var visualController = new ObjectSelectionVisualController<string>(selection, visual);
```

Raycast adapters are initialized with the same service:

```csharp
public sealed class StringRaycastSelectionController : RaycastSelectionController<string>
{
}

controller.Initialize(selection);
controller.ShouldIgnoreInput = () => false;
```

## Samples

The package contains one sample:

- `Primitive Selection`: `Samples~/PrimitiveSelection/PrimitiveSelection.unity`

Open the scene and enter Play Mode. The sample creates a cube, sphere, capsule, and cylinder with keys `cube`, `sphere`, `capsule`, and `cylinder`.

The sample demonstrates click selection, programmatic selection through IMGUI buttons and number keys, current selection, previous selection, selection changed events, and a simple sample-only selection visual strategy.

## Integrations

Object Selection has no compiled integration assembly and does not reference Core State, UI Binding, API, Session, UI Toolkit, UGUI, ServiceLocator, or backend architecture.

Future Core State integration should live in a bridge package that shares keys:

```text
ObjectSelection
        ^
ObjectSelection-CoreState-Bridge
        ^
CoreState
        ^
UIBinding-CoreState-Bridge
        ^
UIBinding
```

This package intentionally does not implement that bridge.

## Versioning

Current package version: `1.0.0`.

Branch strategy:

- `main`: stable package branch.
- `develop`: development package branch.

Use branch refs for active development and release tags when tags are available.

## Limitations

- Selection is single-item selection by key, not multi-select.
- Runtime visuals are intentionally small and optional; rich rendering effects belong in consuming code or dedicated visual packages.
- Runtime raycast input uses Unity's classic `Input` and `Physics` APIs and has no UI suppression dependency. Consumers can provide `ShouldIgnoreInput` when they want UI-aware suppression.
- The package does not provide persistence, networking, undo/redo, UI binding, Core State bridging, or service-location infrastructure.
