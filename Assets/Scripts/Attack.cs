using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public int Damage;
    public AttackType Type;
    public bool IsSupporting;
    public int Range;
}

public enum AttackType
{
    Social, Physical, Emotional
}
