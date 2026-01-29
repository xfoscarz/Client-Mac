
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Godot;

public partial class MapManager : Node
{
    public static Bindable<Map> Selected { get; set; } = new(null);

    public static List<Map> Maps { get; set; } = new();

    public static event Action<Map> MapDeleted;

    public static event Action<Map> MapUpdated;

    public override void _Ready()
    {
        MapCache.Initialize();
        MapCache.Load(true);
    }

    public static void Update(Map map)
    {
        throw new NotImplementedException();
    }

    public static void Delete(Map map)
    {
        try
        {
            string mapSetPath = Path.Combine(MapsFolder, map.Collection);
            string path = Path.Combine(mapSetPath, map.Name);

            try
            {
                File.Delete(path);
            }
            catch
            {
                if (File.Exists(path))
                {
                    {
                        Logger.Error("Unable to delete map");
                    }
                }
            }
            MapCache.RemoveMap(map);
            map.MapSet.Maps.Remove(map);

            MapDeleted?.Invoke(map);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }
    }

    //public static void Delete(MapSet mapSet)
    //{
    //    if (mapSet.Maps.Count == 0)
    //    {
    //        return;
    //    }

    //    try
    //    {
    //        string directory = Path.Combine(MapsFolder, mapSet.Directory);

    //        try
    //        {
    //            Directory.Delete(directory, true);
    //        }
    //        catch (Exception e)
    //        {
    //            Logger.Error(e.Message);
    //        }

    //        if (Directory.Exists(directory))
    //        {
    //            Logger.Error("Unable to delete a mapset.");
    //            return;
    //        }

    //        try
    //        {
    //            mapSet.Maps.ForEach(MapCache.RemoveMap);
    //        }
    //        catch (Exception e)
    //        {
    //            Logger.Error(e.Message);
    //        }

    //        Maps.Remove(mapSet);
    //        MapSetDeleted?.Invoke(mapSet);
    //    }
    //    catch (Exception e)
    //    {
    //        Logger.Error(e.Message);
    //    }
    //}

    public static string MapsFolder => $"{Constants.USER_FOLDER}/maps";
}
