using Godot;

public partial class World : Node2D
{
    private int totalSlimes = 2; // Total de slimes a derrotar
    private int totalCoins = 2;  // Total de monedas a recoger

    private int collectedCoins = 0; // Contador de monedas recogidas
    private int defeatedSlimes = 0; // Contador de slimes derrotados

    public override void _Ready()
    {
        GD.Print($"Total Slimes: {totalSlimes}, Total Coins: {totalCoins}");
    }

    public void OnCoinCollected()
    {
        collectedCoins++;
        GD.Print($"Coins collected: {collectedCoins}/{totalCoins}");
        CheckEndgameCondition();
    }

    public void OnSlimeDefeated()
    {
        defeatedSlimes++;
        GD.Print($"Slimes defeated: {defeatedSlimes}/{totalSlimes}");
        CheckEndgameCondition();
    }

    private void CheckEndgameCondition()
    {
        if (collectedCoins >= totalCoins && defeatedSlimes >= totalSlimes)
        {
            GD.Print("Endgame Condition Met!");
            TriggerEndgame();
        }
    }

    private void TriggerEndgame()
    {
        GD.Print("You Win!"); // Mensaje de victoria en la consola
        GetTree().Paused = true; // Pausa el juego
    }

}




