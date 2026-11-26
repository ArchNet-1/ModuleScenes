<p align="center">
  <a href="https://archnet-plateform.fr"><img src="https://archnet-plateform.fr/ArchNetLogoWithBck.png" alt="ArchNet" width="240"></a>
</p>

# ArchNet · Load Scene Extension

The scenes of your game: a loading screen, a ready main menu and a gameplay scene with a pause menu.

- Unity **6000.0** or newer · Needs `com.unity.ugui` and [ArchNet Core](https://github.com/ArchNet-1/ArchNetCore)
- Part of [ArchNet](https://archnet-plateform.fr), free and open-source Unity packages

## Add them to your game (one click each)

With the [ArchNet Hub](https://github.com/ArchNet-1/ArchNetHub), *ArchNet > Hub > Services*:

| Service | Creates | Use |
|---|---|---|
| **Loading screen** | `Assets/ArchNet/Loading/`: `GameLoader.cs`, `Loading.prefab` (progress bar, percentage, tips) under the ArchNet Bootstrap | `Services.Get<GameLoader>().LoadScene(GameLoader.Gameplay);` |
| **Gameplay** | `Assets/ArchNet/Gameplay/`: `GameplayZone.cs`, `Gameplay.unity` (HUD area, Pause button, pause menu) | Build your game in this scene. `Pause()`, `Resume()`, `BackToMenu()`. |
| **Main menu** | `Assets/ArchNet/Menu/`: `MainMenu.cs`, `MainMenu.unity`, first scene of the build | Play, Settings, Credits, Quit, already wired. Adds Loading and Gameplay if missing. |

The menu and the pause menu show the buttons of the services you already added (Settings, Credits, Popups to confirm Quit, Save to continue slot 0). Add a service later, then click **Update** on the Menu and Gameplay cards: the scripts are rewritten if you have not changed them (otherwise a `.new` file is written next to yours). Scenes you already have are kept.

Loading keeps the screen at least 0.6 s (no flash) and resets the time scale, so leaving a paused game never freezes the next scene. **Open tool** opens the prefab or the scene.

## The component

**ArchNet > Load Scene** still loads a scene on start, after a delay, or when `Load()` is called from a button.

## License

MIT, see [LICENSE.md](LICENSE.md).
