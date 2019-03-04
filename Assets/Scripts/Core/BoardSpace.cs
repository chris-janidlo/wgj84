using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardSpace
{
    public Vector2Int Position;

    public BoardSpace Clone ()
    {
        return new BoardSpace
        {
            Position = Position
        };
    }
}
