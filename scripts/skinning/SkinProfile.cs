using Godot;
using System;

[GlobalClass]
public partial class SkinProfile : GodotObject
{
    public Color[] Colors { get; set; } = [new(0x00ffedff), new(0xff8ff9ff)];

    public string RawColors { get; set; } = "00ffed,ff8ff9";

    public ImageTexture CursorImage { get; set; } = new();

    public ImageTexture GridImage { get; set; } = new();

    public ImageTexture PanelLeftBackgroundImage { get; set; } = new();

    public ImageTexture PanelRightBackgroundImage { get; set; } = new();

    public ImageTexture HealthImage { get; set; } = new();

    public ImageTexture HealthBackgroundImage { get; set; } = new();

    public ImageTexture ProgressImage { get; set; } = new();

    public ImageTexture ProgressBackgroundImage { get; set; } = new();

    public ImageTexture HitsImage { get; set; } = new();

    public ImageTexture MissesImage { get; set; } = new();

    public ImageTexture MissFeedbackImage { get; set; } = new();

    public ImageTexture JukeboxPlayImage { get; set; } = new();

    public ImageTexture JukeboxPauseImage { get; set; } = new();

    public ImageTexture JukeboxSkipImage { get; set; } = new();

    public ImageTexture FavoriteImage { get; set; } = new();

    public ImageTexture ModNofailImage { get; set; } = new();

    public ImageTexture ModSpinImage { get; set; } = new();

    public ImageTexture ModGhostImage { get; set; } = new();

    public ImageTexture ModChaosImage { get; set; } = new();

    public ImageTexture ModFlashlightImage { get; set; } = new();

    public ImageTexture ModHardrockImage { get; set; } = new();

    public byte[] HitSoundBuffer { get; set; } = [];

    public byte[] FailSoundBuffer { get; set; } = [];

    public ArrayMesh NoteMesh { get; set; } = new();

    public string MenuSpaceName = "waves";

    public string GameSpaceName = "grid";

    public Node3D MenuSpace { get; set; }

    public Node3D GameSpace { get; set; }
}
