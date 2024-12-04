using Godot;

public partial class Coin : Area2D
{
    private AnimatedSprite2D _animatedSprite;

    public override void _Ready()
    {
        // Obtén referencia al nodo AnimatedSprite2D
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        // Asegúrate de que la animación predeterminada está configurada
        _animatedSprite.Play("default");

        // Conecta la señal BodyEntered al método OnBodyEntered
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            GD.Print("+1 coin!");
            GetTree().Root.GetNode<World>("world").OnCoinCollected();
            QueueFree(); // Elimina la moneda del juego
        }
    }
}
