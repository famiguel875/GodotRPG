using Godot;

public partial class Fruit : Area2D
{
    private AnimatedSprite2D _animatedSprite;
    [Export] private int HealAmount { get; set; } = 20; // Cantidad de curación

    public override void _Ready()
    {
        // Conecta la señal BodyEntered al método OnBodyEntered
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            // Curar al jugador
            player.TakeDamage(-HealAmount); // Uso de valor negativo para curar
            GD.Print($"Player healed by {HealAmount}!");
            
            // Eliminar la fruta
            QueueFree();
        }
    }
}

