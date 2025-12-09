using System.IO;
using Godot;

[GlobalClass]
public partial class Constants : Node
{
    public static readonly ulong STARTED = Time.GetTicksUsec();

	public static readonly string ROOT_FOLDER = Directory.GetCurrentDirectory();
	
	public static readonly string USER_FOLDER = OS.GetUserDataDir();

	public static readonly bool TEMP_MAP_MODE = false;//OS.GetCmdlineArgs().Length > 0;

	public static readonly double CURSOR_SIZE = 0.2625;

	public static readonly double GRID_SIZE = 3.0;

	public static readonly Vector2 BOUNDS = new((float)(GRID_SIZE / 2 - CURSOR_SIZE / 2), (float)(GRID_SIZE / 2 - CURSOR_SIZE / 2));

	public static readonly double HIT_BOX_SIZE = 0.07;

	public static readonly double HIT_WINDOW = 55;

	public static readonly int BREAK_TIME = 4000;  // used for skipping breaks mid-map

	public static readonly string[] DIFFICULTIES = ["N/A", "Easy", "Medium", "Hard", "Expert", "Insane"];

	public static readonly Color[] DIFFICULTY_COLORS = [Color.FromHtml("ffffff"), Color.FromHtml("00ff00"), Color.FromHtml("ffff00"), Color.FromHtml("ff0000"), Color.FromHtml("7f00ff"), Color.FromHtml("007fff")];

	public static readonly Color[] SECONDARY_DIFFICULTY_COLORS = [Color.FromHtml("808080"), Color.FromHtml("7fff7f"), Color.FromHtml("ffff7f"), Color.FromHtml("ff007f"), Color.FromHtml("ff00ff"), Color.FromHtml("007fff")];

	public static readonly Godot.Collections.Dictionary<string, double> MODS_MULTIPLIER_INCREMENT = new()
	{
		["NoFail"] = 0,
		["Ghost"] = 0.0675,
		["Spin"] = 0.18,
		["Flashlight"] = 0.1,
		["Chaos"] = 0.07,
		["HardRock"] = 0.08
	};
}
