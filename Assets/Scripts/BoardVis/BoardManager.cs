using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class BoardManager : Singleton<BoardManager>
{
    public Board Board;

    public List<BoardPieceVis> Pieces => GetComponentsInChildren<BoardPieceVis>().ToList();
    public List<BoardSpaceVis> Spaces => GetComponentsInChildren<BoardSpaceVis>().ToList();

    void Awake ()
    {
        SingletonSetInstance(this, true);

        Dictionary<BoardSpace, BoardPiece> state = new Dictionary<BoardSpace, BoardPiece>();

        foreach (var s in Spaces)
        {
            var p = s.GetComponentInChildren<BoardPieceVis>();
            state.Add(s.Properties, p?.Properties);
        }

        Board = new Board(state);
    }

    public BoardPieceVis GetPieceVis (BoardPiece properties)
    {
        return Pieces.Single(p => p.Properties == properties);
    }

    public BoardSpaceVis GetSpaceVis (BoardSpace properties)
    {
        return Spaces.Single(s => s.Properties == properties);
    }

    public List<BoardSpaceVis> GetMoveableSpaceVis (BoardPieceVis mover)
    {
        return Board.GetMovableSpaces(mover.Properties).Select(s => GetSpaceVis(s)).ToList();
    }

    public List<BoardPieceVis> GetAttackablePieceVis (BoardPieceVis attacker, AttackCategory category)
    {
        return Board.GetAttackablePieces(attacker.Properties, category).Select(p => GetPieceVis(p)).ToList();
    }

    public void DoMove (BoardPieceVis mover, BoardSpaceVis target)
    {
        Board.DoMove(mover.Properties, target.Properties);
        mover.transform.parent = target.transform;
    }

    public void DoAttack (AttackCategory category, BoardPieceVis target, BoardPieceVis shooter)
    {
        Board.DoAttack(category, target.Properties, shooter.Properties);
    }
}
