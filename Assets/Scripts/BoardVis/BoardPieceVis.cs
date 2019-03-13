using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPieceVis : Button3DColor
{
    public TextAsset Data;

    BoardPiece _properties;
    public BoardPiece Properties
    {
        get
        {
            if (_properties == null)
            {
                _properties = JsonUtility.FromJson<BoardPiece>(Data.text);
            }
            return _properties;
        }
    }

    public BoardSpaceVis CurrentSpaceVis { get; set; }

    void Awake ()
    {
        Properties.HasDied += die;
    }

    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, CurrentSpaceVis.GroundLevel, 0.8f * Time.deltaTime);

        transform.localScale = new Vector3(1, Properties.PercentHealth, 1);
    }

    void die (object sender, System.EventArgs e)
    {
        Properties.HasDied -= die;
        Destroy(gameObject);
    }
}
