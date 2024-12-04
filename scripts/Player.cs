using Godot;

public partial class Player : CharacterBody2D
{
    [Export] private int Speed { get; set; } = 100;
    [Export] private PackedScene ArrowScene { get; set; } // Ahora esta propiedad será utilizada para cargar la escena de la flecha
    
    private const int AttackDamage = 20; // Daño por ataque
    private const float AttackCooldownDuration = 0.5f; // Duración del cooldown en segundos
    private const int MaxHealth = 100; // Salud máxima
    private const int RegenAmount = 20; // Cantidad de salud regenerada por tick

    private string currentDir = "front";
    private AnimatedSprite2D anim;
    private int health = MaxHealth;
    private bool playerAlive = true;
    private bool attackCooldown = false; // Para controlar el cooldown de ataques
    private bool attackInProgress = false; // Controla si un ataque está en curso

    private ProgressBar healthBar;

    // Ruta para cargar el script Arrow.cs
    private Script arrowScript = GD.Load<Script>("res://scripts/Arrow.cs");

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        healthBar = GetNode<ProgressBar>("healthbar"); // Nodo ProgressBar para la barra de salud
        anim.Play("front_idle");

        // Conectar señales del área de ataque
        var attackArea = GetNode<Area2D>("player_hitbox");
        attackArea.BodyEntered += OnAttackAreaBodyEntered;

        // Conectar temporizadores
        GetNode<Timer>("deal_attack_timer").Timeout += OnDealAttackTimerTimeout;
        GetNode<Timer>("attack_cooldown").Timeout += OnAttackCooldownTimeout;
        GetNode<Timer>("regen_timer").Timeout += OnRegenTimerTimeout;

        UpdateHealth(); // Inicializar la barra de salud
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!playerAlive)
            return;

        PlayerMovement(delta);

        if (Input.IsActionJustPressed("attack"))
        {
            Attack();
        }

        if (Input.IsActionJustPressed("ranged_attack"))
        {
            ShootArrow();
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void PlayerMovement(double delta)
    {
        if (Input.IsActionPressed("ui_right"))
        {
            currentDir = "side";
            PlayAnim(1);
            Velocity = new Vector2(Speed, 0);
            anim.FlipH = false; // No refleja el sprite al moverse a la derecha
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            currentDir = "side";
            PlayAnim(1);
            Velocity = new Vector2(-Speed, 0);
            anim.FlipH = true; // Refleja el sprite al moverse a la izquierda
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            currentDir = "front";
            PlayAnim(1);
            Velocity = new Vector2(0, Speed);
        }
        else if (Input.IsActionPressed("ui_up"))
        {
            currentDir = "back";
            PlayAnim(1);
            Velocity = new Vector2(0, -Speed);
        }
        else
        {
            PlayAnim(0);
            Velocity = Vector2.Zero;
        }

        MoveAndSlide();
    }

    private void PlayAnim(int movement)
    {
        string animation = movement switch
        {
            1 => $"{currentDir}_walk",
            _ => attackInProgress ? $"{currentDir}_attack" : $"{currentDir}_idle"
        };
        anim.Play(animation);
    }

    private void Attack()
    {
        if (!attackCooldown)
        {
            attackInProgress = true;
            attackCooldown = true;

            // Reproduce la animación de ataque y comienza el temporizador de ataque
            anim.Play($"{currentDir}_attack");
            GetNode<Timer>("deal_attack_timer").Start(); // Controla el momento del ataque
            GetNode<Timer>("attack_cooldown").Start();   // Controla el cooldown del ataque
        }
    }

    private void ShootArrow()
    {
        if (ArrowScene == null)
        {
            GD.PrintErr("ArrowScene is not assigned!");
            return;
        }

        // Instanciar la flecha desde la escena exportada
        var arrowInstance = (Node2D)ArrowScene.Instantiate();
        GetParent().AddChild(arrowInstance);

        // Configurar la posición y rotación inicial de la flecha
        arrowInstance.Position = Position;
        arrowInstance.Rotation = GetArrowRotation();
        GD.Print("Arrow Shot!");
    }

    private float GetArrowRotation()
    {
        // Calcular la dirección según el movimiento o la dirección actual
        return currentDir switch
        {
            "front" => Mathf.Pi / 2,
            "back" => -Mathf.Pi / 2,
            "side" => anim.FlipH ? Mathf.Pi : 0,
            _ => 0
        };
    }

    private void OnDealAttackTimerTimeout()
    {
        attackInProgress = false;
        PlayAnim(0); // Regresa a la animación de reposo
    }

    private void OnAttackCooldownTimeout()
    {
        attackCooldown = false; // Cooldown terminado
    }

    private void OnAttackAreaBodyEntered(Node body)
    {
        if (attackInProgress && body is Slime slime)
        {
            slime.TakeDamage(AttackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; // Reducir salud si es positivo; curar si es negativo
        health = Mathf.Clamp(health, 0, 100); // Limitar salud entre 0 y 100
        if (health < 0)
        {
            health = 0;
        }

        GD.Print($"Player Health: {health}");
        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerAlive = false;
        GD.Print("Player has been killed");
        QueueFree();
    }

    private void UpdateHealth()
    {
        // Actualiza el valor de la barra de salud
        healthBar.Value = health;

        // Oculta o muestra la barra según la salud actual
        healthBar.Visible = health < MaxHealth;
    }

    private void OnRegenTimerTimeout()
    {
        if (health < MaxHealth)
        {
            health += RegenAmount;
            if (health > MaxHealth)
            {
                health = MaxHealth;
            }

            GD.Print($"Health Regenerated: {health}");
            UpdateHealth();
        }
    }
}



