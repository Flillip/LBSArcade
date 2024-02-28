using Godot;
using System;

public partial class Intro : Control
{
    private AnimatedTextureRect _textureRect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _textureRect = GetNode<AnimatedTextureRect>("./ColorRect/AnimatedTextureRect");

        SignalBus.Instance.StartIntro += StartIntro;
        SignalBus.Instance.StopIntro += StopIntro;
    }

    private void StartIntro()
    {
        GD.Print("recieved signal");
        Visible = true;
        _textureRect.Start();
    }

    private void StopIntro()
    {
        Visible = false;
        _textureRect.Start();
    }
}
