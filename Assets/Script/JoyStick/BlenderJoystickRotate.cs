using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlenderJoystickRotate : IOGameBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Image joystickImg;
	private Image blenderRotateIndicator;

	private float stackedYawValue = 0f;

	private Vector2 DragStartPosition;
	private Vector2 PreviousDragPosition;

	[HideInInspector]
	public Vector3 BaseBodyRotation;

	Vector2 JoystickCenterPosition;

	bool IsEnableCameraRotation = false;

	private void Start(){
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild (0).GetComponent<Image> ();
		blenderRotateIndicator = transform.GetChild (1).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		if (!IsEnableCameraRotation)
			return;

		Blender blender = GlobalGameState.PlayerBlenderController.CharacterObject;
		if (blender == null)
			return;

		// check if we are hittin within the image
		Vector2 draggedPos = Vector2.zero;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out draggedPos)) 
		{
			if (draggedPos.y > 0f)
				stackedYawValue -= (PreviousDragPosition.x - draggedPos.x) / Screen.width;
			else
				stackedYawValue += (PreviousDragPosition.x - draggedPos.x) / Screen.width;
			
			if (draggedPos.x > 0f)
				stackedYawValue += (PreviousDragPosition.y - draggedPos.y) / Screen.height;
			else
				stackedYawValue -= (PreviousDragPosition.y - draggedPos.y) / Screen.height;

			// Move Joystick IMG
			joystickImg.rectTransform.anchoredPosition = draggedPos;

			blenderRotateIndicator.rectTransform.rotation = Quaternion.Euler (0f, 0f, -blender.transform.rotation.eulerAngles.y);

			PreviousDragPosition = draggedPos;
		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		Blender blender = GlobalGameState.PlayerBlenderController.CharacterObject;
		if (blender == null)
			return;

		Vector2 pointerDownPos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out pointerDownPos)) 
		{
			DragStartPosition = pointerDownPos;
			BaseBodyRotation = blender.gameObject.transform.rotation.eulerAngles;

			joystickImg.color = new Color (joystickImg.color.r, joystickImg.color.g, joystickImg.color.b, 0.25f);
			//Debug.Log ("PointerDown:" + BaseScreenPosition);

			IsEnableCameraRotation = true;

			PreviousDragPosition = pointerDownPos;
		}
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		stackedYawValue = 0f;

		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
		joystickImg.color = new Color (joystickImg.color.r, joystickImg.color.g, joystickImg.color.b, 0f);

		IsEnableCameraRotation = false;
	}

	public float Yaw(){
		return stackedYawValue;

	}

}
