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
    public List<Attack> Antagonisms, Supports;
    public BoardSpace Space;
    public bool HasMoved, HasAttacked;

    int currentAnt, currentSup;

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
    public Attack NextAntagonism => Antagonisms[currentAnt];
    public Attack NextSupport => Supports[currentSup];

    void Start ()
    {
        Health = StartingHealth;
    }

    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, Space.GroundLevel, 0.5f * Time.deltaTime);
    }

    public Attack PopAttack (AttackCategory category)
    {
        bool isAnt = category == AttackCategory.Antagonistic;
        var temp = isAnt ? NextAntagonism : NextSupport;
        
        if (isAnt)
            currentAnt++;
        else
            currentSup++;

        return temp;
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
