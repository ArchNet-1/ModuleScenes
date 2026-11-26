# Changelog

## [2.1.0] - 2026-11-25

### Renamed
- Repository `LoadSceneExtension` is now `ModuleScenes`, package id `com.archnet.extension.loadscene` is now the one in package.json. Remove the old id from Packages/manifest.json and install this one.

### Added
- The Back key (ArchNet Input Module) closes this screen cleanly: the loading screen ignores it; the pause menu resumes the game, and the Gameplay scene pauses on BackPressed; Controls buttons in the menu and the pause menu when the Controls service exists.
- `SceneLoader` and `LoadingView`: asynchronous loading behind a loading screen (progress bar, percentage, random tip, minimum duration), time scale reset after a load.
- ArchNet Hub services: **Loading screen**, **Gameplay** (scene with HUD area and pause menu) and **Main menu** (first scene, buttons wired to the services present).

### Changed
- Needs ArchNet Core and `com.unity.ugui`. The `LoadSceneExtension` component is unchanged.

## [2.0.0] - 2026-11-25

### Renamed
- Repository `LoadSceneExtension` is now `ModuleScenes`, package id `com.archnet.extension.loadscene` is now the one in package.json. Remove the old id from Packages/manifest.json and install this one.

Unity 6 release.

### Changed
- Requires Unity 6000.0 or newer.
- Code split into `Runtime/` (game code) and `Editor/` (Editor-only assembly, never in builds).
- Namespace `ArchNet.<Category>`; every file starts with a short description and a link to the documentation.
- Files moved with their `.meta`: existing scenes, prefabs and assets keep their references.
- The scene is stored by name, not by its position in the build list.
- Clear error when the scene is not in the build.
- Package id `com.archnet.extension.scene.loader` → `com.archnet.module.scenes`.
