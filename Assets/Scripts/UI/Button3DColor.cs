using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button3DColor : Button3D
{
	public Renderer Renderer;

	public Color NormalColor = Color.white,
	             HighlightedColor = Color.white,
	             PressedColor = Color.white,
                 DisabledColor = Color.white;
	// TODO:
	// public float FadeDuration;

    void Start ()
    {
        if (Renderer == null) Renderer = GetComponent<Renderer>();
    }

    protected override void stateChange (state newValue)
    {
        Color newColor;

        switch (newValue)
        {
            case state.disabled:
                newColor = DisabledColor;
                break;

            case state.idle:
                newColor = NormalColor;
                break;

            case state.hovered:
            case state.pressed_away:
                newColor = HighlightedColor;
                break;

            case state.pressed_over:
                newColor = PressedColor;
                break;

            default:
                throw new Exception("Unexpected default when switching on " + newValue);
        }

        Renderer.material.color = newColor;
    }
}
