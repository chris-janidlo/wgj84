using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public Color NormalColor = Color.white,
	             HighlightedColor = Color.white,
				 PressedColor = Color.white;
	// TODO:
	// public float FadeDuration;

	public event EventHandler OnPointerEnterCallback, OnPointerExitCallback, OnClickCallback;

    [SerializeField]
	bool _clickable = true;
	public bool Clickable
	{
		get
		{
			return _clickable;
		}
		set
		{
			if (!value) pressed = false;
			_clickable = value;
		}
	}

	bool hovered, pressed;

    Renderer rend;

    protected virtual void Awake ()
    {
        rend = GetComponent<Renderer>();
		rend.material.color = NormalColor;
    }

	public void OnPointerEnter (PointerEventData eventData)
	{
		hovered = true;
		rend.material.color = pressed ? PressedColor : HighlightedColor;

        threadSafeCallback(OnPointerEnterCallback);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		hovered = false;
		rend.material.color = pressed ? HighlightedColor : NormalColor;

        threadSafeCallback(OnPointerExitCallback);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!Clickable) return;

		pressed = true;
		rend.material.color = PressedColor;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!Clickable) return;

		pressed = false;
		rend.material.color = NormalColor;

		if (hovered) threadSafeCallback(OnClickCallback);
	}

    void threadSafeCallback (EventHandler callback)
    {
        EventHandler temp = callback;
        if (temp != null) temp(this, null);
    }
}
