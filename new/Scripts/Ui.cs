using Godot;
using System;
using System.Diagnostics;

public partial class Ui : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//ProjectSettings.SetSetting("display/window/size/always_on_top", true);
		//ProjectSettings.Save();
		DLLImports.SetWindowPos(Process.GetCurrentProcess().Handle, new IntPtr(-1), 0, 0, 0, 0, DLLImports.SetWindowPosFlags.IgnoreMove | DLLImports.SetWindowPosFlags.IgnoreResize);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Process currentProcess = Process.GetCurrentProcess();
		if (!DLLImports.IsWindowFocused(currentProcess))
		{;
			DLLImports.SetWindowFocus(currentProcess);
		}

		if (Input.IsActionJustPressed("ui_left"))
			GD.Print("pressed left");
	}

	private void OnTransitionFinished()
	{

	}
}
