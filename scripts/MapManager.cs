using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MapManager : Node
{
    public Dictionary<string, Map> Maps = new();
}
