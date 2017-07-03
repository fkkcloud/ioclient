using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlenderJoystickCamera : IOGameBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Vector3 inputVector;

	Vector2 BaseScreenPosition;

	bool IsEnableCameraRotation = false;

	private void Start(){
		bgImg = GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		if (!IsEnableCameraRotation)
			return;

		// check if we are hittin within the image
		Vector2 draggedPos = Vector2.zero;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out draggedPos)) 
		{

			float xDelta = (draggedPos.x - BaseScreenPosition.x) / bgImg.rectTransform.sizeDelta.x; //yaw - normalized
			float yDelta = (draggedPos.y - BaseScreenPosition.y) / bgImg.rectTransform.sizeDelta.y; //pitch - normalized
			inputVector = new Vector3 (xDelta, yDelta, 0f);
			inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;
		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		Vector2 pointerDownPos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out pointerDownPos)) 
		{
			BaseScreenPosition = new Vector2(pointerDownPos.x, pointerDownPos.y);

			IsEnableCameraRotation = true;
		}
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector3.zero;

		IsEnableCameraRotation = false;
	}

	public float Yaw(){
		return inputVector.x;

	}

	public float Pitch(){
		return inputVector.y;
	}
}
