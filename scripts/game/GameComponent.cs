using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameComponent : Node3D
{
    [Export]
    public Camera3D Camera { get; set; }

    [Export]
    public Array<UIComponent> InterfaceComponents { get; set; }

    [Export]
    public Array<Renderer> Renderers { get; set; }

    public bool Playing { get; private set; } = true;

    public Attempt CurrentAttempt { get; private set; } = new();

    public void Play(Attempt attempt)
    {
        ApplySettings(attempt.Settings);
    }

    public override void _Ready()
    {
        ApplySettings(CurrentAttempt.Settings);
    }

    public override void _Process(double delta)
    {
        // Modify Attempt based on game logic loop
        // ProcessLogic(CurrentAttempt);

        // Update rendering (notes/objects) on attempt state
        foreach (var renderer in Renderers)
        {
            renderer.Process(delta, CurrentAttempt);
        }

        // Update interface components based on attempt state
        foreach (var component in InterfaceComponents)
        {
            component.Process(delta, CurrentAttempt);
        }

    }

    public void UpdateCursor(Attempt attempt, Vector2 mouseDelta)
    {
        var settings = CurrentAttempt.Settings;

        float sensitivity = (float)(settings.Sensitivity);
        sensitivity *= (float)settings.FoV / 70f;

        if (!CurrentAttempt.Mods.Contains("Spin"))
        {
            if (settings.CursorDrift)
            {
                attempt.CursorPosition = (CurrentAttempt.CursorPosition + new Vector2(1, -1) * mouseDelta / 120 * sensitivity).Clamp(-Constants.BOUNDS, Constants.BOUNDS);
            }
            else
            {
                CurrentAttempt.RawCursorPosition += new Vector2(1, -1) * mouseDelta / 120 * sensitivity;
                CurrentAttempt.CursorPosition = CurrentAttempt.RawCursorPosition.Clamp(-Constants.BOUNDS, Constants.BOUNDS);
            }

            attempt.CursorPosition = new Vector2(attempt.CursorPosition.X, attempt.CursorPosition.Y);
            attempt.CameraPosition = new Vector3(0, 0, 3.75f) + new Vector3(CurrentAttempt.CursorPosition.X, CurrentAttempt.CursorPosition.Y, 0) * (float)(settings.Parallax);
            attempt.CameraRotation = Vector3.Zero;
        }

        // Redo spin for it to work for this :/

        //else
        //{
        //    //attempt.CameraRotation += new Vector3(-mouseDelta.Y / 120 * sensitivity / (float)Math.PI, -mouseDelta.X / 120 * sensitivity / (float)Math.PI, 0);
        //    //attempt.CameraRotation = new Vector3((float)Math.Clamp(attempt.CameraRotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)), attempt.CameraRotation.Y, attempt.CameraRotation.Z);
        //    //attempt.CameraPosition = new Vector3(CurrentAttempt.CursorPosition.X * 0.25f, CurrentAttempt.CursorPosition.Y * 0.25f, 3.5f) + Camera.Basis.Z / 4;

        //    //float wtf = 0.95f;
        //    //float hypotenuse = (wtf + attempt.CameraPosition.Z) / Camera.Basis.Z.Z;
        //    //float distance = (float)Math.Sqrt(Math.Pow(hypotenuse, 2) - Math.Pow(wtf + Camera.Position.Z, 2));

        //    //CurrentAttempt.RawCursorPosition = new Vector2(Camera.Basis.Z.X, Camera.Basis.Z.Y).Normalized() * -distance;
        //    //CurrentAttempt.CursorPosition = CurrentAttempt.RawCursorPosition.Clamp(-Constants.BOUNDS, Constants.BOUNDS);
        //    //Cursor.Position = new Vector3(CurrentAttempt.CursorPosition.X, CurrentAttempt.CursorPosition.Y, 0);

        //    //VideoQuad.Position = Camera.Position - Camera.Basis.Z * 103.75f;
        //    //VideoQuad.Rotation = Camera.Rotation;
        //}
    }

    public void ApplySettings(SettingsProfile settings)
    {
        CurrentAttempt.Settings = settings;

        foreach (var component in InterfaceComponents)
        {
            component.ApplySettings(settings);
        }

        foreach (var renderer in Renderers)
        {
            renderer.ApplySettings(settings);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && Playing && !CurrentAttempt.IsReplay)
        {
            UpdateCursor(CurrentAttempt, eventMouseMotion.Relative);

            CurrentAttempt.DistanceMM += eventMouseMotion.Relative.Length() / CurrentAttempt.Settings.Sensitivity / 57.5;
        }
    }
}
