# Contributing

## Scope

Object Selection is a standalone UPM package. Keep it independent from Core State, Generic UI Items, API Helper, Session Helper, UI Toolkit, UGUI, ServiceLocator, backend services, and application-specific architecture.

Selection identity must stay keyed by `TKey`. Unity objects are payloads for lookup and scene interaction; they are not the identity.

## Local Validation

Run structural validation from the package root:

```powershell
pwsh ./Tools/Validate-Package.ps1
```

For Unity validation, use a separate test project that references this package by file path:

```json
"com.jorishoef.object-selection": "file:C:/Repositories/ObjectSelection"
```

Package source should stay in this repository. Do not copy package code into the test project.

## Pull Requests

- Keep runtime changes focused.
- Add or update EditMode tests for behavior changes.
- Keep runtime asmdef free of editor-only references.
- Do not add dependencies beyond what the package actually uses.
- Put bridges to other packages in separate bridge packages, not in this runtime.
