using Godot;
using System;

namespace Pikol93.CJ13;

public partial class StandingTorch : Node3D
{
    private OmniLight3D light;
    private GpuParticles3D particles;

    public override void _Ready()
    {
        light = GetNode<OmniLight3D>("OmniLight3D");
        particles = GetNode<GpuParticles3D>("GPUParticles3D");
    }

    public void Enable()
    {
        light.Visible = true;
        particles.Emitting = true;
    }

    public void Disable()
    {
        light.Visible = false;
        particles.Emitting = false;
    }
}
