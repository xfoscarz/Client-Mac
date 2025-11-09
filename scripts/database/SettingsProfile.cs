using System;
using System.IO;
using Godot;
using Godot.Collections;
using System.Reflection;

public class SettingsProfile
{
    public bool Fullscreen { get; set; } = false;
    public double VolumeMaster { get; set; } = 50;
    public double VolumeMusic { get; set; } = 50;
    public double VolumeSFX { get; set; } = 50;
    public bool AutoplayJukebox { get; set; } = true;
    public bool AlwaysPlayHitSound { get; set; } = false;
    public string Skin { get; set; } = "default";
    public double FoV { get; set; } = 70;
    public double Sensitivity { get; set; } = 0.5;
    public double Parallax { get; set; } = 0.1;
    public double ApproachRate { get; set; } = 32;
    public double ApproachDistance { get; set; } = 20;
    public double ApproachTime => ApproachDistance / ApproachRate;
    public double FadeIn { get; set; } = 15;
    public bool FadeOut { get; set; } = true;
    public bool Pushback { get; set; } = true;
    public double NoteSize { get; set; } = 0.875;
    public double CursorScale { get; set; } = 1;
    public bool CursorTrail { get; set; } = false;
    public double TrailTime { get; set; } = 0.05;
    public double TrailDetail { get; set; } = 1;
    public bool CursorDrift { get; set; } = true;
    public double VideoDim { get; set; } = 80;
    public double VideoRenderScale { get; set; } = 100;
    public bool SimpleHUD { get; set; } = false;
    public string Space { get; set; } = "skin";
    public bool AbsoluteInput { get; set; } = false;
    public bool RecordReplays { get; set; } = true;
    public bool HitPopups { get; set; } = true;
    public bool MissPopups { get; set; } = true;
    public double FPS { get; set; } = 240;
    public bool UnlockFPS { get; set; } = true;
}
