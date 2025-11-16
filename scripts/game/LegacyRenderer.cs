using System;
using Godot;

public partial class LegacyRenderer : MultiMeshInstance3D
{
    private SettingsProfile settings;

    public override void _Ready()
    {
        settings = SettingsManager.Instance.Settings;
    }

    public override void _Process(double delta)
    {
        if (!LegacyRunner.Playing)
        {
            return;
        }

        Multimesh.InstanceCount = LegacyRunner.ToProcess;

        float ar = (float)(LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].ApproachRate : settings.ApproachRate);
        float ad = (float)(LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].ApproachDistance : settings.ApproachDistance);
        float at = ad / ar;
        float fadeIn = (float)(LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].FadeIn : settings.FadeIn);
        bool fadeOut = LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].FadeOut : settings.FadeOut;
        bool pushback = LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].Pushback : settings.Pushback;
        float noteSize = (float)(LegacyRunner.CurrentAttempt.IsReplay ? LegacyRunner.CurrentAttempt.Replays[0].NoteSize : settings.NoteSize);
        Transform3D transform = new(new Vector3(noteSize / 2, 0, 0), new Vector3(0, noteSize / 2, 0), new Vector3(0, 0, noteSize / 2), Vector3.Zero);

        for (int i = 0; i < LegacyRunner.ToProcess; i++)
        {
            Note note = LegacyRunner.ProcessNotes[i];
            float depth = (note.Millisecond - (float)LegacyRunner.CurrentAttempt.Progress) / (1000 * at) * ad / (float)LegacyRunner.CurrentAttempt.Speed;
            float alpha = Math.Clamp((1 - (float)depth / ad) / (fadeIn / 100), 0, 1);

            if (LegacyRunner.CurrentAttempt.Mods["Ghost"])
            {
                alpha -= Math.Min(1, (ad - depth) / (ad / 2));
            }
            else if (fadeOut)
            {
                alpha -= (ad - depth) / (ad + (float)Constants.HIT_WINDOW * ar / 1000);
            }

            if (!pushback && note.Millisecond - LegacyRunner.CurrentAttempt.Progress <= 0)
            {
                alpha = 0;
            }

            int j = LegacyRunner.ToProcess - i - 1;
            Color color = SkinManager.Instance.Skin.Colors[note.Index % SkinManager.Instance.Skin.Colors.Length];

            transform.Origin = new Vector3(note.X, note.Y, -depth);
            color.A = alpha;
            Multimesh.SetInstanceTransform(j, transform);
            Multimesh.SetInstanceColor(j, color);
        }
    }
}
