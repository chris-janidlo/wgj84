using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace : MonoBehaviour
{
    public Vector2Int Position;
    [Tooltip("Used for positioning pieces visually")]
    public Vector3 OffsetFromGroundLevel;

    [SerializeField]
    BoardPiece _currentPiece;
    public BoardPiece CurrentPiece
    {
        get => _currentPiece;
        set
        {
            if (_currentPiece != null && value != null)
            {
                throw new InvalidOperationException("Board space is already occupied");
            }

            _currentPiece = value;

            if (value != null)
            {
                _currentPiece.Space = this;
            }
        }
    }

    public Vector3 GroundLevel => transform.position + OffsetFromGroundLevel;
}
