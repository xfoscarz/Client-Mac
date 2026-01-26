using System.IO;
using System.Text.RegularExpressions;
using Godot;
using System.Reflection;
using Godot.NativeInterop;

[GlobalClass]
public partial class SkinManager : Node
{
    public static SkinManager Instance { get; private set; }

	[Signal]
	public delegate void SavedEventHandler();

	[Signal]
	public delegate void LoadedEventHandler(SkinProfile skin);

    public SkinProfile Skin { get; set; } = new SkinProfile();

    public override void _Ready()
    {
        Instance = this;
    }

	public static void Save()
	{
		var settings = SettingsManager.Instance.Settings;
        var skin = Instance.Skin;

		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/colors.txt", skin.RawColors);
		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/space.txt", skin.GameSpaceName);
		Logger.Log($"Saved skin {settings.Skin.Value}");

		Instance.EmitSignal(SignalName.Saved);
	}

	public static void Load()
	{
        var settings = SettingsManager.Instance.Settings;
        var skin = Instance.Skin;

		// Colors

		skin.RawColors = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/colors.txt").TrimSuffix(",");

		string[] split = skin.RawColors.Split(",");
		Color[] colors = new Color[split.Length];

		for (int i = 0; i < split.Length; i++)
		{
			split[i] = split[i].TrimPrefix("#").Substr(0, 6);
			split[i] = new Regex("[^a-fA-F0-9$]").Replace(split[i], "f");
			colors[i] = Color.FromHtml(split[i]);
		}

		skin.Colors = colors;

		/////
        
        // Textures

        skin.CursorImage = loadTexture("game/cursor.png");
        skin.GridImage = loadTexture("game/grid.png");
        skin.PanelLeftBackgroundImage = loadTexture("game/panel_left_background.png");
		skin.PanelRightBackgroundImage = loadTexture("game/panel_right_background.png");
        skin.HealthImage = loadTexture("game/health.png");
		skin.HealthBackgroundImage = loadTexture("game/health_background.png");
		skin.ProgressImage = loadTexture("game/progress.png");
		skin.ProgressBackgroundImage = loadTexture("game/progress_background.png");
		skin.HitsImage = loadTexture("game/hits.png");
		skin.MissesImage = loadTexture("game/misses.png");
		skin.MissFeedbackImage = loadTexture("game/miss_feedback.png");

		skin.SettingsButtonImage = loadTexture("ui/buttons/settings.png");
		skin.OpenFolderButtonImage = loadTexture("ui/buttons/open_folder.png");
		skin.ImportButtonImage = loadTexture("ui/buttons/import.png");
		skin.RandomButtonImage = loadTexture("ui/buttons/random.png");
		skin.FilterButtonImage = loadTexture("ui/buttons/filter.png");
		skin.SortButtonImage = loadTexture("ui/buttons/sort.png");
        skin.AuthorButtonImage = loadTexture("ui/buttons/author.png");
		skin.SearchButtonImage = loadTexture("ui/buttons/search.png");
        skin.LayoutListButtonImage = loadTexture("ui/buttons/layout_list.png");
		skin.LayoutGridButtonImage = loadTexture("ui/buttons/layout_grid.png");

		skin.SpeedPresetMinusMinusButtonImage = loadTexture("ui/buttons/speed_minus_minus.png");
        skin.SpeedPresetMinusButtonImage = loadTexture("ui/buttons/speed_minus.png");
        skin.SpeedPresetMiddleButtonImage = loadTexture("ui/buttons/speed_middle.png");
        skin.SpeedPresetPlusButtonImage = loadTexture("ui/buttons/speed_plus.png");
		skin.SpeedPresetPlusPlusButtonImage = loadTexture("ui/buttons/speed_plus_plus.png");

        skin.PlayButtonImage = loadTexture("ui/buttons/play.png");
		skin.FavoriteButtonImage = loadTexture("ui/buttons/favorite.png");
		skin.CopyButtonImage = loadTexture("ui/buttons/copy.png");
		skin.DeleteButtonImage = loadTexture("ui/buttons/delete.png");
		skin.AddVideoButtonImage = loadTexture("ui/buttons/add_video.png");
		skin.RemoveVideoButtonImage = loadTexture("ui/buttons/remove_video.png");

        skin.GrabberNormalImage = loadTexture("ui/buttons/grabber_normal.png");
		skin.GrabberPressedImage = loadTexture("ui/buttons/grabber_pressed.png");
		skin.GrabberTickImage = loadTexture("ui/buttons/grabber_tick.png");

        skin.JukeboxPlayImage = loadTexture("ui/jukebox_play.png");
		skin.JukeboxPauseImage = loadTexture("ui/jukebox_pause.png");
		skin.JukeboxSkipImage = loadTexture("ui/jukebox_skip.png");
        skin.BackgroundTileImage = loadTexture("ui/background_tile.png");

		skin.FavoriteImage = loadTexture("ui/play/favorite.png");
		skin.MapListMaskImage = loadTexture("ui/play/maplist_mask.png");
		skin.MapListSelectionCursorImage = loadTexture("ui/play/maplist_selection_cursor.png");
        skin.MapListScrollBarTopImage = loadTexture("ui/play/scrollbar_top.png");
		skin.MapListScrollBarMiddleImage = loadTexture("ui/play/scrollbar_middle.png");
		skin.MapListScrollBarBottomImage = loadTexture("ui/play/scrollbar_bottom.png");
		skin.MapListScrollBarBackgroundTopImage = loadTexture("ui/play/scrollbar_background_top.png");
		skin.MapListScrollBarBackgroundMiddleImage = loadTexture("ui/play/scrollbar_background_middle.png");
		skin.MapListScrollBarBackgroundBottomImage = loadTexture("ui/play/scrollbar_background_bottom.png");
        skin.MapListGridCoverBackgroundImage = loadTexture("ui/play/grid_cover_background.png");

        skin.MapInfoCoverBackgroundImage = loadTexture("ui/play/mapinfo_cover_background.png");

        skin.ModNofailImage = loadTexture("modifiers/nofail.png");
		skin.ModSpinImage = loadTexture("modifiers/spin.png");
		skin.ModGhostImage = loadTexture("modifiers/ghost.png");
		skin.ModChaosImage = loadTexture("modifiers/chaos.png");
		skin.ModFlashlightImage = loadTexture("modifiers/flashlight.png");
		skin.ModHardrockImage = loadTexture("modifiers/hardrock.png");
		
        // Sounds

        skin.HitSoundBuffer = loadSound("hit.mp3");
        skin.FailSoundBuffer = loadSound("fail.mp3");

        // Meshes

        skin.NoteMesh = loadMesh("note.obj");

        // Spaces

        // skin.GameSpaceName = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/space.txt");
        skin.GameSpaceName = "grid";
        skin.GameSpace = loadSpace($"res://prefabs/spaces/{skin.GameSpaceName}.tscn");

        skin.MenuSpaceName = "waves";
		skin.MenuSpace = loadSpace($"res://prefabs/spaces/{skin.MenuSpaceName}.tscn");

		// Shaders

		skin.BackgroundTileShader = loadShader("ui/background_tile.gdshader");
        skin.MapButtonCoverShader = loadShader("ui/play/map_button_cover.gdshader");

        /////

        ToastNotification.Notify($"Loaded skin [{settings.Skin.Value}]");
		Logger.Log($"Loaded skin {settings.Skin.Value}");

		Instance.EmitSignal(SignalName.Loaded, skin);
	}

	public static void Reload()
	{
        Save();
        Load();
    }

	private static ImageTexture loadTexture(string skinPath)
	{
		var settings = SettingsManager.Instance.Settings;
		return ImageTexture.CreateFromImage(Image.LoadFromFile($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/{skinPath}"));
	}

	private static byte[] loadSound(string skinPath)
	{
		var settings = SettingsManager.Instance.Settings;
		string path = $"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/{skinPath}";
		byte[] buffer = [];

		if (File.Exists(path))
		{
			Godot.FileAccess file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
			buffer = file.GetBuffer((long)file.GetLength());
			file.Close();
		}

		return buffer;
	}

	private static ArrayMesh loadMesh(string skinPath)
	{
		var settings = SettingsManager.Instance.Settings;
		if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/{skinPath}"))
		{
			return (ArrayMesh)Util.Misc.OBJParser.Call("load_obj", $"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/{skinPath}");
		}
		else
		{
			return GD.Load<ArrayMesh>($"res://skin/note.obj");
		}
	}

	private static Shader loadShader(string skinPath)
	{
		var settings = SettingsManager.Instance.Settings;
        string shader = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin.Value}/{skinPath}");

        return new() { Code = shader };
    }

	private static BaseSpace loadSpace(string path)
	{
		var settings = SettingsManager.Instance.Settings;
		bool exists = Godot.FileAccess.FileExists(path);
		
        return GD.Load<PackedScene>(exists ? path : "res://prefabs/spaces/void.tscn").Instantiate<Node3D>() as BaseSpace;
	}
}
