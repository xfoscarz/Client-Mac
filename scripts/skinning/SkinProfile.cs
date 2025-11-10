using System.IO;
using System.Text.RegularExpressions;
using Godot;
using System.Reflection;

public class SkinProfile
{
	public static Color[] Colors { get; set; } = [Color.FromHtml("#00ffed"), Color.FromHtml("#ff8ff9")];
	public static string RawColors { get; set; } = "00ffed,ff8ff9";
	public static ImageTexture CursorImage { get; set; } = new();
	public static ImageTexture GridImage { get; set; } = new();
	public static ImageTexture PanelLeftBackgroundImage { get; set; } = new();
	public static ImageTexture PanelRightBackgroundImage { get; set; } = new();
	public static ImageTexture HealthImage { get; set; } = new();
	public static ImageTexture HealthBackgroundImage { get; set; } = new();
	public static ImageTexture ProgressImage { get; set; } = new();
	public static ImageTexture ProgressBackgroundImage { get; set; } = new();
	public static ImageTexture HitsImage { get; set; } = new();
	public static ImageTexture MissesImage { get; set; } = new();
	public static ImageTexture MissFeedbackImage { get; set; } = new();
	public static ImageTexture JukeboxPlayImage { get; set; } = new();
	public static ImageTexture JukeboxPauseImage { get; set; } = new();
	public static ImageTexture JukeboxSkipImage { get; set; } = new();
	public static ImageTexture FavoriteImage { get; set; } = new();
	public static ImageTexture ModNofailImage { get; set; } = new();
	public static ImageTexture ModSpinImage { get; set; } = new();
	public static ImageTexture ModGhostImage { get; set; } = new();
	public static ImageTexture ModChaosImage { get; set; } = new();
	public static ImageTexture ModFlashlightImage { get; set; } = new();
	public static ImageTexture ModHardrockImage { get; set; } = new();
	public static byte[] HitSoundBuffer { get; set; } = [];
	public static byte[] FailSoundBuffer { get; set; } = [];
	public static ArrayMesh NoteMesh { get; set; } = new();
    public static string MenuSpaceName = "waves";
    public static string GameSpaceName = "grid";
    public static Node3D MenuSpace { get; set; }
	public static Node3D GameSpace { get; set; }

	public static void Save()
	{
		var settings = SettingsManager.Settings;

		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/colors.txt", RawColors);
		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/space.txt", GameSpaceName);
		Logger.Log($"Saved skin {settings.Skin}");
	}

	public static void Load()
	{
		var settings = SettingsManager.Settings;

		RawColors = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/colors.txt").TrimSuffix(",");

		string[] split = RawColors.Split(",");
		Color[] colors = new Color[split.Length];

		for (int i = 0; i < split.Length; i++)
		{
			split[i] = split[i].TrimPrefix("#").Substr(0, 6);
			split[i] = new Regex("[^a-fA-F0-9$]").Replace(split[i], "f");
			colors[i] = Color.FromHtml(split[i]);
		}

		Colors = colors;

		foreach (PropertyInfo property in typeof(SkinProfile).GetProperties())
		{
			if (!property.Name.Contains("Image"))
			{
				continue;
			}

			property.SetValue(null, ImageTexture.CreateFromImage(Image.LoadFromFile($"{Constants.USER_FOLDER}/skins/{settings.Skin}/{property.Name.TrimSuffix("Image").ToSnakeCase()}.png")));
		}

		GameSpaceName = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/space.txt");
        bool exists = Godot.FileAccess.FileExists($"res://prefabs/spaces/{GameSpaceName}.tscn");
        GameSpace = GD.Load<PackedScene>($"res://prefabs/spaces/{(exists ? GameSpaceName : "void")}.tscn").Instantiate<Node3D>();

        MenuSpaceName = "waves";
		exists = Godot.FileAccess.FileExists($"res://prefabs/spaces/{MenuSpaceName}.tscn");
		MenuSpace = GD.Load<PackedScene>($"res://prefabs/spaces/{(exists ? MenuSpaceName : "void")}.tscn").Instantiate<Node3D>();

        if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/note.obj"))
		{
			NoteMesh = (ArrayMesh)Util.OBJParser.Call("load_obj", $"{Constants.USER_FOLDER}/skins/{settings.Skin}/note.obj");
		}
		else
		{
			NoteMesh = GD.Load<ArrayMesh>($"res://skin/note.obj");
		}

		if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/hit.mp3"))
		{
			Godot.FileAccess file = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/skins/{settings.Skin}/hit.mp3", Godot.FileAccess.ModeFlags.Read);
			HitSoundBuffer = file.GetBuffer((long)file.GetLength());
			file.Close();
		}

		if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/fail.mp3"))
		{
			Godot.FileAccess file = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/skins/{settings.Skin}/fail.mp3", Godot.FileAccess.ModeFlags.Read);
			FailSoundBuffer = file.GetBuffer((long)file.GetLength());
			file.Close();
		}

		ToastNotification.Notify($"Loaded skin [{settings.Skin}]");
		Logger.Log($"Loaded skin {settings.Skin}");
	}
}
