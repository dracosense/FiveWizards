using Godot;
using System;
using static Lib;

public class Arrow : KinematicBody
{

    public Vector3 speed;
    public float startTime;
    public float damage;
    public int effect;
    public bool active;

    protected Root root;
    protected MeshInstance model;

    public virtual void Destroy()
    {
        QueueFree();
    }

    public virtual void Collide(KinematicCollision c)
    {
        if (c.Collider is Unit)
        {
            ((Unit)c.Collider).Damage(damage, effect, -1    );
        }
        Destroy();
    }

    public virtual void Init()
    {
        damage = BASIC_ARROW_DAMAGE;
        effect = -1;
        active = true;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        model = (MeshInstance)GetNode("Model");
        startTime = root.time;
        Init(); //
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!active)
        {
            return;
        }
        this.Visible = active;
        this.Rotation = new Vector3(0.0f, -(new Vector2(speed.x, speed.z)).Angle() + Mathf.Pi / 2.0f, 0.0f); 
        KinematicCollision c = MoveAndCollide(delta * speed); 
        if (c != null)
        {
            Collide(c);
        }
        if (root.time - startTime >= ARROW_DESTROY_TIME)
        {
            Destroy();
        }
    }

}
