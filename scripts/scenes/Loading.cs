using Godot;
using System;

public partial class Loading : BaseScene
{
    private Color opaque = new(1, 1, 1, 1);
    private Color transparent = new(1, 1, 1, 0);

    private ColorRect background;
    private TextureRect splash;
    private TextureRect splashShift;
    private Label label;

    public override void _Ready()
    {
        base._Ready();

        background = GetNode<ColorRect>("Background");
        splash = GetNode<TextureRect>("Splash");
        splashShift = GetNode<TextureRect>("SplashShift");
        label = GetNode<Label>("Label");

        label.Modulate = transparent;

        Tween inTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetParallel();
        inTween.TweenProperty(background, "color", Color.FromHtml("#060509"), 1);
        inTween.TweenProperty(splash, "modulate", opaque, 0.5);
        inTween.TweenProperty(splashShift, "modulate", opaque, 0.25);
        inTween.TweenProperty(label, "modulate", opaque, 0.5);

        inTween.SetParallel(false);

        inTween.TweenCallback(Callable.From(() => {
            if (MapManager.Initialized)
            {
                exit();
            }
            else
            {
                MapManager.MapsInitialized += _ => exit();
            }
        }));

        inTween.SetTrans(Tween.TransitionType.Sine);
        inTween.TweenProperty(splashShift, "modulate", transparent, 2.5);
    }

    private void exit()
    {
        label.Text = "Done!";

        Tween outTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetParallel();
        outTween.TweenProperty(background, "color", Color.Color8(0, 0, 0), 0.5);
        outTween.TweenProperty(splash, "modulate", Color.Color8(0, 0, 0), 0.5);
        outTween.Chain().TweenCallback(Callable.From(() => { SceneManager.Load("res://scenes/main_menu.tscn"); }));
    }
}
