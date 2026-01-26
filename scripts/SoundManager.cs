using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Menu;

public partial class SoundManager : Node, ISkinnable
{
    public static SoundManager Instance;

    public static AudioStreamPlayer HitSound;
    public static AudioStreamPlayer MissSound;
    public static AudioStreamPlayer FailSound;
    public static AudioStreamPlayer Song;

    [Signal]
    public delegate void JukeboxPlayedEventHandler(Map map);

    public static string[] JukeboxQueue = [];
    public static Dictionary<string, int> JukeboxQueueInverse = [];
    public static int JukeboxIndex = 0;
    public static bool JukeboxPaused = false;
    public static ulong LastRewind = 0;
    public static Map Map;

    public override void _Ready()
    {
        Instance = this;

        HitSound = new();
        MissSound = new();
        FailSound = new();
        Song = new();

        HitSound.MaxPolyphony = 16;

        AddChild(HitSound);
        AddChild(MissSound);
        AddChild(FailSound);
        AddChild(Song);

        SkinManager.Instance.Loaded += UpdateSkin;
        
        UpdateSkin(SkinManager.Instance.Skin);

        Song.Finished += () => {
            switch (SceneManager.Scene.Name)
            {
                case "SceneMenu":
                    if (SettingsManager.Instance.Settings.AutoplayJukebox)
                    {
                        JukeboxIndex++;
                        PlayJukebox(JukeboxIndex);
                    }
                    break;
                case "SceneResults":
                    PlayJukebox(JukeboxIndex);  // play skinnable results song here in the future
                    break;
                default:
                    break;
            }
        };

        SettingsManager.Instance.Loaded += UpdateVolume;
        Lobby.Instance.SpeedChanged += (speed) => { SoundManager.Song.PitchScale = (float)speed; };

        UpdateVolume();
        UpdateJukeboxQueue();

        if (SettingsManager.Instance.Settings.AutoplayJukebox)
        {
            PlayJukebox();
        }
    }

    public void UpdateSkin(SkinProfile skin)
    {
        HitSound.Stream = Util.Audio.LoadStream(skin.HitSoundBuffer);
        FailSound.Stream = Util.Audio.LoadStream(skin.FailSoundBuffer);
    }

    public static void PlayJukebox(int index = -1, bool setRichPresence = true)
    {
        if (JukeboxQueue.Length == 0)
        {
            return;
        }

        index = index == -1 ? JukeboxIndex : index;

        if (index >= JukeboxQueue.Length)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = JukeboxQueue.Length - 1;
        }

        Map = MapParser.Decode(JukeboxQueue[index]);

        if (Map.AudioBuffer == null)
        {
            JukeboxIndex++;
            PlayJukebox(JukeboxIndex);
            return;
        }

        Instance.EmitSignal(SignalName.JukeboxPlayed, Map);

        Song.Stream = Util.Audio.LoadStream(Map.AudioBuffer);
        Song.Play();

        if (setRichPresence)
        {
            Discord.Client.UpdateState($"Listening to {Map.PrettyTitle}");
        }
    }

    public static void PlayJukebox(Map map, bool setRichPresence = true)
    {
        if (JukeboxQueueInverse.TryGetValue(map.FilePath.GetFile().GetBaseName(), out int index))
        {
            PlayJukebox(index, setRichPresence);
        }
    }

    public static void UpdateVolume()
    {
        var settings = SettingsManager.Instance.Settings;

        Song.VolumeDb = -80 + 70 * (float)Math.Pow(settings.VolumeMusic.Value / 100, 0.1) * (float)Math.Pow(settings.VolumeMaster.Value / 100, 0.1);
        HitSound.VolumeDb = -80 + 80 * (float)Math.Pow(settings.VolumeSFX.Value / 100, 0.1) * (float)Math.Pow(settings.VolumeMaster.Value / 100, 0.1);
        FailSound.VolumeDb = -80 + 80 * (float)Math.Pow(settings.VolumeSFX.Value / 100, 0.1) * (float)Math.Pow(settings.VolumeMaster.Value / 100, 0.1);
    }

    public static void UpdateJukeboxQueue()
    {
        JukeboxQueue = [.. Directory.GetFiles($"{Constants.USER_FOLDER}/maps").Shuffle()];

        for (int i = 0; i < JukeboxQueue.Length; i++)
        {
            JukeboxQueueInverse[JukeboxQueue[i].GetFile().GetBaseName()] = i;
        }
    }
}
