using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    public override void _Ready()
    {
        Instance = GetNode<SignalBus>("/root/SignalBus");
    }


    [Signal] public delegate void StartIntroEventHandler();
    [Signal] public delegate void StopIntroEventHandler();
}
