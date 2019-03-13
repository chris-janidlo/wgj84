using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class BoardSpaceVis : Button3D
{
    [Tooltip("Used for positioning pieces visually")]
    public float OffsetFromGroundLevel = 0.5f;
    public Color NormalColor, ClickableColor, PressedColor;

    [Header("Movement Parameters")]
    public Vector2 ScaleBounds = new Vector2(0.8f, 1);
    public float RotateAccelMax = 10;
    public float ScaleAccelMax = 0.1f;
    public Vector2 ScaleDeltaBounds = new Vector2(0.99f, 1.01f);
    public float SinMultiplier = 0.15f;
    public float RotateClampNormal = 15, RotateClampHovered = 60, RotateClampClicked = 120;
    public float RotateHoverMult = 600;
    public Vector2 SinPhaseRange = new Vector2(-.5f, .5f);

    public Vector3 GroundLevel => transform.position + Vector3.up * OffsetFromGroundLevel;

    BoardSpace _properties;
    public BoardSpace Properties
    {
        get
        {
            if (_properties == null)
            {
                _properties = new BoardSpace { Position = new Vector2Int((int) transform.position.x, (int) transform.position.z) };
            }
            return _properties;
        }
    }
    
    Vector3 rotateDelta = new Vector3();
    float scaleDelta = 1;
    float rotClamp;

    // sin parameters
    float A, B, C, a, b, c;
    Vector3 startPos;

    Renderer rend;

    void Start ()
    {
        transform.rotation = Quaternion.Euler
        (
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        transform.localScale = Vector3.one * RandomExtra.Range(ScaleBounds);

        rotClamp = RotateClampNormal;

        // set up A, B, and C so that they add up to 1
        A = Random.Range(0f, 1f);
        B = Random.Range(0f, 1 - A);
        C = 1 - A - B;

        a = RandomExtra.Range(SinPhaseRange);
        b = RandomExtra.Range(SinPhaseRange);
        c = RandomExtra.Range(SinPhaseRange);

        startPos = transform.position;

        rend = GetComponent<Renderer>();
    }

    void Update ()
    {
        var dt = Time.deltaTime;

        setLocation();
        setRotation(dt);
        setScale(dt);
    }

    protected override void stateChange (state newValue)
    {
        switch (newValue)
        {
            case state.disabled:
                rend.material.color = NormalColor;
                rotClamp = RotateClampNormal;
                break;

            case state.idle:
                rend.material.color = ClickableColor;
                rotClamp = RotateClampNormal;
                break;

            case state.hovered:
                rotClamp = RotateClampHovered;
                rotateDelta *= RotateHoverMult;
                break;

            case state.pressed_away:
                rend.material.color = ClickableColor;
                rotClamp = RotateClampHovered;
                break;

            case state.pressed_over:
                rend.material.color = PressedColor;
                rotClamp = RotateClampClicked;
                rotateDelta *= RotateHoverMult;
                break;
        }
    }

    void setLocation ()
    {
        var t = Time.time;

        transform.position = startPos + Vector3.up * SinMultiplier *
            (A * Mathf.Sin(a * t) + B * Mathf.Sin(b * t) + C * Mathf.Sin(c * t));
    }

    void setRotation (float dt)
    {
        System.Func<float> newDeltaRot = () => Random.Range(-RotateAccelMax, RotateAccelMax) * dt;

        rotateDelta = new Vector3
        (
            Mathf.Clamp(rotateDelta.x + newDeltaRot(), -rotClamp, rotClamp),
            Mathf.Clamp(rotateDelta.y + newDeltaRot(), -rotClamp, rotClamp),
            Mathf.Clamp(rotateDelta.z + newDeltaRot(), -rotClamp, rotClamp)
        );

        transform.Rotate(rotateDelta * dt);
    }

    void setScale (float dt)
    {
        scaleDelta += (RandomExtra.Chance(.5f) ? -ScaleAccelMax : ScaleAccelMax) * dt;
        scaleDelta = Mathf.Clamp(scaleDelta, ScaleDeltaBounds.x, ScaleDeltaBounds.y); 

        var next = transform.localScale * scaleDelta;

        transform.localScale += (next - transform.localScale) * dt;
        transform.localScale = new Vector3
        (
            Mathf.Clamp(transform.localScale.x, ScaleBounds.x, ScaleBounds.y),
            Mathf.Clamp(transform.localScale.y, ScaleBounds.x, ScaleBounds.y),
            Mathf.Clamp(transform.localScale.z, ScaleBounds.x, ScaleBounds.y)
        );
    }
}
