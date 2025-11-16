using System;
using Godot;
using Menu;

public partial class KeybindsManager : Node
{
    private static bool popupShown = false;
    private static ulong lastVolumeChange = 0;
    private static Node lastVolumeChangeScene;

    public override void _Process(double delta)
    {
        if (lastVolumeChangeScene == SceneManager.Scene && popupShown && Time.GetTicksMsec() - lastVolumeChange >= 1000)
        {
            popupShown = false;

            Panel volumePopup = SceneManager.Scene.GetNode<Panel>("Volume");
            Label label = volumePopup.GetNode<Label>("Label");
            Tween tween = volumePopup.CreateTween();
            tween.TweenProperty(volumePopup, "modulate", Color.FromHtml("ffffff00"), 0.25).SetTrans(Tween.TransitionType.Quad);
            tween.Parallel().TweenProperty(label, "anchor_bottom", 1, 0.35).SetTrans(Tween.TransitionType.Quad);
            tween.Play();
        }
    }

    public override void _Input(InputEvent @event)
    {
        var settings = SettingsManager.Instance.Settings;
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.F11 || (eventKey.AltPressed && (eventKey.Keycode == Key.Enter || eventKey.Keycode == Key.KpEnter)))
            {
                bool value = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed;

                settings.Fullscreen = value;
                DisplayServer.WindowSetMode(value ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);

                if (SceneManager.Scene.Name == "SceneMenu")
                {
                    SettingsManager.UpdateSettings();
                    MainMenu.UpdateSpectrumSpacing();
                }
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        var settings = SettingsManager.Instance.Settings;

        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed)
        {
            if (eventMouseButton.CtrlPressed && (eventMouseButton.ButtonIndex == MouseButton.WheelUp || eventMouseButton.ButtonIndex == MouseButton.WheelDown))
            {
                switch (eventMouseButton.ButtonIndex)
                {
                    case MouseButton.WheelUp:
                        SettingsManager.Instance.Settings.VolumeMaster = Math.Min(100, settings.VolumeMaster + 5f);
                        break;
                    case MouseButton.WheelDown:
                        SettingsManager.Instance.Settings.VolumeMaster = Math.Max(0, settings.VolumeMaster - 5f);
                        break;
                }

                Panel volumePopup = SceneManager.Scene.GetNode<Panel>("Volume");
                Label label = volumePopup.GetNode<Label>("Label");
                label.Text = settings.VolumeMaster.ToString();
                Tween tween = volumePopup.CreateTween();
                tween.TweenProperty(volumePopup, "modulate", Color.FromHtml("ffffffff"), 0.25).SetTrans(Tween.TransitionType.Quad);
                tween.Parallel().TweenProperty(volumePopup.GetNode<ColorRect>("Main"), "anchor_right", settings.VolumeMaster / 100, 0.15).SetTrans(Tween.TransitionType.Quad);
                tween.Parallel().TweenProperty(label, "anchor_bottom", 0, 0.15).SetTrans(Tween.TransitionType.Quad);
                tween.Play();

                popupShown = true;
                lastVolumeChange = Time.GetTicksMsec();
                lastVolumeChangeScene = SceneManager.Scene;

                SoundManager.UpdateSounds();
                SoundManager.UpdateVolume();

            }
        }
    }
}
