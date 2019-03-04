using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class Board
{
    Dictionary<BoardSpace, BoardPiece> state;

    public Board (Dictionary<BoardSpace, BoardPiece> startingState)
    {
        state = startingState;
    }

    public BoardSpace TryGetSpace (Vector2Int coordinate)
    {
        return state.Keys.SingleOrDefault(s => s.Position == coordinate);
    }

    public BoardSpace GetSpace (BoardPiece piece)
    {
        // if a piece exists, it must be on a space. if there is no space, that's an issue elsewhere
        return state.Single(x => x.Value == piece).Key;
    }

    public BoardPiece TryGetPiece (Vector2Int coordinate)
    {
        var space = TryGetSpace(coordinate);
        if (space == null)
        {
            return null;
        }
        else
        {
            return TryGetPiece(space);
        }
    }

    public BoardPiece TryGetPiece (BoardSpace space)
    {
        BoardPiece p;
        state.TryGetValue(space, out p);
        return p;
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
                var space = TryGetSpace(point);
                if (space == null) continue;

                bool inRange = (point - center).sqrMagnitude <= range * range;
                bool isValid = (filter != null) ? filter(space) : true;
                Debug.Log(filter);

                if (inRange && isValid)
                {
                    circle.Add(space);
                }
            }
        }

        return circle.ToList();
    }

    public List<BoardSpace> GetMovableSpaces (BoardPiece mover)
    {
        return GetCircle
        (
            GetSpace(mover).Position,
            mover.Speed,
            s => state[s] == null
        );
    }

    public List<BoardPiece> GetAttackablePieces (BoardPiece attacker, AttackCategory category)
    {
        return GetCircle
        (
            GetSpace(attacker).Position,
            attacker.PeekAttack(category).Range,
            s => state[s] != null && state[s].Team != attacker.Team
        )
        .Select(s => state[s]).ToList();
    }

    // TODO: test me
    public CoverData GetCover (BoardPiece target, BoardPiece shooter)
    {
        Vector2Int tpos = GetSpace(target).Position, spos = GetSpace(shooter).Position;
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

        return new CoverData
        {
            LeftCover = TryGetPiece(coverPositions[truemod((octant - 1), 8)]),
            MiddleCover = TryGetPiece(coverPositions[octant]),
            RightCover = TryGetPiece(coverPositions[truemod((octant + 1), 8)])
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
        var oldSpace = GetSpace(mover);
        state[oldSpace] = null;
        state[target] = mover;
    }

    public Board Clone ()
    {
        Dictionary<BoardSpace, BoardPiece> copy = new Dictionary<BoardSpace, BoardPiece>();

        foreach (var s in state.Keys)
        {
            copy.Add(s.Clone(), TryGetPiece(s)?.Clone());
        }

        return new Board(copy);
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
