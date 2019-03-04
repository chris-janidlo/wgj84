using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPieceVis : Button3D
{
    public BoardPiece Properties;

    BoardSpaceVis currentSpaceVis => GetComponentInParent<BoardSpaceVis>();

    protected override void Awake ()
    {
        base.Awake();
        Properties.HasDied += die;
    }

    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, currentSpaceVis.GroundLevel, 0.8f * Time.deltaTime);

        transform.localScale = new Vector3(1, Properties.PercentHealth, 1);

        Clickable = !(Properties.Team == Team.AI || (Properties.HasAttacked && Properties.HasMoved));
    }

    void die (object sender, System.EventArgs e)
    {
        Properties.HasDied -= die;
        Destroy(gameObject);
    }
}
