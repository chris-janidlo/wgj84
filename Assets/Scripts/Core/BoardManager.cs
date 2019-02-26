using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class BoardManager : Singleton<BoardManager>
{
    public Dictionary<Vector2Int, BoardSpace> SpaceLookup => spaces.ToDictionary(s => s.Position);
    
    List<BoardSpace> spaces;

    void Start ()
    {
        SingletonSetInstance(this, true);
        spaces = GetComponentsInChildren<BoardSpace>().ToList();
    }

    // returns a list of all spaces within range of center for which filter is true
    // TODO: test me
    public List<BoardSpace> GetCircle (Vector2Int center, int range, Func<BoardSpace, bool> filter = null)
    {
        HashSet<BoardSpace> circle = new HashSet<BoardSpace>();

        for (int x = center.x - range; x <= center.x + range; x++)
        {
            for (int y = center.y - range; y <= center.y + range; y++)
            {
                Vector2Int point = new Vector2Int(x, y);
                BoardSpace space;
                SpaceLookup.TryGetValue(point, out space);
                if (space == null) continue;

                bool inRange = (point - center).sqrMagnitude <= range * range;
                bool isValid = (filter != null) ? filter(space) : true;

                if (inRange && isValid)
                {
                    circle.Add(space);
                }
            }
        }

        return circle.ToList();
    }

    // TODO: test me
    public CoverData GetCover (BoardPiece target, BoardPiece shooter)
    {
        Vector2Int tpos = target.Position, spos = shooter.Position;
        Vector2 coverDir = ((Vector2) spos - tpos);
        int angle = (int) Vector2.Angle(Vector2.up, coverDir);

        // regular octant as defined in geometry except it's offset by 22.5
        // (we want to basically round the angle to the nearest 45 degree axis)
        int octant = truemod(Mathf.FloorToInt((angle - 22.5f) / 45f), 8);

        // clockwise from the top
        // every potential space that could provide cover
        Vector2Int[] coverPositions = 
        {
            Vector2Int.up,
            new Vector2Int(1, 1),
            Vector2Int.right,
            new Vector2Int(1, -1),
            Vector2Int.down,
            new Vector2Int(-1, -1),
            Vector2Int.left,
            new Vector2Int(-1, 1)
        };

        BoardSpace leftSpace, middleSpace, rightSpace;

        SpaceLookup.TryGetValue(coverPositions[truemod((octant - 1), 8)], out leftSpace);
        SpaceLookup.TryGetValue(coverPositions[octant], out middleSpace);
        SpaceLookup.TryGetValue(coverPositions[truemod((octant + 1), 8)], out rightSpace);

        return new CoverData
        {
            LeftCover = leftSpace?.CurrentPiece,
            MiddleCover = middleSpace?.CurrentPiece,
            RightCover = rightSpace?.CurrentPiece
        };
    }

    // returns true if hit, false if miss
    public bool DoAttack (AttackCategory category, BoardPiece target, BoardPiece shooter)
    {
        shooter.HasAttacked = true;
        var attack = shooter.PopAttack(category);
        var coverHit = GetCover(target, shooter).TryHit();

        if (coverHit == null)
        {
            target.Health -= attack.Damage;
            return true;
        }
        else
        {
            coverHit.Health -= attack.Damage;
            return false;
        }
    }

    public void DoMove (BoardPiece mover, BoardSpace target)
    {
        mover.HasMoved = true;
        mover.Space.CurrentPiece = null;
        mover.Space = target;
        target.CurrentPiece = mover;
    }

    // works as expected on negative numbers
    int truemod (int a, int b)
    {
        return (int) Mathf.Repeat(a, b);
    }
}

public struct CoverData
{
    public BoardPiece LeftCover;
    public BoardPiece MiddleCover;
    public BoardPiece RightCover;
    
    public float OverallChance => leftChance + middleChance + rightChance;

    float leftChance => (LeftCover?.PercentHealth ?? 0) / 2;
    float middleChance => (MiddleCover?.PercentHealth ?? 0) / 2;
    float rightChance => (RightCover?.PercentHealth ?? 0) / 2;

    // returns the piece that is hit from cover fire, if there is one, otherwise null
    public BoardPiece TryHit ()
    {
        // check middle first since we like it more
        if (RandomExtra.Chance(middleChance))
        {
            return MiddleCover;
        }
        if (RandomExtra.Chance(leftChance))
        {
            return LeftCover;
        }
        if (RandomExtra.Chance(rightChance))
        {
            return RightCover;
        }
        return null;
    }
}
