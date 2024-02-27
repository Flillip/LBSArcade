using Godot;
using System;

public partial class AnimatedTextureRect : TextureRect
{
	[Signal] public delegate void AnimationFinishedEventHandler();

	[Export] SpriteFrames SpriteFrames;
	[Export] float Delay;
	[Export] bool ShouldLoop;
	[Export] bool Running {
		get {
			return _running;
		}

		set {
			_running = value;
			if (value) tween.Play();
			else tween.Stop();
		}
	}

	private bool _running = false;
	private Tween tween;
	private int length;
	private int prevFrame = -1;

    public override void _Ready()
    {
		length = SpriteFrames.GetFrameCount("default") - 1;
		float duration = Delay * length;

        tween = GetTree().CreateTween();
		tween.TweenMethod(Callable.From<int>(Animate), 0, length, duration);
		
		if (ShouldLoop) tween.SetLoops();
		if (Running == false) tween.Stop();
    }

	private void Animate(int frame)
	{
		if (frame != prevFrame)
		{
			this.Texture = SpriteFrames.GetFrameTexture("default", frame);
			if (frame == length) 
			{
				EmitSignal(SignalName.AnimationFinished);
				if (ShouldLoop == false) Running = false;
			}
		}

		prevFrame = frame;
	}
}
