using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Godot;
using Godot.NativeInterop;

public partial class Rhythia : Node
{
    private static bool loaded = false;

    [Signal]
    public delegate void FilesDroppedEventHandler(string[] files);

    public static Rhythia Instance;
    public static bool Quitting { get; private set; } = false;

    public override void _Ready()
    {
        Instance = this;

        GetTree().AutoAcceptQuit = false;

        // Set up user folder
        
        static void deepCopy(string resDir = "")
        {
            string userDir = $"{Constants.USER_FOLDER}{resDir}";

            if (!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }

            foreach (string resFile in Godot.DirAccess.GetFilesAt($"res://user{resDir}"))
            {
                string userFile = $"{userDir}/{resFile}";
                string ext = resFile.GetExtension();

                if (File.Exists(userFile) || ext == "import" || ext == "uid" || ext == "gitkeep")
                {
                    continue;
                }

                Godot.FileAccess source = Godot.FileAccess.Open($"res://user{resDir}/{resFile}", Godot.FileAccess.ModeFlags.Read);
                byte[] buffer = source.GetBuffer((long)source.GetLength());
                source.Close();

                Godot.FileAccess copy = Godot.FileAccess.Open(userFile, Godot.FileAccess.ModeFlags.Write);
                copy.StoreBuffer(buffer);
                copy.Close();
            }

            foreach (string dir in Godot.DirAccess.GetDirectoriesAt($"res://user{resDir}"))
            {
                deepCopy($"{resDir}/{dir}");
            }
        }

        deepCopy();

        // Settings

        if (!File.Exists($"{Constants.USER_FOLDER}/profiles/default.json"))
        {
            SettingsManager.Save("default");
        }

        try
        {
            SettingsManager.Load();
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            SettingsManager.Save();
        }

        // Stats

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

        Stats.GamesOpened++;

        List<string> import = [];

        foreach (string file in Directory.GetFiles($"{Constants.USER_FOLDER}/maps", $"*.{Constants.DEFAULT_MAP_EXT}", SearchOption.AllDirectories))
        {
            string ext = file.GetExtension();

            if (!MapParser.IsValidExt(ext))
            {
                File.Delete(file);
            }
            else if (ext != Constants.DEFAULT_MAP_EXT)
            {
                import.Add(file);
            }
        }

        MapParser.BulkImport([.. import]);

        foreach (string file in import)
        {
            File.Delete(file);
        }

        GetViewport().Connect("files_dropped", Callable.From((string[] files) => {
            EmitSignal(SignalName.FilesDropped, files);

            List<string> maps = [];
            List<Replay> replays = [];

            foreach (string file in files)
            {
                string ext = file.GetExtension();

                if (MapParser.IsValidExt(ext))
                {
                    maps.Add(file);
                }
                else
                {
                    switch (ext)
                    {
                        case "phxr":
                            Replay replay = new(file);

                            if (!replay.Valid)
                            {
                                continue;
                            }

                            replays.Add(replay);
                            break;
                    }
                }
            }
            
            if (maps.Count > 0)
            {
                MapParser.BulkImport([.. maps]);
            
                if (SceneManager.Scene is MainMenu)
                {
                    var menu = SceneManager.Scene as MainMenu;
                    menu.Transition(menu.PlayMenu);
                }
            }

            if (replays.Count > 0)
            {
                List<Replay> matching = [];

                foreach (Replay replay in replays)
                {
                    if (replay == replays[0])
                    {
                        matching.Add(replay);
                    }
                }

                LegacyRunner.Play(MapParser.Decode(matching[0].MapFilePath), matching[0].Speed, matching[0].StartFrom, matching[0].Modifiers, null, [.. matching]);
            }
        }));

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

        Discord.Client.Dispose();
        
        Instance.GetTree().Quit();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (SceneManager.Scene != null && SceneManager.Scene is LegacyRunner)
            {
                Stats.RageQuits++;
            }

            Quit();
        }
    }
}
