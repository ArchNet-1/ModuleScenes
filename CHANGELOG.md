# Changelog

## [2.0.0] - 2026-11-25

Unity 6 release.

### Changed
- Requires Unity 6000.0 or newer.
- Code split into `Runtime/` (game code) and `Editor/` (Editor-only assembly, never in builds).
- Namespace `ArchNet.<Category>`; every file starts with a short description and a link to the documentation.
- Files moved with their `.meta`: existing scenes, prefabs and assets keep their references.
- The scene is stored by name, not by its position in the build list.
- Clear error when the scene is not in the build.
- Package id `com.archnet.extension.scene.loader` → `com.archnet.extension.loadscene`.
