using Godot;
using System;

public partial class MapInfo : AspectRatioContainer
{
    public static MapInfo Instance;

    public Map Map;
    public MapInfoContainer InfoContainer;

    private MapList mapList;
    private Panel holder;

    private readonly PackedScene infoContainerTemplate = ResourceLoader.Load<PackedScene>("res://prefabs/map_info_container.tscn");

    public override void _Ready()
    {
        Instance = this;
        
        mapList = GetParent().GetNode<MapList>("MapList");
        holder = GetNode<Panel>("Holder");
    }
	
    public override void _Draw()
    {
        float height = (AnchorBottom - AnchorTop) * GetParent<Control>().Size.Y - OffsetTop + OffsetBottom;

        holder.CustomMinimumSize = Vector2.One * Math.Min(850, height);
    }

	public void Select(Map map)
	{
        if (Map != null && map.Name == Map.Name) { return; }

        Map = map;
		
        var oldContainer = InfoContainer;
        InfoContainer?.Transition(false).TweenCallback(Callable.From(() => { holder.RemoveChild(oldContainer); oldContainer.QueueFree(); }));

        InfoContainer = infoContainerTemplate.Instantiate<MapInfoContainer>();

        holder.AddChild(InfoContainer);
		InfoContainer.Setup(map);
        InfoContainer.Transition(true);
    }
}