using System;
using System.Collections.Generic;
using System.IO;
using Godot;

public partial class Phoenyx : Node
{
    private static bool initialized = false;
    private static bool loaded = false;

    public static bool Quitting { get; private set; } = false;

    public override void _Ready()
    {
        Setup();
    }

    public static void Setup()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (!File.Exists($"{Constants.USER_FOLDER}/favorites.txt"))
        {
            File.WriteAllText($"{Constants.USER_FOLDER}/favorites.txt", "");
        }

        if (!Directory.Exists($"{Constants.USER_FOLDER}/cache"))
        {
            Directory.CreateDirectory($"{Constants.USER_FOLDER}/cache");
        }

        if (!Directory.Exists($"{Constants.USER_FOLDER}/cache/maps"))
        {
            Directory.CreateDirectory($"{Constants.USER_FOLDER}/cache/maps");
        }

        foreach (string cacheFile in Directory.GetFiles($"{Constants.USER_FOLDER}/cache"))
        {
            File.Delete(cacheFile);
        }

        for (int i = 0; i < Util.UserDirectories.Length; i++)
        {
            string Folder = Util.UserDirectories[i];

            if (!Directory.Exists($"{Constants.USER_FOLDER}/{Folder}"))
            {
                Directory.CreateDirectory($"{Constants.USER_FOLDER}/{Folder}");
            }
        }

        if (!Directory.Exists($"{Constants.USER_FOLDER}/skins/default"))
        {
            Directory.CreateDirectory($"{Constants.USER_FOLDER}/skins/default");
        }

        foreach (string skinFile in Util.SkinFiles)
        {
            try
            {
                byte[] buffer = [];

                if (skinFile.GetExtension() == "txt")
                {
                    Godot.FileAccess file = Godot.FileAccess.Open($"res://skin/{skinFile}", Godot.FileAccess.ModeFlags.Read);
                    buffer = file.GetBuffer((long)file.GetLength());
                }
                else
                {
                    var source = ResourceLoader.Load($"res://skin/{skinFile}");

                    switch (source.GetType().Name)
                    {
                        case "CompressedTexture2D":
                            buffer = (source as CompressedTexture2D).GetImage().SavePngToBuffer();
                            break;
                        case "AudioStreamMP3":
                            buffer = (source as AudioStreamMP3).Data;
                            break;
                    }
                }

                if (buffer.Length == 0)
                {
                    continue;
                }

                Godot.FileAccess target = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/skins/default/{skinFile}", Godot.FileAccess.ModeFlags.Write);
                target.StoreBuffer(buffer);
                target.Close();
            }
            catch (Exception exception)
            {
                Logger.Log($"Couldn't copy default skin file {skinFile}; {exception}");
            }
        }

        if (!File.Exists($"{Constants.USER_FOLDER}/current_profile.txt"))
        {
            File.WriteAllText($"{Constants.USER_FOLDER}/current_profile.txt", "default");
        }

        if (!File.Exists($"{Constants.USER_FOLDER}/profiles/default.json"))
        {
            SettingsManager.Save("default");
        }

        try
        {
            SettingsManager.Load();
        }
        catch
        {
            SettingsManager.Save();
        }

        if (!File.Exists($"{Constants.USER_FOLDER}/stats"))
        {
            Logger.Log("Stats file not found");
            File.WriteAllText($"{Constants.USER_FOLDER}/stats", "");
            Stats.Save();
        }

        try
        {
            Stats.Load();
        }
        catch
        {
            Stats.GamePlaytime = 0;
            Stats.TotalPlaytime = 0;
            Stats.GamesOpened = 0;
            Stats.TotalDistance = 0;
            Stats.NotesHit = 0;
            Stats.NotesMissed = 0;
            Stats.HighestCombo = 0;
            Stats.Attempts = 0;
            Stats.Passes = 0;
            Stats.FullCombos = 0;
            Stats.HighestScore = 0;
            Stats.TotalScore = 0;
            Stats.RageQuits = 0;
            Stats.PassAccuracies = [];
            Stats.FavoriteMaps = [];

            Stats.Save();
        }

        SettingsManager.UpdateSettings();
        Stats.GamesOpened++;

        List<string> import = [];

        foreach (string file in Directory.GetFiles($"{Constants.USER_FOLDER}/maps"))
        {
            if (file.GetExtension() == "sspm" || file.GetExtension() == "txt")
            {
                import.Add(file);
            }
        }

        MapParser.BulkImport([.. import]);

        loaded = true;
    }

    public static void Quit()
    {
        var settings = SettingsManager.Instance.Settings;

        if (Quitting)
        {
            return;
        }

        Quitting = true;

        if (!LegacyRunner.CurrentAttempt.IsReplay)
        {
            LegacyRunner.CurrentAttempt.Stop();
        }

        Stats.TotalPlaytime += (Time.GetTicksUsec() - Constants.STARTED) / 1000000;

        if (loaded)
        {
            SettingsManager.Save();
            Stats.Save();
        }

        if (File.Exists($"{Constants.USER_FOLDER}/maps/NA_tempmap.phxm"))
        {
            File.Delete($"{Constants.USER_FOLDER}/maps/NA_tempmap.phxm");
        }

        Discord.Client.Dispose();

        if (SceneManager.Scene.Name == "SceneMenu")
        {
            Tween tween = SceneManager.Scene.CreateTween();
            tween.TweenProperty(SceneManager.Scene, "modulate", Color.Color8(1, 1, 1, 0), 0.5).SetTrans(Tween.TransitionType.Quad);
            tween.TweenCallback(Callable.From(() =>
            {
                SceneManager.Scene.GetTree().Quit();
            }));
            tween.Play();
        }
        else
        {
            SceneManager.Scene.GetTree().Quit();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (SceneManager.Scene.Name == "SceneGame")
            {
                Stats.RageQuits++;
            }

            Quit();
        }
    }


}
