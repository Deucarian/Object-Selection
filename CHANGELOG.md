# Changelog

## 1.0.4 - 2026-07-17

- Applied the portfolio sample contract and aligned the exact Logging dependency.

## 1.0.3 - 2026-06-22

- Accepted the public release automation state for `com.deucarian.object-selection` 1.0.3 on develop.

## 1.0.2 - 2026-06-17

- Renamed package ecosystem references from Core State bridge packages to Integration packages.

## Unreleased

- Declared the built-in Unity Physics module dependency required by `RaycastSelectionController<TKey>`.
- Added optional object selection and hover visual strategy interfaces.
- Added visual controllers that subscribe to selection or hover services without owning state.
- Added dependency-free renderer tint and transform scale selection visuals.
- Updated the Primitive Selection sample to use a visual strategy controller.
- Added EditMode coverage for selection visual controller behavior.

## 1.0.1

- Standardized package logging on com.deucarian.logging.
- Added `SelectionLog` package categories for selection package and sample diagnostics.

## 1.0.0

- Created standalone UPM package `com.deucarian.object-selection`.
- Added generic keyed `ObjectSelectionRegistry<TKey>` and `ObjectSelectionService<TKey>`.
- Added current and previous selection tracking, idempotent same-key selection, and selection change events.
- Added separate `ObjectHoverService<TKey>` with hover start, end, and change events.
- Added `RaycastSelectionController<TKey>` as a no-UI-dependency raycast input adapter.
- Added `IObjectSelectionHighlighter<TKey>` hook interface for consumer-owned rendering effects.
- Added EditMode tests, Primitive Selection sample, README, contributing guide, validation tooling, and CI workflows.
