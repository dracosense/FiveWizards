using Godot;
using System;
using static Lib;

public class Spell : Arrow
{

    public int copy = 0;
    
    protected CPUParticles particles;

    /*public override void Destroy()
    {
        root.ReleaseSpell(this);
    }*/

    public override void Collide(KinematicCollision c)
    {
        if (c.Collider is Tower)
        {
            ((Tower)c.Collider).SpellCollision(this, c.Normal);
        }
        else
        {
            base.Collide(c);
        }
    }

    public override void Init()
    {
        base.Init();
        copy = 0;
    }

    public override void _Ready()
    {
        base._Ready();
        particles = (CPUParticles)GetNode("Particles");
    }

    public override void _PhysicsProcess(float delta)
    {   
        base._PhysicsProcess(delta);
        if (active)
        {
            particles.MaterialOverride = model.MaterialOverride = ((effect == -1)?null:eMaterials[effect]);
        }
    }

}
