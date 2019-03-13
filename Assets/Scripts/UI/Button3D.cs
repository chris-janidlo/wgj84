using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public event Action<Button3D> OnClickCallback;

    [SerializeField]
	bool _interactable = true;
	public bool Interactable
	{
		get => _interactable;
		set
		{
			if (!value) pressed = false;
			_interactable = value;
			stateChange(value ? state.idle : state.disabled);
		}
	}

	protected enum state
	{
		disabled, idle, hovered, pressed_away, pressed_over
	}

	bool hovered, pressed;

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (!Interactable) return;

		hovered = true;

		stateChange(pressed ? state.pressed_over : state.hovered);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (!Interactable) return;

		hovered = false;

		stateChange(pressed ? state.pressed_away : state.idle);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (!Interactable) return;

		pressed = true;

		stateChange(state.pressed_over);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		if (!Interactable) return;

		if (hovered && pressed)
		{
			Action<Button3D> temp = OnClickCallback;
			if (temp != null)
				temp(this);
		}

		pressed = false;

		// check interactable because OnClickCallback may have changed it
		if (Interactable) stateChange(hovered ? state.hovered : state.idle);
	}

	protected virtual void stateChange (state newValue) {}
}
