using Godot;
using Godot.Collections;
using Menu;

public enum SceneType
{
    Loading,
    Menu,
    Game,
    Results
}

public partial class SceneManager : Node
{
    public static Node Node { get; private set; }

    private static Callable? callable;
    private static bool SkipNextTransition = false;

    public static Node ActiveScene;
    private static string activeScenePath;

    private static SubViewportContainer sceneContainer;
    private static SubViewportContainer backgroundContainer;

    private static SubViewport defaultViewport;
    private static SubViewport subViewportUI;

    private static Dictionary<string, Node> scenes;

    public static Node Scene;

    public override void _Ready()
    {
        // Python referenced...
        if (Name != "Main")
        {
            return;
        }
        
        Node = this;

        sceneContainer = Node.GetNode<SubViewportContainer>("Scene");
        backgroundContainer = Node.GetNode<SubViewportContainer>("Background");

        subViewportUI = backgroundContainer.GetNode<SubViewport>("SubViewport");
        defaultViewport = sceneContainer.GetNode<SubViewport>("SubViewport");

        Load("res://scenes/loading.tscn");


        //callable = Callable.From((Node child) =>
        //{
        //    if (child.Name != "SceneMenu" && child.Name != "SceneGame" && child.Name != "SceneResults")
        //    {
        //        return;
        //    }

        //    Scene = child;

        //    if (SkipNextTransition)
        //    {
        //        SkipNextTransition = false;
        //        return;
        //    }

        //    ColorRect inTransition = Scene.GetNode<ColorRect>("Transition");
        //    inTransition.SelfModulate = Color.FromHtml("ffffffff");
        //    Tween inTween = inTransition.CreateTween();
        //    inTween.TweenProperty(inTransition, "self_modulate", Color.FromHtml("ffffff00"), 0.25).SetTrans(Tween.TransitionType.Quad);
        //    inTween.Play();
        //});

        //Node.GetTree().Connect("node_added", (Callable)callable);
    }

    public static void ReloadCurrentScene()
    {
        ActiveScene.QueueFree();
        Load(activeScenePath);
    }

    private static void registerScene(Node node)
    {
        if (!scenes.ContainsKey(node.Name))
            scenes[node.Name] = node;
    }

    public static void Setup()
    {

    }

    public static void Load(string path, bool skipTransition = false)
    {

        if (ActiveScene != null && ActiveScene.GetParent() != null)
        {
            ActiveScene.GetParent().RemoveChild(ActiveScene);
        }

        var node = ResourceLoader.Load<PackedScene>(path).Instantiate();

        activeScenePath = path;
        ActiveScene = node;
        defaultViewport.AddChild(node);
    }

    public static void ALoad(string path, bool skipTransition = false)
    {
        if (skipTransition)
        {
            SkipNextTransition = true;
            Node.GetTree().ChangeSceneToFile(path);
        }
        else
        {
            ColorRect outTransition = Scene.GetNode<ColorRect>("Transition");
            Tween outTween = outTransition.CreateTween();
            outTween.TweenProperty(outTransition, "self_modulate", Color.FromHtml("ffffffff"), 0.25).SetTrans(Tween.TransitionType.Quad);
            outTween.TweenCallback(Callable.From(() =>
            {
                Node.GetTree().ChangeSceneToFile(path);
            }));
            outTween.Play();
        }
    }
}
