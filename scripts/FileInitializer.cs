using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

public partial class FileInitializer : Node
{
    Node OBJExporter;

    public override void _Ready()
    {
        OBJExporter = GetNode("/root/ObjExporter");

        deepCopy();
    }

    void deepCopy(string resDir = "")
    {
        string userDir = $"{Constants.USER_FOLDER}{resDir}";

        if (!Directory.Exists(userDir))
        {
            Directory.CreateDirectory(userDir);
        }


        foreach (string resFile in DirAccess.GetFilesAt($"res://user{resDir}"))
        {

            string userFile = $"{userDir}/{resFile}";
            string ext = resFile.GetExtension();

            if (File.Exists(userFile) || ext == "import" || ext == "uid" || ext == "gitkeep")
            {
                continue;
            }

            var source = Godot.FileAccess.Open($"res://user{resDir}/{resFile}", Godot.FileAccess.ModeFlags.Read);
            byte[] buffer = source.GetBuffer((long)source.GetLength());
            source.Close();

            Godot.FileAccess copy = Godot.FileAccess.Open(userFile, Godot.FileAccess.ModeFlags.Write);
            copy.StoreBuffer(buffer);
            copy.Close();
        }

        // Attempts to load files that have not been added as a resource.

        foreach (string resFile in ResourceLoader.ListDirectory($"res://user{resDir}").Where(x => x.Last() != '/'))
        {

            string userFile = $"{userDir}/{resFile}";
            string ext = resFile.GetExtension();

            if (File.Exists(userFile) || ext == "import" || ext == "uid" || ext == "gitkeep")
            {
                continue;
            }

            var resource = GD.Load($"res://user{resDir}/{resFile}");
            byte[] buffer = [];

            switch (resource.GetType().Name)
            {
                case "CompressedTexture2D":
                    buffer = (resource as CompressedTexture2D).GetImage().SavePngToBuffer();
                    break;
                case "AudioStreamMP3":
                    buffer = (resource as AudioStreamMP3).Data;
                    break;
                case "ArrayMesh":
                    OBJExporter.Call("save_mesh_to_files", resource, userDir, resFile.Replace(".obj", ""));
                        continue;
                default:
                    Logger.Error($"[{resFile}] {resource.GetType().Name} is not supported for the user folder.");
                    continue;
            }
            File.WriteAllBytes(userFile, buffer);
        }

        foreach (string dir in Godot.DirAccess.GetDirectoriesAt($"res://user{resDir}"))
        {
            deepCopy($"{resDir}/{dir}");
        }
    }
}
