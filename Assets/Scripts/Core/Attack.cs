using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attack")]
public class Attack : ScriptableObject
{
    public int Damage;
    public AttackType Type;
    public AttackCategory Category;
    public int Range;
    public List<string> Dialog;
}

public enum AttackType
{
    Social, Physical, Emotional
}

public enum AttackCategory
{
    Antagonistic, Supporting
}
