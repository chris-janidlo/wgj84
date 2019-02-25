using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    public string Name;
    public Team Team;
    public AttackType Type;
    public int StartingHealth;
    public int Speed;
    public BoardSpace Space;

    [SerializeField]
    int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                die();
            }
        }
    }

    public Vector2Int Position => Space.Position;
    public float PercentHealth => (float) Health / StartingHealth;

    void Start ()
    {
        Health = StartingHealth;
    }

    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, Space.GroundLevel, 0.5f * Time.deltaTime);
    }

    void die ()
    {
        Destroy(gameObject);
    }
}

public enum Team
{
    Player, AI
}
