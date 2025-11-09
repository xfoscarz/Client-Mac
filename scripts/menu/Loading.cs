using System.Threading.Tasks;
using Godot;

namespace Menu;

public partial class Loading : Control
{
    public Control Control;

    public ColorRect Background;
    public TextureRect Splash;
    public TextureRect SplashShift;

    public override void _Ready()
    {
        Control = this;
        SceneManager.Scene = this;

        Background = GetNode<ColorRect>("Background");
        Splash = GetNode<TextureRect>("Splash");
        SplashShift = GetNode<TextureRect>("SplashShift");

        Tween inTween = CreateTween();
        inTween.TweenProperty(Background, "color", Color.FromHtml("060509"), 1).SetTrans(Tween.TransitionType.Quad);
        inTween.Parallel().TweenProperty(Splash, "modulate", Color.FromHtml("ffffffff"), 0.5).SetTrans(Tween.TransitionType.Quad);
        inTween.Parallel().TweenProperty(SplashShift, "modulate", Color.FromHtml("ffffffff"), 0.25).SetTrans(Tween.TransitionType.Quad);
        inTween.TweenProperty(SplashShift, "modulate", Color.FromHtml("00000000"), 2.5).SetTrans(Tween.TransitionType.Sine);
        inTween.Parallel().TweenCallback(Callable.From(LoadMenu));
        inTween.Play();
    }

    public async void LoadMenu()
    {
        await Task.Delay(2500);

        Tween outTween = Control.CreateTween();
        outTween.TweenProperty(Background, "color", Color.Color8(0, 0, 0, 255), 0.5).SetTrans(Tween.TransitionType.Quad);
        outTween.Parallel().TweenProperty(Splash, "modulate", Color.Color8(0, 0, 0, 255), 0.5).SetTrans(Tween.TransitionType.Quad);
        outTween.TweenCallback(Callable.From(() =>
        {
            SceneManager.Load("res://scenes/main_menu.tscn");
        }));
        outTween.Play();
    }
}
