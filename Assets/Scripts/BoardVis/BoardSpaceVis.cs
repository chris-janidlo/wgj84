using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceVis : MonoBehaviour
{
    public BoardSpace Properties;
    [Tooltip("Used for positioning pieces visually")]
    public Vector3 OffsetFromGroundLevel;
    public Vector3 GroundLevel => transform.position + OffsetFromGroundLevel;
}
