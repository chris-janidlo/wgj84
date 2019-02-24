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
                BoardSpace space = SpaceLookup[point];

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
    public float CoverAmount (BoardPiece target, BoardPiece shooter)
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

        float leftCover = leftSpace?.CurrentPiece?.PercentHealth ?? 0;
        float middleCover = middleSpace?.CurrentPiece?.PercentHealth ?? 0;
        float rightCover = rightSpace?.CurrentPiece?.PercentHealth ?? 0;

        return Mathf.Max(leftCover / 2, middleCover, rightCover / 2);
    }

    // works as expected on negative numbers
    int truemod (int a, int b)
    {
        return (int) Mathf.Repeat(a, b);
    }
}
