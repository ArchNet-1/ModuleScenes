// ArchNet · Load Scene Extension — scene recipes (ArchNet Hub > Services)
//   Loading screen → Assets/ArchNet/Loading/: GameLoader.cs + Loading.prefab under the Bootstrap.
//   Gameplay       → Assets/ArchNet/Gameplay/: GameplayZone.cs + Gameplay.unity (HUD area, pause menu).
//   Menu           → Assets/ArchNet/Menu/: MainMenu.cs + MainMenu.unity, first scene of the build.
// The menu and the pause menu only show the buttons of services already created (Settings,
// Credits, Popups, Save); click Update on their card after adding one.
// This assembly only compiles when the ArchNet Hub (com.archnet.hub 2.0.0+) is installed.
// Docs: https://archnet-plateform.fr/#ModuleScenes

using System;
using System.Collections.Generic;
using ArchNet.Core;
using ArchNet.Core.Editor;
using ArchNet.Installer.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ArchNet.Extension.Editor
{
    static class SceneRecipeText
    {
        public const string Templates = "Packages/com.archnet.module.scenes/Editor/Recipe/Templates/";
        public static readonly string[] OptionalServices = { "settings", "credits", "popups", "save", "inputs", "controls" };

        public static void OpenScene(string path)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
            {
                ArchNetLog.Warning($"{path} was not found.", "Click Update on the service card to create it again.");
                return;
            }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(path);
        }

        public static Button MenuButton(string label, Transform column, bool primary)
        {
            Button button = UIBuilder.Button(label, column, label, primary ? UIBuilder.Accent : UIBuilder.Neutral, out _);
            var layout = button.GetComponent<LayoutElement>();
            layout.preferredWidth = 460;
            layout.preferredHeight = 96;
            return button;
        }

        public static RectTransform CenteredColumn(string name, Transform parent, Vector2 position)
        {
            RectTransform column = UIBuilder.Rect(name, parent);
            UIBuilder.Anchor(column, new Vector2(0.5f, 0.5f), new Vector2(460f, 0f), position);
            UIBuilder.Column(column, 24).childControlWidth = true;
            column.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return column;
        }
    }

    public sealed class LoadingRecipe : ServiceRecipe
    {
        public const string RecipeId = "loading";
        const string ServiceScript = "Scripts/GameLoader.cs";
        const string PrefabFile = "Prefabs/Loading.prefab";
        const int SortingOrder = 900; // above everything but popups

        public override string Id => RecipeId;
        public override string DisplayName => "Loading screen";
        public override string Description => "Loads scenes in the background behind a loading screen with a progress bar and tips: Services.Get<GameLoader>().LoadScene(\"Gameplay\").";
        public override string Category => "Scenes";
        public override string FolderName => "Loading";
        public override string[] RequiredServices => new[] { BootstrapRecipe.RecipeId };
        public override bool HasTool => true;

        public override void Create(RecipeContext context)
        {
            UIBuilder.EnsureTextMeshPro();
            context.WriteTemplate(ServiceScript, RecipeContext.LoadTemplate(SceneRecipeText.Templates + "GameLoader.cs.txt"));
        }

        public override void AfterScriptsCompiled(RecipeContext context)
        {
            Type serviceType = RecipeTools.GeneratedType<SceneLoader>(context, "GameLoader", ServiceScript);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(context.PathOf(PrefabFile))
                                ?? context.SavePrefab(PrefabFile, BuildScreen(serviceType));
            BootstrapRecipe.AddUnderBootstrap(prefab);
        }

        public override void OpenTool() =>
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>($"{RecipeRunner.RootFolder}/Loading/{PrefabFile}"));

        static GameObject BuildScreen(Type serviceType)
        {
            GameObject root = UIBuilder.ScreenCanvas("Loading", SortingOrder);
            var panel = new SerializedObject(root.GetComponent<UIPanel>());
            panel.FindProperty("_closableByBack").boolValue = false; // the Back key must not hide a load in progress
            panel.ApplyModifiedPropertiesWithoutUndo();
            Color opaque = UIBuilder.Backdrop;
            opaque.a = 1f; // hides the scene being left
            UIBuilder.Stretch(UIBuilder.Image("Background", root.transform, opaque).rectTransform);

            TextMeshProUGUI title = UIBuilder.Text("Title", root.transform, "Loading…", 64, style: FontStyles.Bold);
            UIBuilder.Anchor(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1200f, 100f), new Vector2(0f, 60f));

            Image bar = UIBuilder.Image("Bar", root.transform, UIBuilder.Neutral);
            UIBuilder.Anchor(bar.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1000f, 24f), new Vector2(0f, -40f));
            Image fill = UIBuilder.Image("Fill", bar.transform, UIBuilder.Accent);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = new Vector2(0f, 1f);
            fill.rectTransform.offsetMin = fill.rectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI percent = UIBuilder.Text("Percent", root.transform, "0 %", 36);
            percent.color = UIBuilder.MutedText;
            UIBuilder.Anchor(percent.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(400f, 60f), new Vector2(0f, -100f));

            TextMeshProUGUI tip = UIBuilder.Text("Tip", root.transform, "Tip", 34, style: FontStyles.Italic);
            tip.color = UIBuilder.MutedText;
            UIBuilder.Anchor(tip.rectTransform, new Vector2(0.5f, 0f), new Vector2(1500f, 120f), new Vector2(0f, 80f));

            var view = root.AddComponent<LoadingView>();
            RecipeTools.Assign(view, "_barFill", fill.rectTransform);
            RecipeTools.Assign(view, "_percent", percent);
            RecipeTools.Assign(view, "_tip", tip);
            Component service = root.AddComponent(serviceType);
            RecipeTools.Assign(service, "_view", view);
            return root;
        }
    }

    public sealed class GameplayRecipe : ServiceRecipe
    {
        public const string RecipeId = "gameplay";
        const string Script = "Scripts/GameplayZone.cs";
        const string SceneFile = "Gameplay.unity";

        public override string Id => RecipeId;
        public override string DisplayName => "Gameplay";
        public override string Description => "The gameplay scene to build your game in: a HUD area and a pause menu (Resume, Settings, Main menu) that freezes the game.";
        public override string Category => "Scenes";
        public override string FolderName => "Gameplay";
        public override string[] RequiredServices => new[] { BootstrapRecipe.RecipeId, LoadingRecipe.RecipeId };
        public override bool HasTool => true;

        public override void Create(RecipeContext context) =>
            context.WriteTemplate(Script, RecipeContext.LoadTemplate(SceneRecipeText.Templates + "GameplayZone.cs.txt"),
                RecipeRunner.CreatedFlags(SceneRecipeText.OptionalServices));

        public override void AfterScriptsCompiled(RecipeContext context)
        {
            Type zoneType = RecipeTools.GeneratedType<MonoBehaviour>(context, "GameplayZone", Script);
            Dictionary<string, string> created = RecipeRunner.CreatedFlags(SceneRecipeText.OptionalServices);
            RecipeTools.CreateScene(context, SceneFile, false, _ => Build(zoneType, created["settings"] == "true", created["controls"] == "true"));
        }

        public override void OpenTool() => SceneRecipeText.OpenScene($"{RecipeRunner.RootFolder}/Gameplay/{SceneFile}");

        static void Build(Type zoneType, bool settings, bool controls)
        {
            RecipeTools.AddCamera(new Color(0.16f, 0.18f, 0.24f));
            GameObject canvas = UIBuilder.ScreenCanvas("Gameplay UI", 0, asPanel: false);

            TextMeshProUGUI hud = UIBuilder.Text("HUD", canvas.transform, "HUD: score, health, timer… go here", 36, TextAlignmentOptions.TopLeft);
            hud.color = UIBuilder.MutedText;
            UIBuilder.Anchor(hud.rectTransform, new Vector2(0f, 1f), new Vector2(900f, 80f), new Vector2(40f, -40f));

            Button pause = UIBuilder.Button("Pause Button", canvas.transform, "Pause", UIBuilder.Neutral, out _);
            UIBuilder.Anchor((RectTransform)pause.transform, new Vector2(1f, 1f), new Vector2(220f, 84f), new Vector2(-40f, -40f));

            RectTransform pauseMenu = UIBuilder.Stretch(UIBuilder.Rect("Pause Menu", canvas.transform));
            var panel = pauseMenu.gameObject.AddComponent<UIPanel>();
            pauseMenu.GetComponent<CanvasGroup>().alpha = 0f; // hidden while you edit the scene, like in the game
            UIBuilder.Stretch(UIBuilder.Image("Shade", pauseMenu, UIBuilder.Shade).rectTransform);
            RectTransform column = SceneRecipeText.CenteredColumn("Buttons", pauseMenu, Vector2.zero);
            UIBuilder.Text("Title", column, "Paused", 64, style: FontStyles.Bold).gameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            Button resume = SceneRecipeText.MenuButton("Resume", column, true);
            Button openSettings = settings ? SceneRecipeText.MenuButton("Settings", column, false) : null;
            Button openControls = controls ? SceneRecipeText.MenuButton("Controls", column, false) : null;
            Button menu = SceneRecipeText.MenuButton("Main menu", column, false);

            Component zone = canvas.AddComponent(zoneType);
            RecipeTools.Assign(zone, "_pauseMenu", panel);
            RecipeTools.OnClick(pause, zone, "Pause");
            RecipeTools.OnClick(resume, zone, "Resume");
            if (openSettings != null) RecipeTools.OnClick(openSettings, zone, "OpenSettings");
            if (openControls != null) RecipeTools.OnClick(openControls, zone, "OpenControls");
            RecipeTools.OnClick(menu, zone, "BackToMenu");
        }
    }

    public sealed class MenuRecipe : ServiceRecipe
    {
        public const string RecipeId = "menu";
        const string Script = "Scripts/MainMenu.cs";
        const string SceneFile = "MainMenu.unity";

        public override string Id => RecipeId;
        public override string DisplayName => "Main menu";
        public override string Description => "The first scene of the game: title and Play, Settings, Credits and Quit buttons, wired to the services you added. Adds the Gameplay scene too.";
        public override string Category => "Scenes";
        public override string FolderName => "Menu";
        public override string[] RequiredServices => new[] { BootstrapRecipe.RecipeId, LoadingRecipe.RecipeId, GameplayRecipe.RecipeId };
        public override bool HasTool => true;

        public override void Create(RecipeContext context) =>
            context.WriteTemplate(Script, RecipeContext.LoadTemplate(SceneRecipeText.Templates + "MainMenu.cs.txt"),
                RecipeRunner.CreatedFlags(SceneRecipeText.OptionalServices));

        public override void AfterScriptsCompiled(RecipeContext context)
        {
            Type menuType = RecipeTools.GeneratedType<MonoBehaviour>(context, "MainMenu", Script);
            Dictionary<string, string> created = RecipeRunner.CreatedFlags(SceneRecipeText.OptionalServices);
            RecipeTools.CreateScene(context, SceneFile, true, _ => Build(menuType, created["settings"] == "true", created["controls"] == "true", created["credits"] == "true"));
        }

        public override void OpenTool() => SceneRecipeText.OpenScene($"{RecipeRunner.RootFolder}/Menu/{SceneFile}");

        static void Build(Type menuType, bool settings, bool controls, bool credits)
        {
            RecipeTools.AddCamera(UIBuilder.Backdrop);
            GameObject canvas = UIBuilder.ScreenCanvas("Main Menu", 0, asPanel: false);
            UIBuilder.Stretch(UIBuilder.Image("Background", canvas.transform, UIBuilder.Window).rectTransform);

            string gameName = string.IsNullOrWhiteSpace(PlayerSettings.productName) ? "My Game" : PlayerSettings.productName;
            TextMeshProUGUI title = UIBuilder.Text("Title", canvas.transform, gameName, 110, style: FontStyles.Bold);
            title.color = UIBuilder.Accent;
            UIBuilder.Anchor(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(1600f, 180f), new Vector2(0f, -120f));

            RectTransform column = SceneRecipeText.CenteredColumn("Buttons", canvas.transform, new Vector2(0f, -80f));
            Button play = SceneRecipeText.MenuButton("Play", column, true);
            Button openSettings = settings ? SceneRecipeText.MenuButton("Settings", column, false) : null;
            Button openControls = controls ? SceneRecipeText.MenuButton("Controls", column, false) : null;
            Button openCredits = credits ? SceneRecipeText.MenuButton("Credits", column, false) : null;
            Button quit = SceneRecipeText.MenuButton("Quit", column, false);

            Component menu = canvas.AddComponent(menuType);
            RecipeTools.OnClick(play, menu, "Play");
            if (openSettings != null) RecipeTools.OnClick(openSettings, menu, "OpenSettings");
            if (openControls != null) RecipeTools.OnClick(openControls, menu, "OpenControls");
            if (openCredits != null) RecipeTools.OnClick(openCredits, menu, "OpenCredits");
            RecipeTools.OnClick(quit, menu, "Quit");
        }
    }
}
