using System;
using System.IO;
using Godot;
using System.Collections.Generic;

public class Util
{
    public static string[] UserDirectories = ["maps", "profiles", "skins", "replays", "pbs"];
    public static string[] SkinFiles = ["cursor.png", "grid.png", "health.png", "hits.png", "misses.png", "miss_feedback.png", "health_background.png", "progress.png", "progress_background.png", "panel_left_background.png", "panel_right_background.png", "jukebox_play.png", "jukebox_pause.png", "jukebox_skip.png", "favorite.png", "mod_nofail.png", "mod_spin.png", "mod_ghost.png", "mod_chaos.png", "mod_flashlight.png", "mod_hardrock.png", "hit.mp3", "fail.mp3", "colors.txt"];

    public static GodotObject OBJParser = (GodotObject)GD.Load<GDScript>("res://scripts/OBJParser.gd").New();

    public static string GetProfile()
    {
        return File.ReadAllText($"{Constants.USER_FOLDER}/current_profile.txt");
    }

    public static ImageTexture GetModIcon(string mod)
    {
        ImageTexture tex = new();

        switch (mod)
        {
            case "NoFail":
                tex = SkinProfile.ModNofailImage;
                break;
            case "Spin":
                tex = SkinProfile.ModSpinImage;
                break;
            case "Ghost":
                tex = SkinProfile.ModGhostImage;
                break;
            case "Chaos":
                tex = SkinProfile.ModChaosImage;
                break;
            case "Flashlight":
                tex = SkinProfile.ModFlashlightImage;
                break;
            case "HardRock":
                tex = SkinProfile.ModHardrockImage;
                break;
        }

        return tex;
    }
}
