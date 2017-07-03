using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KillerJoystickCamera : IOGameBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Image joystickImg;
	private Vector3 inputVector;

	[HideInInspector]
	public Vector3 BaseHeadLocalRotation;
	public Vector3 BaseBodyRotation;

	Vector2 BaseScreenPosition;

	bool IsEnableCameraRotation = false;

	private void Start(){
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild (0).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		if (!IsEnableCameraRotation)
			return;

		Killer killer = GlobalGameState.PlayerKillerController.CharacterObject;
		if (killer == null)
			return;

		// check if we are hittin within the image
		Vector2 draggedPos = Vector2.zero;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out draggedPos)) 
		{
			/* OLD METHOD
			float xDelta = (draggedPos.x - BaseScreenPosition.x) / Screen.width; //yaw - normalized
			float yDelta = (BaseScreenPosition.y - draggedPos.y) / Screen.height; //pitch - normalized

			inputVector = new Vector3 (xDelta, yDelta, 0f);

			//Debug.Log (BaseScreenPosition + " VS " + draggedPos);
			//Debug.Log ("Delta:" + inputVector);

			// Move Joystick IMG
			joystickImg.rectTransform.anchoredPosition = draggedPos;
			*/

			float xDelta = (draggedPos.x - BaseScreenPosition.x) / bgImg.rectTransform.sizeDelta.x; //yaw - normalized
			float yDelta = (draggedPos.y - BaseScreenPosition.y) / bgImg.rectTransform.sizeDelta.y; //pitch - normalized
			inputVector = new Vector3 (xDelta, yDelta, 0f);
			inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;

			joystickImg.rectTransform.anchoredPosition = new Vector3(
				inputVector.x * (bgImg.rectTransform.sizeDelta.x * 0.5f), 
				inputVector.y * (bgImg.rectTransform.sizeDelta.y * 0.5f), 
				0f);
		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		Killer killer = GlobalGameState.PlayerKillerController.CharacterObject;
		if (killer == null)
			return;

		Vector2 pointerDownPos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out pointerDownPos)) 
		{
			BaseScreenPosition = new Vector2(pointerDownPos.x, pointerDownPos.y);

			BaseHeadLocalRotation = killer.HeadTransform.localRotation.eulerAngles;
			BaseBodyRotation = killer.gameObject.transform.rotation.eulerAngles;

			joystickImg.color = new Color (joystickImg.color.r, joystickImg.color.g, joystickImg.color.b, 0.25f);
			//Debug.Log ("PointerDown:" + BaseScreenPosition);

			IsEnableCameraRotation = true;
		}
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector3.zero;

		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
		joystickImg.color = new Color (joystickImg.color.r, joystickImg.color.g, joystickImg.color.b, 0f);

		IsEnableCameraRotation = false;
	}

	public float Yaw(){
		return inputVector.x;

	}

	public float Pitch(){
		return inputVector.y;
	}
}
