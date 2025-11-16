using Godot;

public partial class SceneManager : Node
{

    private static SubViewportContainer backgroundContainer;

    private static SubViewport backgroundViewport;

    private static Node3D space;

    private static string activeScenePath;

    private static bool skipNextTransition = false;

    public static Node Node { get; private set; }

    public static Node Scene;

    public override void _Ready()
    {
        if (Name != "Main")
        {
            return;
        }

        Node = this;
        backgroundContainer = Node.GetNode<SubViewportContainer>("Background");

        backgroundViewport = backgroundContainer.GetNode<SubViewport>("SubViewport");

        Load("res://scenes/loading.tscn", true);

        Node.GetTree().Connect("node_added", Callable.From((System.Action<Node>)((Node child) =>
        {
            if (child.Name != "SceneMenu" && child.Name != "SceneGame" && child.Name != "SceneResults")
            {
                return;
            }

            if (skipNextTransition)
            {
                skipNextTransition = false;
                return;
            }

            ColorRect inTransition = SceneManager.Scene.GetNode<ColorRect>("Transition");
            inTransition.SelfModulate = Color.FromHtml("ffffffff");
            var inTween = inTransition.CreateTween();
            inTween.TweenProperty(inTransition, "self_modulate", Color.FromHtml("ffffff00"), 0.25).SetTrans(Tween.TransitionType.Quad);
            inTween.Play();
        })));
    }

    public static void ReloadCurrentScene()
    {
        Load(activeScenePath);
    }

    public static void Load(string path, bool skipTransition = false)
    {

        if (skipTransition)
        {
            skipNextTransition = true;
            swapScene(path);
        }
        else
        {
            ColorRect outTransition = Scene.GetNode<ColorRect>("Transition");
            Tween outTween = outTransition.CreateTween();
            outTween.TweenProperty(outTransition, "self_modulate", Color.FromHtml("ffffffff"), 0.25).SetTrans(Tween.TransitionType.Quad);
            outTween.TweenCallback(Callable.From(() =>
            {
                swapScene(path);
            }));
            outTween.Play();
        }
    }

    private static void swapScene(string path)
    {
        var node = ResourceLoader.Load<PackedScene>(path).Instantiate();
        
        if (Scene != null && Scene.GetParent() != null)
        {
            Node.RemoveChild(Scene);
            
            if (space != null && space.GetParent() != null)
            {
                backgroundViewport.RemoveChild(space);
            }
            
            switch (node.Name)
            {
                case "SceneMenu":
                    space = SkinManager.Instance.Skin.MenuSpace;
                    break;
                case "SceneGame":
                    space = SkinManager.Instance.Skin.GameSpace;
                    break;
            }

            // temp solution until the game scene is non-static
            backgroundViewport.TransparentBg = node.Name != "SceneMenu";

            backgroundViewport.AddChild(space);
        }

        activeScenePath = path;
        Scene = node;
        Node.AddChild(node);
    }
}
