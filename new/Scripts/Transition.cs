using Godot;
using System;

public partial class Transition : ColorRect
{
	[Signal] public delegate void TransitionFinishedEventHandler();

	private bool _hasRun = false;
	private bool _hasIntroFinnished = false;

	public override void _Ready()
	{
		SignalBus.Instance.StopIntro += () => _hasIntroFinnished = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnIntroAnimationFinished()
	{
		if (_hasRun || _hasIntroFinnished == false) return;

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
