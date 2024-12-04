using Godot;

public partial class Mainmenu : Control
{
    private Button _startButton;
    private Button _exitButton;
    private PackedScene _startLevel;

    public override void _Ready()
    {
        // Corrige las rutas de los nodos con los nombres exactos
        _startButton = GetNode<Button>("MarginContainer/HBoxContainer/VBoxContainer/Start_Button");
        _exitButton = GetNode<Button>("MarginContainer/HBoxContainer/VBoxContainer/Exit_Button");

        // Carga la escena correctamente
        _startLevel = GD.Load<PackedScene>("res://scenes/world.tscn");

        // Conecta las señales a los métodos
        _startButton.Pressed += OnStartPressed;
        _exitButton.Pressed += OnExitPressed;
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToPacked(_startLevel);
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}


