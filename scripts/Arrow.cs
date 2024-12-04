using Godot;

public partial class Arrow : Area2D
{
    [Export] public int Speed { get; set; } = 400;
    [Export] public int Damage { get; set; } = 20;

    public override void _Ready()
    {
        BodyEntered += _OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Transform.BasisXform(Vector2.Right) * Speed * (float)delta;
    }

    private void _OnBodyEntered(Node body)
    {
        if (body is Slime slime)
        {
            slime.TakeDamage(Damage);
            QueueFree();
        }
    }
}


