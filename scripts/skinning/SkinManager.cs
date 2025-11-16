using System.IO;
using System.Text.RegularExpressions;
using Godot;
using System.Reflection;

[GlobalClass]
public partial class SkinManager : Node
{
    public static SkinManager Instance { get; private set; }

    public SkinProfile Skin { get; set; } = new SkinProfile();

    public override void _Ready()
    {
        Instance = this;
    }

	public static void Save()
	{
		var settings = SettingsManager.Instance.Settings;
        var skin = Instance.Skin;

		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/colors.txt", skin.RawColors);
		File.WriteAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/space.txt", skin.GameSpaceName);
		Logger.Log($"Saved skin {settings.Skin}");
	}

	public static void Load()
	{
		var settings = SettingsManager.Instance.Settings;
        var skin = Instance.Skin;

		skin.RawColors = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/colors.txt").TrimSuffix(",");

		string[] split = skin.RawColors.Split(",");
		Color[] colors = new Color[split.Length];

		for (int i = 0; i < split.Length; i++)
		{
			split[i] = split[i].TrimPrefix("#").Substr(0, 6);
			split[i] = new Regex("[^a-fA-F0-9$]").Replace(split[i], "f");
			colors[i] = Color.FromHtml(split[i]);
		}

		skin.Colors = colors;

		foreach (PropertyInfo property in typeof(SkinProfile).GetProperties())
		{
			if (!property.Name.Contains("Image"))
			{
				continue;
			}

			property.SetValue(skin, ImageTexture.CreateFromImage(Image.LoadFromFile($"{Constants.USER_FOLDER}/skins/{settings.Skin}/{property.Name.TrimSuffix("Image").ToSnakeCase()}.png")));
		}

		skin.GameSpaceName = File.ReadAllText($"{Constants.USER_FOLDER}/skins/{settings.Skin}/space.txt");
        bool exists = Godot.FileAccess.FileExists($"res://prefabs/spaces/{skin.GameSpaceName}.tscn");
        skin.GameSpace = GD.Load<PackedScene>($"res://prefabs/spaces/{(exists ? skin.GameSpaceName : "void")}.tscn").Instantiate<Node3D>();

        skin.MenuSpaceName = "waves";
		exists = Godot.FileAccess.FileExists($"res://prefabs/spaces/{skin.MenuSpaceName}.tscn");
		skin.MenuSpace = GD.Load<PackedScene>($"res://prefabs/spaces/{(exists ? skin.MenuSpaceName : "void")}.tscn").Instantiate<Node3D>();

        if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/note.obj"))
		{
			skin.NoteMesh = (ArrayMesh)Util.OBJParser.Call("load_obj", $"{Constants.USER_FOLDER}/skins/{settings.Skin}/note.obj");
		}
		else
		{
			skin.NoteMesh = GD.Load<ArrayMesh>($"res://skin/note.obj");
		}

		if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/hit.mp3"))
		{
			Godot.FileAccess file = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/skins/{settings.Skin}/hit.mp3", Godot.FileAccess.ModeFlags.Read);
			skin.HitSoundBuffer = file.GetBuffer((long)file.GetLength());
			file.Close();
		}

		if (File.Exists($"{Constants.USER_FOLDER}/skins/{settings.Skin}/fail.mp3"))
		{
			Godot.FileAccess file = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/skins/{settings.Skin}/fail.mp3", Godot.FileAccess.ModeFlags.Read);
			skin.FailSoundBuffer = file.GetBuffer((long)file.GetLength());
			file.Close();
		}

		ToastNotification.Notify($"Loaded skin [{settings.Skin}]");
		Logger.Log($"Loaded skin {settings.Skin}");
	}
}
