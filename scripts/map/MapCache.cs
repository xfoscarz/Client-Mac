using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Godot;

public static class MapCache
{
    public static void Initialize()
    {
        DatabaseService.Connection.CreateTable<Map>();
    }

    public static void Load(bool fullSync)
    {
        string[] files = Directory.GetFiles(MapsFolder, $"*.{Constants.DEFAULT_MAP_EXT}", SearchOption.AllDirectories);

        if (fullSync)
        {
            syncFiles(files);
            addNonCachedFiles(files);
        }

        OrderAndSetMapSets();
    }

    private static void syncFiles(string[] files)
    {
        var maps = FetchAll();

        for (int i = 0; i < files.Length; i++)
        {
            files[i] = BackSlashToForwardSlash(files[i]);
        }

        var filesHashSet = files.ToHashSet();

        foreach (var map in maps)
        {
            string filePath = BackSlashToForwardSlash(map.FilePath);

            if (filesHashSet.Contains(filePath))
            {
                string checksum = GetMd5Checksum(filePath);

                if (map.Hash == checksum)
                {
                    continue;
                }
                Logger.Log("Map hash error");

                Map newMap;

                try
                {
                    newMap = MapParser.Decode(filePath, null, false, true);
                }
                catch
                {
                    Logger.Error($"Map corrupted: {map.Name}");
                    File.Delete(filePath);
                    DatabaseService.Connection.Delete(map);
                    continue;
                }

                newMap.Id = map.Id;

                DatabaseService.Connection.Update(newMap);

                Logger.Log($"Updated cached map: {newMap.Name}");

                continue;
            }
            else
            {
                DatabaseService.Connection.Delete(map);
                Logger.Log($"Removed {filePath} from the cache, as it no longer exists.");
            }
        }
    }

    private static void addNonCachedFiles(string[] files)
    {
        var maps = FetchAll();

        HashSet<string> hashSet = new();
        maps.ForEach(map => hashSet.Add($"{MapsFolder}/{map.Collection}/{map.Name}.{Constants.DEFAULT_MAP_EXT}"));

        foreach (string file in files)
        {
            if (hashSet.Contains(BackSlashToForwardSlash(file)))
            {
                continue;
            }

            try
            {
                var map = MapParser.Decode(file);
                map.FilePath = $"{Constants.USER_FOLDER}/maps/{map.Collection}/{map.Name}.{Constants.DEFAULT_MAP_EXT}";
                map.Hash = GetMd5Checksum(file);
                map.Collection = file.GetBaseDir().Split("/")[^1];
                File.Move(file, map.FilePath);
                InsertMap(map);
            }
            catch
            {
                File.Delete(file);
                Logger.Log($"Failed to add map non-cached map");
            }
        }
    }

    public static int InsertMap(Map map)
    {
        try
        {
            DatabaseService.Connection.Insert(map);

            return DatabaseService.Connection.Get<Map>(x => x.Hash == map.Hash).Id;
        }
        catch (Exception e)
        {
            var existing = DatabaseService.Connection.Find<Map>(x => x.Hash == x.Hash);
            if (existing == null)
            {
                Logger.Error(e.Message);
                return -1;
            }

            string newPath = Path.Combine(MapsFolder, map.Collection, map.Name);
            string existingPath = Path.Combine(MapsFolder, map.Collection, map.Name);

            if (existingPath != newPath)
            {
                File.Delete(newPath);
                return -1;
            }

            return -1;
        }
    }

    public static void UpdateMap(Map map)
    {
        try
        {
            DatabaseService.Connection.Update(map);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }
    }

    public static void RemoveMap(Map map)
    {
        try
        {
            DatabaseService.Connection.Delete(map);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }
    }

    public static void OrderAndSetMapSets()
    {
        var maps = FetchAll();

        maps = maps.OrderByDescending(x => x.Favorite).ToList();

        MapManager.Maps = maps;
    }

    public static List<MapSet> ConvertToMapSets(IEnumerable<Map> maps)
    {
        var groupedMaps = maps
            .GroupBy(u => u.Collection)
            .Select(x => x.ToList())
            .ToList();

        var mapSets = new List<MapSet>();

        foreach (var mapSet in groupedMaps)
        {
            var set = new MapSet()
            {
                Directory = mapSet.First().Collection,
                Maps = mapSet
            };

            set.Maps.ForEach(x => x.MapSet = set);
            mapSets.Add(set);
        }

        return mapSets;
    }

    public static string GetMd5Checksum(string path)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(path))
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
            }
        }
    }

    public static string MapsFolder => $"{Constants.USER_FOLDER}/maps";

    public static List<Map> FetchAll() => DatabaseService.Connection.Table<Map>().ToList();

    public static string BackSlashToForwardSlash(string path) => path.Replace("\\", "/");
}
