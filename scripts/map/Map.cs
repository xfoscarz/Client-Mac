using System;
using System.Text;
using System.Text.RegularExpressions;
using Godot;

public partial class Map : RefCounted
{
    public string ID { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public bool Ephemeral { get; set; } = false;

    public string Artist { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string PrettyTitle { get; set; } = string.Empty;

    public float Rating { get; set; } = 0;

    public string[] Mappers { get; set; } = [];

    public string PrettyMappers { get; set; } = string.Empty;

    public string DifficultyName { get; set; } = string.Empty;

    public int Difficulty { get; set; } = 0;

    public int Length { get; set; } = 0;

    public byte[] AudioBuffer { get; set; } = [];

    public string AudioExt { get; set; } = string.Empty;

    public byte[] CoverBuffer { get; set; } = [];

    public byte[] VideoBuffer { get; set; } = [];

    public Note[] Notes { get; set; } = [];

    public Map() { }

    public Map(string filePath, Note[] data = null, string id = null, string artist = "", string title = "", float rating = 0, string[] mappers = null, int difficulty = 0, string difficultyName = null, int? length = null, byte[] audioBuffer = null, byte[] coverBuffer = null, byte[] videoBuffer = null, bool ephemeral = false)
    {
        FilePath = filePath;
        Ephemeral = ephemeral;
        Artist = (artist ?? "").Replace("\n", "");
        Title = (title ?? "").Replace("\n", "");
        PrettyTitle = artist != "" ? $"{artist} - {title}" : title;
        Rating = rating;
        Mappers = mappers ?? ["N/A"];
        PrettyMappers = "";
        Difficulty = difficulty;
        DifficultyName = difficultyName ?? Constants.DIFFICULTIES[Difficulty];
        AudioBuffer = audioBuffer;
        CoverBuffer = coverBuffer;
        VideoBuffer = videoBuffer;

        Notes = data ?? Array.Empty<Note>();
        Length = length ?? Notes[^1].Millisecond;
        ID = (id ?? new Regex("[^a-zA-Z0-9_ -]").Replace($"{Mappers.Stringify()}_{PrettyTitle}".Replace(" ", "_"), "")).Replace(".", "_");
        AudioExt = (AudioBuffer != null && Encoding.UTF8.GetString(AudioBuffer[0..4]) == "OggS") ? "ogg" : "mp3";

        foreach (string mapper in Mappers)
        {
            PrettyMappers += $"{mapper}, ";
        }

        PrettyMappers = PrettyMappers.Substr(0, PrettyMappers.Length - 2).Replace("\n", "");
    }

    public string EncodeMeta()
    {
        return Json.Stringify(new Godot.Collections.Dictionary()
        {
            ["ID"] = ID,
            ["Artist"] = Artist,
            ["Title"] = Title,
            ["Rating"] = Rating,
            ["Mappers"] = Mappers,
            ["Difficulty"] = Difficulty,
            ["DifficultyName"] = DifficultyName,
            ["Length"] = Length,
            ["HasAudio"] = AudioBuffer != null,
            ["HasCover"] = CoverBuffer != null,
            ["HasVideo"] = VideoBuffer != null,
            ["AudioExt"] = AudioExt
        }, "\t");
    }
}
