# Primitive Selection Sample

Open `PrimitiveSelection.unity` and enter Play Mode.

The sample creates four primitives and registers them with stable string keys:

- `cube`
- `sphere`
- `capsule`
- `cylinder`

Click a primitive to select it through `PrimitiveRaycastSelectionController`. Use the on-screen IMGUI buttons or number keys `1` through `4` for programmatic selection. Press Backspace or the Clear button to clear selection.

The sample shows current selection, previous selection, and the last selection event. `ObjectSelectionVisualController<string>` connects selection events to a sample-only visual strategy that tints the selected primitive and restores the previous primitive.
