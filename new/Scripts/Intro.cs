using Godot;
using System;

public partial class Intro : Control
{
    private AnimatedTextureRect _logo;
    private Control _foreground;
    private Transition _transition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _foreground = GetNode<Control>("./Foreground");
        _logo = GetNode<AnimatedTextureRect>("./Foreground/AnimatedTextureRect");
        _transition = GetNode<Transition>("./Transition");

        SignalBus.Instance.StartIntro += StartIntro;
        SignalBus.Instance.StopIntro += StopIntro;
        SignalBus.Instance.TransitionScreenCovered += ScreenCovered;
    }

    private void StartIntro()
    {
        Visible = true;
        _logo.Start();
    }

    private void StopIntro()
    {
        _transition.StartTransition();
    }

    private void ScreenCovered()
    {
        GD.Print("hiding");
        _foreground.Visible = false;
        _logo.Stop();
    }
}
