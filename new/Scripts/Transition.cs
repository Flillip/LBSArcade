using Godot;
using System;

public partial class Transition : ColorRect
{
	[Export] float Length;

	private bool _hasRun = false;
	private bool _startedIntro = false;

	public void StartTransition() => _startedIntro = true;

	private async void OnIntroAnimationFinished()
	{
		if (_hasRun || _startedIntro == false) return;

		_hasRun = true;
		Tween tween = GetTree().CreateTween();
		tween.TweenMethod(Callable.From<float>(TweenShader), 0.0f, 1.0f, Length / 2f);

		await ToSignal(tween, "finished");

        SignalBus.Instance.EmitSignal(SignalBus.SignalName.TransitionScreenCovered);

        tween = GetTree().CreateTween();
        tween.TweenMethod(Callable.From<float>(TweenShader), 1.0f, 2.0f, Length / 2f);
    }

    private void TweenShader(float value)
	{
		(Material as ShaderMaterial).SetShaderParameter("progress", value);
	}
}
