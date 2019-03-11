using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField]
	Color _normalColor = Color.white,
	      _highlightedColor = Color.white,
	      _pressedColor = Color.white;

	public Color NormalColor
	{
		get => _normalColor;
		set
		{
			_normalColor = value;
			if (!hovered && !pressed)
			{
				rend.material.color = _normalColor;
			}
		}
	}

	public Color HighlightedColor
	{
		get => _highlightedColor;
		set
		{
			_highlightedColor = value;
			if ((hovered && !pressed) || (!hovered && pressed))
			{
				rend.material.color = _highlightedColor;
			}
		}
	}

	public Color PressedColor
	{
		get => _pressedColor;
		set
		{
			_pressedColor = value;
			if (hovered && pressed)
			{
				rend.material.color = _pressedColor;
			}
		}
	}
	// TODO:
	// public float FadeDuration;

	public event EventHandler OnPointerEnterCallback, OnPointerExitCallback, OnClickCallback;

    [SerializeField]
	bool _clickable = true;
	public bool Clickable
	{
		get => _clickable;
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
