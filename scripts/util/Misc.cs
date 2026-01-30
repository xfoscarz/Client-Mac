using System;
using System.IO;
using Godot;
using System.Collections.Generic;

namespace Util;

public class Misc
{
    public static GodotObject OBJParser = (GodotObject)GD.Load<GDScript>("res://scripts/util/OBJParser.gd").New();

    public static string GetProfile()
    {
        return File.ReadAllText($"{Constants.USER_FOLDER}/current_profile.txt");
    }

    public static ImageTexture GetModIcon(string mod)
    {
        ImageTexture tex;

        switch (mod)
        {
            case "NoFail":
                tex = SkinManager.Instance.Skin.ModNoFailImage;
                break;
            case "Ghost":
                tex = SkinManager.Instance.Skin.ModGhostImage;
                break;
            default:
                tex = new();
                break;
        }

        return tex;
    }

    public static void CopyProperties(Node node, Node reference)
    {
        foreach (Godot.Collections.Dictionary property in reference.GetPropertyList())
		{
            string key = (string)property["name"];

            if (key == "size" || key == "script")
            {
                continue;
            }
            
            node.Set(key, reference.Get(key));
        }
    }

    public static void CopyReference(Node node, Node reference)
    {
        Util.Misc.CopyProperties(node, reference);

        reference.ReplaceBy(node);
        reference.QueueFree();
    }
}
