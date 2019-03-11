using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardPiece
{
    public string Name;
    public Team Team;
    public AttackType Type;
    public int StartingHealth;
    public int Speed;
    public List<Attack> Antagonisms, Supports;
    public bool HasMoved, HasAttacked, IsDead;

    public event EventHandler HasDied;

    int currentAnt, currentSup;

    [SerializeField]
    int _health;
    public int Health
    {
        get
        {
            if (_health <= 0 && !IsDead)
            {
                _health = StartingHealth;
            }
            return _health;
        }
        set
        {
            _health = value;
            if (value <= 0)
            {
                IsDead = true;
                var temp = HasDied;
                if (temp != null)
                {
                    temp(this, null);
                }
            }
        }
    }

    public float PercentHealth => (float) Health / StartingHealth;
    public Attack NextAntagonism => Antagonisms[currentAnt];
    public Attack NextSupport => Supports[currentSup];

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

    public Attack PeekAttack (AttackCategory category)
    {
        if (category == AttackCategory.Antagonistic)
        {
            return NextAntagonism;
        }
        else
        {
            return NextSupport;
        }
    }

    // shallow clone because we don't care if antagonisms and supports refer to the same list
    public BoardPiece Clone ()
    {
        return new BoardPiece
        {
            Name = Name,
            Team = Team,
            Type = Type,
            StartingHealth = StartingHealth,
            Speed = Speed,
            Antagonisms = Antagonisms,
            Supports = Supports,
            HasMoved = HasMoved,
            HasAttacked = HasAttacked,
            IsDead = IsDead,
            currentAnt = currentAnt,
            currentSup = currentSup,
            _health = _health
        };
    }
}

[Serializable]
public class Attack
{
    public string DialogPreview;
    public List<string> Dialog;
    public int Damage;
    public int Range;
}

public enum AttackType
{
    Social, Physical, Emotional
}

public enum AttackCategory
{
    Antagonistic, Supporting
}

public enum Team
{
    Player, AI
}
