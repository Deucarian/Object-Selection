# Changelog

## 1.0.0

- Created standalone UPM package `com.jorishoef.object-selection`.
- Added generic keyed `ObjectSelectionRegistry<TKey>` and `ObjectSelectionService<TKey>`.
- Added current and previous selection tracking, idempotent same-key selection, and selection change events.
- Added separate `ObjectHoverService<TKey>` with hover start, end, and change events.
- Added `RaycastSelectionController<TKey>` as a no-UI-dependency raycast input adapter.
- Added `IObjectSelectionHighlighter<TKey>` hook interface for consumer-owned rendering effects.
- Added EditMode tests, Primitive Selection sample, README, contributing guide, validation tooling, and CI workflows.
