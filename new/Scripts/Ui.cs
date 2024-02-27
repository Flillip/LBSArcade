using Godot;
using System;
using System.Diagnostics;

public partial class Ui : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProjectSettings.SetSetting("display/window/size/always_on_top", true);
		ProjectSettings.Save();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Process currentProcess = Process.GetCurrentProcess();
		//if (!DLLImports.IsWindowFocused(currentProcess))
		//{
		//	GD.Print("not in focus");
  //          DLLImports.SetWindowFocus(currentProcess);
  //      }
		//else
		//	GD.Print("in focus");
	}

	private void OnTransitionFinished()
	{

	}
}
