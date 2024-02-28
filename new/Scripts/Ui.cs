using Godot;
using System;
using System.Diagnostics;

public partial class Ui : Control
{
	// Called when the node enters the scene tree for the first time.
	internal static GlobalInputCSharp GlobalInput;

	public override async void _Ready()
	{

        GlobalInput = GetNode<GlobalInputCSharp>("/root/GlobalInput/GlobalInputCSharp");
		Window window = GetViewport().GetWindow();

		window.AlwaysOnTop = true;

		GD.Print("waiting");
		await ToSignal(GetTree().CreateTimer(1.0), "timeout");
        GD.Print("done waiting 1s");

		GD.Print("emitting");
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.StartIntro);

		// load stuff
		await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        SignalBus.Instance.EmitSignal(SignalBus.SignalName.StopIntro);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

	private void OnTransitionFinished()
	{

	}
}
