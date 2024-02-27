using Godot;
using System;

public partial class Transition : ColorRect
{
	[Signal] public delegate void TransitionFinishedEventHandler();

	// Called when the node enters the scene tree for the first time.
	private bool _hasRun = false;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnIntroAnimationFinished()
	{
		if (_hasRun) return;

		GD.Print("animation");

		_hasRun = true;
		Tween tween = GetTree().CreateTween();
		tween.TweenMethod(Callable.From<float>(TweenShader), 0.0f, 1.0f, 2.0f);

		await ToSignal(tween, "finished");
		GD.Print("Done");
	}

	private void TweenShader(float value)
	{
		(Material as ShaderMaterial).SetShaderParameter("progress", value);
	}
}
