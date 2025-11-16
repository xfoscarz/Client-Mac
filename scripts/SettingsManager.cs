using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SettingsManager : Control
{
    public static Control Control;

    public static ColorRect SettingsNode;
    public static Panel Holder;
    public static bool Shown = false;
    public static LineEdit FocusedLineEdit = null;

    public static SettingsManager Instance { get; private set; }

    public SettingsProfile Settings { get; private set; } = new SettingsProfile();

    public override void _Ready()
    {
        Control = this;
        Instance = this;

        SettingsNode = GD.Load<PackedScene>("res://prefabs//settings.tscn").Instantiate<ColorRect>();
        Holder = SettingsNode.GetNode<Panel>("Holder");
        SettingsNode.GetNode<Button>("Deselect").Pressed += HideSettings;

        AddChild(SettingsNode);
        HideSettings();
        GetViewport().SizeChanged += () =>
        {
            SettingsNode.SetSize(DisplayServer.WindowGetSize());
        };

        SettingsNode.SetSize(DisplayServer.WindowGetSize());

        foreach (Node holder in Holder.GetNode("Sidebar").GetNode("Container").GetChildren())
        {
            holder.GetNode<Button>("Button").Pressed += () =>
            {
                foreach (ColorRect otherHolder in Holder.GetNode("Sidebar").GetNode("Container").GetChildren())
                {
                    otherHolder.Color = Color.FromHtml($"#ffffff{(holder.Name == otherHolder.Name ? "08" : "00")}");
                }

                foreach (ScrollContainer category in Holder.GetNode("Categories").GetChildren())
                {
                    category.Visible = category.Name == holder.Name;
                }
            };
        }

        OptionButton profiles = Holder.GetNode("Header").GetNode<OptionButton>("Profiles");
        LineEdit profileEdit = Holder.GetNode("Header").GetNode<LineEdit>("ProfileEdit");
        OptionButton skins = Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Skin").GetNode<OptionButton>("OptionsButton");
        OptionButton spaces = Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Space").GetNode<OptionButton>("OptionsButton");

        Holder.GetNode("Header").GetNode<Button>("CreateProfile").Pressed += () =>
        {
            profileEdit.Visible = !profileEdit.Visible;
        };
        profileEdit.FocusEntered += () => FocusedLineEdit = profileEdit;
        profileEdit.FocusExited += () => FocusedLineEdit = null;
        profileEdit.TextSubmitted += (string text) =>
        {
            text = new Regex("[^a-zA-Z0-9_ -]").Replace(text.Replace(" ", "_"), "");

            profileEdit.ReleaseFocus();
            profileEdit.Visible = false;

            if (File.Exists($"{Constants.USER_FOLDER}/profiles/{text}.json"))
            {
                ToastNotification.Notify($"Profile {text} already exists!");
                return;
            }

            File.WriteAllText($"{Constants.USER_FOLDER}/profiles/{text}.json", File.ReadAllText($"{Constants.USER_FOLDER}/profiles/default.json"));
            UpdateSettings();
        };
        profiles.Pressed += ShowMouse;
        profiles.ItemSelected += (long item) =>
        {
            string profile = profiles.GetItemText((int)item);

            HideMouse();
            Save();
            File.WriteAllText($"{Constants.USER_FOLDER}/current_profile.txt", profile);
            Load(profile);
            UpdateSettings();
        };

        skins.Pressed += ShowMouse;
        skins.ItemSelected += (long item) =>
        {
            HideMouse();
            Instance.Settings.Skin = skins.GetItemText((int)item);
            SkinManager.Load();

            if (SceneManager.Scene.Name == "SceneMenu")
            {
                Menu.MainMenu.Cursor.Texture = SkinManager.Instance.Skin.CursorImage;
            }

            Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Colors").GetNode<LineEdit>("LineEdit").Text = SkinManager.Instance.Skin.RawColors;
        };

        spaces.Pressed += ShowMouse;
        spaces.ItemSelected += (long item) =>
        {
            HideMouse();
            Instance.Settings.Space = spaces.GetItemText((int)item);
        };

        Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Skin").GetNode<Button>("SkinFolder").Pressed += () =>
        {
            OS.ShellOpen($"{Constants.USER_FOLDER}/skins/{Instance.Settings.Skin}");
        };
        Holder.GetNode("Categories").GetNode("Other").GetNode("Container").GetNode("RhythiaImport").GetNode<Button>("Button").Pressed += () =>
        {
            if (!Directory.Exists($"{OS.GetDataDir()}/SoundSpacePlus") || !File.Exists($"{OS.GetDataDir()}/SoundSpacePlus/settings.json"))
            {
                ToastNotification.Notify("Could not locate Rhythia settings", 1);
                return;
            }

            Godot.FileAccess file = Godot.FileAccess.Open($"{OS.GetDataDir()}/SoundSpacePlus/settings.json", Godot.FileAccess.ModeFlags.Read);
            Godot.Collections.Dictionary data = (Godot.Collections.Dictionary)Json.ParseString(file.GetAsText());

            Instance.Settings.ApproachRate = (float)data["approach_rate"];
            Instance.Settings.ApproachDistance = (float)data["spawn_distance"];
            Instance.Settings.FoV = (float)data["fov"];
            Instance.Settings.Sensitivity = (float)data["sensitivity"] * 2;
            Instance.Settings.Parallax = (float)data["parallax"] / 50;
            Instance.Settings.FadeIn = (float)data["fade_length"] * 100;
            Instance.Settings.FadeOut = (bool)data["half_ghost"];
            Instance.Settings.Pushback = (bool)data["do_note_pushback"];
            Instance.Settings.NoteSize = (float)data["note_size"] * 0.875f;
            Instance.Settings.CursorScale = (float)data["cursor_scale"];
            Instance.Settings.CursorTrail = (bool)data["cursor_trail"];
            Instance.Settings.TrailTime = (float)data["trail_time"];
            Instance.Settings.SimpleHUD = (bool)data["simple_hud"];
            Instance.Settings.AbsoluteInput = (bool)data["absolute_mode"];
            Instance.Settings.FPS = (double)data["fps"];
            Instance.Settings.UnlockFPS = (bool)data["unlock_fps"];

            UpdateSettings();

            ToastNotification.Notify("Successfully imported Rhythia settings");
        };

        UpdateSettings(true);
    }

    public static void ShowSettings(bool show = true)
    {
        Shown = show;
        SettingsNode.GetNode<Button>("Deselect").MouseFilter = Shown ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        Control.CallDeferred("move_to_front");

        if (Shown)
        {
            SettingsNode.Visible = true;
        }

        Tween tween = SettingsNode.CreateTween();
        tween.TweenProperty(SettingsNode, "modulate", Color.Color8(255, 255, 255, (byte)(Shown ? 255 : 0)), 0.25).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(Holder, "offset_top", Shown ? 0 : 25, 0.25).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(Holder, "offset_bottom", Shown ? 0 : 25, 0.25).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.TweenCallback(Callable.From(() =>
        {
            SettingsNode.Visible = Shown;
        }));
        tween.Play();
    }

    public static void HideSettings()
    {
        ShowSettings(false);
    }

    public static void ApplySetting(string setting, object value)
    {
        switch (setting)
        {
            case "Sensitivity":
                Instance.Settings.Sensitivity = (double)value;
                break;
            case "ApproachRate":
                Instance.Settings.ApproachRate = (double)value;
                break;
            case "ApproachDistance":
                Instance.Settings.ApproachDistance = (double)value;
                break;
            case "FadeIn":
                Instance.Settings.FadeIn = (double)value;
                break;
            case "Parallax":
                Instance.Settings.Parallax = (double)value;
                break;
            case "FoV":
                Instance.Settings.FoV = (double)value;
                break;
            case "VolumeMaster":
                Instance.Settings.VolumeMaster = (double)value;
                break;
            case "VolumeMusic":
                Instance.Settings.VolumeMusic = (double)value;
                break;
            case "VolumeSFX":
                Instance.Settings.VolumeSFX = (double)value;
                break;
            case "AlwaysPlayHitSound":
                Instance.Settings.AlwaysPlayHitSound = (bool)value;
                break;
            case "NoteSize":
                Instance.Settings.NoteSize = (double)value;
                break;
            case "CursorScale":
                Instance.Settings.CursorScale = (double)value;

                //if (SceneManager.Scene.Name == "SceneMenu" && Menu.MainMenu.Cursor != null)
                //{
                //    Menu.MainMenu.Cursor.Size = new Vector2(32 * (float)Settings.CursorScale, 32 * (float)Settings.CursorScale);
                //}

                break;
            case "FadeOut":
                Instance.Settings.FadeOut = (bool)value;
                break;
            case "Pushback":
                Instance.Settings.Pushback = (bool)value;
                break;
            case "Fullscreen":
                Instance.Settings.Fullscreen = (bool)value;
                DisplayServer.WindowSetMode((bool)value ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
                break;
            case "CursorTrail":
                Instance.Settings.CursorTrail = (bool)value;
                break;
            case "TrailTime":
                Instance.Settings.TrailTime = (double)value;
                break;
            case "TrailDetail":
                Instance.Settings.TrailDetail = (double)value;
                break;
            case "CursorDrift":
                Instance.Settings.CursorDrift = (bool)value;
                break;
            case "VideoDim":
                Instance.Settings.VideoDim = (double)value;
                break;
            case "VideoRenderScale":
                Instance.Settings.VideoRenderScale = (double)value;
                break;
            case "SimpleHUD":
                Instance.Settings.SimpleHUD = (bool)value;
                break;
            case "AutoplayJukebox":
                Instance.Settings.AutoplayJukebox = (bool)value;
                break;
            case "AbsoluteInput":
                Instance.Settings.AbsoluteInput = (bool)value;
                break;
            case "RecordReplays":
                Instance.Settings.RecordReplays = (bool)value;
                break;
            case "HitPopups":
                Instance.Settings.HitPopups = (bool)value;
                break;
            case "MissPopups":
                Instance.Settings.MissPopups = (bool)value;
                break;
            case "FPS":
                Instance.Settings.FPS = (double)value;
                Engine.MaxFps = Instance.Settings.UnlockFPS ? 0 : Convert.ToInt32(value);
                break;
            case "UnlockFPS":
                Instance.Settings.UnlockFPS = (bool)value;
                Engine.MaxFps = Instance.Settings.UnlockFPS ? 0 : Convert.ToInt32(Instance.Settings.FPS);
                break;
        }

        UpdateSettings();
    }

    public static void UpdateSettings(bool connections = false)
    {
        OptionButton spaces = Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Space").GetNode<OptionButton>("OptionsButton");
        OptionButton skins = Holder.GetNode("Categories").GetNode("Visuals").GetNode("Container").GetNode("Skin").GetNode<OptionButton>("OptionsButton");
        OptionButton profiles = Holder.GetNode("Header").GetNode<OptionButton>("Profiles");
        string currentProfile = File.ReadAllText($"{Constants.USER_FOLDER}/current_profile.txt");

        skins.Clear();
        profiles.Clear();

        for (int i = 0; i < spaces.ItemCount; i++)
        {
            if (spaces.GetItemText(i) == Instance.Settings.Space)
            {
                spaces.Selected = i;
                break;
            }
        }

        int j = 0;

        foreach (string path in Directory.GetDirectories($"{Constants.USER_FOLDER}/skins"))
        {
            string name = Path.GetFileName(path);

            skins.AddItem(name, j);

            if (Instance.Settings.Skin == name)
            {
                skins.Selected = j;
            }

            j++;
        }

        j = 0;

        foreach (string path in Directory.GetFiles($"{Constants.USER_FOLDER}/profiles"))
        {
            string name = Path.GetFileName(path).TrimSuffix(".json");

            profiles.AddItem(name, j);

            if (currentProfile == name)
            {
                profiles.Selected = j;
            }

            j++;
        }

        foreach (ScrollContainer category in Holder.GetNode("Categories").GetChildren())
        {
            foreach (Panel option in category.GetNode("Container").GetChildren())
            {
                var property = Instance.Settings.GetType().GetProperty(option.Name);

                if (option.FindChild("HSlider") != null)
                {
                    HSlider slider = option.GetNode<HSlider>("HSlider");
                    LineEdit lineEdit = option.GetNode<LineEdit>("LineEdit");

                    slider.Value = (double)property.GetValue(Instance.Settings);
                    lineEdit.Text = (Math.Floor(slider.Value * 1000) / 1000).ToString();

                    if (connections)
                    {
                        void set(string text)
                        {
                            try
                            {
                                if (text == "")
                                {
                                    text = lineEdit.PlaceholderText;
                                }

                                slider.Value = text.ToFloat();
                                lineEdit.Text = slider.Value.ToString();

                                ApplySetting(option.Name, slider.Value);
                            }
                            catch (Exception exception)
                            {
                                ToastNotification.Notify($"Incorrect format; {exception.Message}", 2);
                            }

                            lineEdit.ReleaseFocus();
                        }

                        slider.ValueChanged += (double value) =>
                        {
                            lineEdit.Text = value.ToString();

                            ApplySetting(option.Name, value);
                        };
                        lineEdit.FocusEntered += () =>
                        {
                            FocusedLineEdit = lineEdit;
                        };
                        lineEdit.FocusExited += () =>
                        {
                            set(lineEdit.Text);
                            FocusedLineEdit = null;
                        };
                        lineEdit.TextSubmitted += (string text) =>
                        {
                            set(text);
                        };
                    }
                }
                else if (option.FindChild("CheckButton") != null)
                {
                    CheckButton checkButton = option.GetNode<CheckButton>("CheckButton");

                    checkButton.ButtonPressed = (bool)property.GetValue(Instance.Settings);

                    if (connections)
                    {
                        checkButton.Toggled += (bool value) =>
                        {
                            ApplySetting(option.Name, value);
                        };
                    }
                }
                else if (option.FindChild("LineEdit") != null)
                {
                    LineEdit lineEdit = option.GetNode<LineEdit>("LineEdit");

                    void set(string text)
                    {
                        if (text == "")
                        {
                            text = lineEdit.PlaceholderText;
                            lineEdit.Text = text;
                        }

                        switch (option.Name)
                        {
                            case "Colors":
                                string[] split = text.Replace(" ", "").Replace("\n", ",").Split(",");
                                string raw = "";
                                Color[] colors = new Color[split.Length];

                                if (split.Length == 0)
                                {
                                    split = lineEdit.PlaceholderText.Split(",");
                                }

                                for (int i = 0; i < split.Length; i++)
                                {
                                    split[i] = split[i].TrimPrefix("#").Substr(0, 6).PadRight(6, Convert.ToChar("f"));
                                    split[i] = new Regex("[^a-fA-F0-9$]").Replace(split[i], "f");
                                    colors[i] = Color.FromHtml(split[i]);

                                    raw += $"{split[i]},";
                                }

                                raw = raw.TrimSuffix(",");
                                lineEdit.Text = raw;

                                SkinManager.Instance.Skin.Colors = colors;
                                SkinManager.Instance.Skin.RawColors = raw;

                                break;
                        }

                        lineEdit.ReleaseFocus();
                    }

                    if (connections)
                    {
                        lineEdit.FocusEntered += () =>
                        {
                            FocusedLineEdit = lineEdit;
                        };
                        lineEdit.FocusExited += () =>
                        {
                            set(lineEdit.Text);
                            FocusedLineEdit = null;
                        };
                        lineEdit.TextSubmitted += (string text) =>
                        {
                            set(text);
                        };
                    }
                }
            }
        }
    }

    public static void ShowMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public static void HideMouse()
    {
        if (SceneManager.Scene.Name == "SceneMenu")
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
        }
    }

    public static void Save(string profile = null)
    {
        profile ??= Util.GetProfile();

        Dictionary data = new()
        {
            ["_Version"] = 1
        };

        foreach (PropertyInfo property in typeof(SettingsProfile).GetProperties())
        {
            if (property.DeclaringType != typeof(SettingsProfile))
            {
                continue;
            }

            data[property.Name] = (Variant)typeof(Variant).GetMethod("From").MakeGenericMethod(property.GetValue(Instance.Settings).GetType()).Invoke(Instance.Settings, [property.GetValue(Instance.Settings)]);
        }

        File.WriteAllText($"{Constants.USER_FOLDER}/profiles/{profile}.json", Json.Stringify(data, "\t"));
        SkinManager.Save();
        Logger.Log($"Saved settings {profile}");
    }

    public static void Load(string profile = null)
    {
        profile ??= Util.GetProfile();

        Exception err = null;

        try
        {
            Godot.FileAccess file = Godot.FileAccess.Open($"{Constants.USER_FOLDER}/profiles/{profile}.json", Godot.FileAccess.ModeFlags.Read);
            Dictionary data = (Dictionary)Json.ParseString(file.GetAsText());

            file.Close();

            foreach (PropertyInfo property in typeof(SettingsProfile).GetProperties())
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                if (data.ContainsKey(property.Name))
                {
                    property.SetValue(Instance.Settings, data[property.Name]
                        .GetType()
                        .GetMethod("As", BindingFlags.Public | BindingFlags.Instance)
                        .MakeGenericMethod(property.PropertyType)
                        .Invoke(data[property.Name], null));
                }
            }

            if (Instance.Settings.Fullscreen)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
            }

            ToastNotification.Notify($"Loaded profile [{profile}]");
        }
        catch (Exception exception)
        {
            err = exception;
        }

        if (!Directory.Exists($"{Constants.USER_FOLDER}/skins/{Instance.Settings.Skin}"))
        {
            Instance.Settings.Skin = "default";
            ToastNotification.Notify($"Could not find skin {Instance.Settings.Skin}", 1);
        }

        SkinManager.Load();

        if (err != null)
        {
            ToastNotification.Notify("Settings file corrupted", 2);
            throw Logger.Error($"Settings file corrupted; {err}");
        }

        Logger.Log($"Loaded settings {profile}");
    }
}
