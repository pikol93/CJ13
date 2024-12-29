using System;
using System.Buffers;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

[Tool]
public partial class Card : Area3D
{
    private static readonly Texture2D DefaultTexture = GD.Load<Texture2D>("res://textures/card_icons/symbol0.png");
    private static readonly Texture2D FallbackNumberTexture = GD.Load<Texture2D>("res://textures/card_icons/numbers/unknown.png");
    private static readonly StandardMaterial3D AllyMaterial = GD.Load<StandardMaterial3D>("res://materials/card_ally.tres");
    private static readonly StandardMaterial3D EnemyMaterial = GD.Load<StandardMaterial3D>("res://materials/card_enemy.tres");

    private static List<Texture2D> NumberTextures = new()
    {
        GD.Load<Texture2D>("res://textures/card_icons/numbers/0.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/1.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/2.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/3.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/4.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/5.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/6.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/7.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/8.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/9.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/10.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/11.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/12.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/13.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/14.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/15.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/16.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/17.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/18.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/19.png"),
        GD.Load<Texture2D>("res://textures/card_icons/numbers/20.png"),
    };

    private int health = 1;
    private int attackVertical = 1;
    private int attackWest = 1;
    private int attackEast = 1;
    private Texture2D icon = null;
    private bool isEnemy = false;

    private Sprite3D iconSprite;
    private MeshInstance3D meshInstance;

    public Node3D Target { get; set; }
	public List<Vector2I> AvailableMoves { get; set; }

    [Export]
    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (IsNodeReady())
            {
                UpdateValues();
            }
        }
    }

    [Export]
    public int AttackVertical
    {
        get => attackVertical;
        set
        {
            attackVertical = value;
            if (IsNodeReady())
            {
                UpdateValues();
            }
        }
    }

    [Export]
    public int AttackWest
    {
        get => attackWest;
        set
        {
            attackWest = value;
            if (IsNodeReady())
            {
                UpdateValues();
            }
        }
    }

    [Export]
    public int AttackEast
    {
        get => attackEast;
        set
        {
            attackEast = value;
            if (IsNodeReady())
            {
                UpdateValues();
            }
        }
    }

    [Export]
    public Texture2D CardIcon
    {
        get => icon;
        set
        {
            icon = value;
            if (IsNodeReady())
            {
                UpdateIcon();
            }
        }
    }

    [Export]
    public bool IsEnemy
    {
        get => isEnemy;
        set
        {
            isEnemy = value;
            if (IsNodeReady())
            {
                UpdateDirection();
            }
        }
    }

    [Export]
    public bool Hidden { get; set; }

    public override void _Ready()
    {
        meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        UpdateValues();
        UpdateIcon();
        UpdateDirection();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        if (Target is Node3D target)
        {
            GlobalPosition = GlobalPosition.Lerp(target.GlobalPosition, 0.1f);
        }

        var targetRotation = Hidden ? Mathf.Pi : 0;
        meshInstance.Rotation = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(meshInstance.Rotation.Z, targetRotation, 0.1f));
    }

    public void ForceSetPositionAndRotation()
    {
        if (Target is Node3D target)
        {
            GlobalPosition = target.GlobalPosition;
        }

        var targetRotation = Hidden ? Mathf.Pi : 0;
        meshInstance.Rotation = new Vector3(0.0f, 0.0f, targetRotation);
    }

    private void UpdateValues()
    {
        GD.Print($"Updating values for card {Name}: {health} {attackVertical} {attackWest} {attackEast}");
        var a = new List<(int, Sprite3D)>
        {
            (health, GetNode<Sprite3D>("MeshInstance3D/Health")),
            (attackVertical, GetNode<Sprite3D>("MeshInstance3D/AttackVertical")),
            (attackWest, GetNode<Sprite3D>("MeshInstance3D/AttackWest")),
            (attackEast, GetNode<Sprite3D>("MeshInstance3D/AttackEast")),
        };

        foreach (var (value, sprite) in a)
        {
            var texture = value < 0 || value >= NumberTextures.Count ? FallbackNumberTexture : NumberTextures[value];
            sprite.Texture = texture;
        }
    }

    private void UpdateIcon()
    {
        var icon = this.icon ?? DefaultTexture;
        var iconSprite = GetNode<Sprite3D>("MeshInstance3D/IconSprite");
        if (iconSprite != null)
        {
            iconSprite.Texture = icon;
        }
    }

    private void UpdateDirection()
    {
        var health = GetNode<Node3D>("MeshInstance3D/Health");
        var healthLabel = GetNode<Node3D>("MeshInstance3D/HealthLabel");
        var attack = GetNode<Node3D>("MeshInstance3D/AttackVertical");
        var attackLabel = GetNode<Node3D>("MeshInstance3D/AttackVerticalLabel");
        var iconSprite = GetNode<Sprite3D>("MeshInstance3D/IconSprite");
        var mesh = GetNode<MeshInstance3D>("MeshInstance3D");

        var multiplier = isEnemy ? -1 : 1;
        var zOffset = 0.12f;

        health.Position = new Vector3(health.Position.X, health.Position.Y, zOffset * multiplier);
        healthLabel.Position = new Vector3(healthLabel.Position.X, healthLabel.Position.Y, zOffset * multiplier);
        attack.Position = new Vector3(attack.Position.X, attack.Position.Y, -zOffset * multiplier);
        attackLabel.Position = new Vector3(attackLabel.Position.X, attackLabel.Position.Y, -zOffset * multiplier);

        iconSprite.FlipH = isEnemy;
        // iconSprite.FlipV = isEnemy;
        mesh.SetSurfaceOverrideMaterial(0, isEnemy ? EnemyMaterial : AllyMaterial);
    }
}
