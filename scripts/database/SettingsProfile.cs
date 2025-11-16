using Godot;
using System;

public partial class SettingsProfile : GodotObject
{
    [Signal]
    public delegate void FieldUpdatedEventHandler(string name, Variant value);

    private void set<T>(ref T field, T value, string name)
    {
        if (!Equals(field, value))
        {
            field = value;
            EmitSignalFieldUpdated(name, Variant.From(value));
        }
    }

    public bool Fullscreen { get; set => set(ref field, value, nameof(Fullscreen)); }
    public double VolumeMaster { get; set => set(ref field, value, nameof(VolumeMaster)); } = 50;
    public double VolumeMusic { get; set => set(ref field, value, nameof(VolumeMusic)); } = 50;
    public double VolumeSFX { get; set => set(ref field, value, nameof(VolumeSFX)); } = 50;
    public bool AutoplayJukebox { get; set => set(ref field, value, nameof(AutoplayJukebox)); } = true;
    public bool AlwaysPlayHitSound { get; set => set(ref field, value, nameof(AlwaysPlayHitSound)); }
    public string Skin { get; set => set(ref field, value, nameof(Skin)); } = "default";
    public double FoV { get; set => set(ref field, value, nameof(FoV)); } = 70;
    public double Sensitivity { get; set => set(ref field, value, nameof(Sensitivity)); } = 0.5;
    public double Parallax { get; set => set(ref field, value, nameof(Parallax)); } = 0.1;
    public double ApproachRate { get; set => set(ref field, value, nameof(ApproachRate)); } = 32;
    public double ApproachDistance { get; set => set(ref field, value, nameof(ApproachDistance)); } = 20;
    public double ApproachTime => ApproachDistance / ApproachRate;
    public double FadeIn { get; set => set(ref field, value, nameof(FadeIn)); } = 15;
    public bool FadeOut { get; set => set(ref field, value, nameof(FadeOut)); } = true;
    public bool Pushback { get; set => set(ref field, value, nameof(Pushback)); } = true;
    public double NoteSize { get; set => set(ref field, value, nameof(NoteSize)); } = 0.875;
    public double CursorScale { get; set => set(ref field, value, nameof(CursorScale)); } = 1;
    public bool CursorTrail { get; set => set(ref field, value, nameof(CursorTrail)); }
    public double TrailTime { get; set => set(ref field, value, nameof(TrailTime)); } = 0.05;
    public double TrailDetail { get; set => set(ref field, value, nameof(TrailDetail)); } = 1;
    public bool CursorDrift { get; set => set(ref field, value, nameof(CursorDrift)); } = true;
    public double VideoDim { get; set => set(ref field, value, nameof(VideoDim)); } = 80;
    public double VideoRenderScale { get; set => set(ref field, value, nameof(VideoRenderScale)); } = 100;
    public bool SimpleHUD { get; set => set(ref field, value, nameof(SimpleHUD)); }
    public string Space { get; set => set(ref field, value, nameof(Space)); } = "skin";
    public bool AbsoluteInput { get; set => set(ref field, value, nameof(AbsoluteInput)); }
    public bool RecordReplays { get; set => set(ref field, value, nameof(RecordReplays)); } = true;
    public bool HitPopups { get; set => set(ref field, value, nameof(HitPopups)); } = true;
    public bool MissPopups { get; set => set(ref field, value, nameof(MissPopups)); } = true;
    public double FPS { get; set => set(ref field, value, nameof(FPS)); } = 240;
    public bool UnlockFPS { get; set => set(ref field, value, nameof(UnlockFPS)); } = true;
}
