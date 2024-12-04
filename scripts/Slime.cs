using Godot;

public partial class Slime : CharacterBody2D
{
    [Export] private float Speed { get; set; } = 40f;
    private const int DamageToPlayer = 20; // Daño al jugador

    private AnimatedSprite2D anim;
    private Node2D player = null;
    private bool playerChase = false;
    private int health = 60;
    private const int MaxHealth = 60; // Salud máxima
    private bool slimeAlive = true; // Indica si el Slime está vivo
    private bool playerInAttackZone = false;
    private bool attackCooldown = false;

    private ProgressBar healthBar;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        healthBar = GetNode<ProgressBar>("healthbar");

        GetNode<Area2D>("detection_area").BodyEntered += OnDetectAreaBodyEntered;
        GetNode<Area2D>("detection_area").BodyExited += OnDetectAreaBodyExited;

        GetNode<Area2D>("enemy_hitbox").BodyEntered += OnEnemyHitboxBodyEntered;
        GetNode<Area2D>("enemy_hitbox").BodyExited += OnEnemyHitboxBodyExited;

        GetNode<Timer>("attack_cooldown").Timeout += OnAttackCooldownTimeout;

        UpdateHealth(); // Inicializar la barra de salud
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!slimeAlive)
            return; // Si está muerto, no ejecutar lógica

        if (playerChase && player != null)
        {
            Position += (player.Position - Position).Normalized() * Speed * (float)delta;

            anim.Play("walk");
            anim.FlipH = (player.Position.X - Position.X) < 0;
        }
        else
        {
            anim.Play("idle");
        }

        AttackPlayer();
    }

    private void OnDetectAreaBodyEntered(Node body)
    {
        if (body is Player target)
        {
            player = target;
            playerChase = true;
        }
    }

    private void OnDetectAreaBodyExited(Node body)
    {
        if (body == player)
        {
            player = null;
            playerChase = false;
        }
    }

    private void OnEnemyHitboxBodyEntered(Node body)
    {
        if (body is Player)
        {
            playerInAttackZone = true;
        }
    }

    private void OnEnemyHitboxBodyExited(Node body)
    {
        if (body is Player)
        {
            playerInAttackZone = false;
        }
    }

    private void AttackPlayer()
    {
        if (playerInAttackZone && !attackCooldown && slimeAlive && player is Player playerNode)
        {
            playerNode.TakeDamage(DamageToPlayer);
            attackCooldown = true;
            GetNode<Timer>("attack_cooldown").Start();
        }
    }

    private void OnAttackCooldownTimeout()
    {
        attackCooldown = false;
    }

    public void TakeDamage(int damage)
    {
        if (!slimeAlive)
            return; // No recibir daño si ya está muerto

        health -= damage;
        GD.Print($"Slime Health: {health}");
        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        slimeAlive = false; // Cambiar el estado a muerto
        GD.Print("Slime has been killed");
        anim.Play("death"); // Reproduce la animación de muerte si existe
        GetTree().Root.GetNode<World>("world").OnSlimeDefeated();
        QueueFree(); // Elimina al slime del juego
    }

    private void UpdateHealth()
    {
        // Actualiza el valor de la barra de salud
        healthBar.Value = health;

        // Oculta la barra de salud si la salud está llena
        healthBar.Visible = health < MaxHealth;
    }
}


