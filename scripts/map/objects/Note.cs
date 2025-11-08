using System.Collections.Generic;
using Godot;

public struct Note : ITimelineObject, ITweenableObject<NoteTween>
{
    public int Id => (int)ObjectType.Note;

    public int ObjectID { get; } = 0;     // map object type id

    public int Index;                  // note index within the map

    public int Millisecond { get; set; }

    public Tween CurrentTween { get; set; }

    public List<NoteTween> TweenObjects { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    public bool Hit { get; set; } = false;

    public bool Hittable { get; set; } = false;

    public Note(int index, int millisecond, float x, float y)
    {
        Index = index;
        Millisecond = millisecond;
        X = x;
        Y = y;
    }

    public override readonly string ToString() => $"({X}, {Y}) @{Millisecond}ms";

    public int CompareTo(ITimelineObject other)
    {
        throw new System.NotImplementedException();
    }
}
