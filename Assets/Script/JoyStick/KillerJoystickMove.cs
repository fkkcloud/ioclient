using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KillerJoystickMove : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Image joystickImg;
	private Vector3 inputVector;
	private Vector3 Velocity;

	private void Start(){
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild (0).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		// check if we are hittin within the image
		Vector2 pos = Vector2.zero;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			bgImg.rectTransform
			, ped.position
			, ped.pressEventCamera
			, out pos)) 
		{
			pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x); // normalize x
			pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y); // normalize y

			//Debug.Log (bgImg.rectTransform.pivot);

			float x = (bgImg.rectTransform.pivot.x == 1f) ? pos.x * 2 + 1 : pos.x * 2 - 1;
			float y = (bgImg.rectTransform.pivot.y == 1f) ? pos.y * 2 + 1 : pos.y * 2 - 1;

			Vector3 DesiredInput = new Vector3 (x, y, 0f);
			DesiredInput = (DesiredInput.magnitude > 1f) ? DesiredInput.normalized : DesiredInput;

			inputVector = Vector3.SmoothDamp (inputVector, DesiredInput, ref Velocity, 3f * Time.deltaTime);

			//inputVector = new Vector3 (x, y, 0f);
			//inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;

			//Debug.Log (inputVector);

			// Move Joystick IMG
			joystickImg.rectTransform.anchoredPosition = new Vector3(
				inputVector.x * (bgImg.rectTransform.sizeDelta.x * 0.5f), 
				inputVector.y * (bgImg.rectTransform.sizeDelta.y * 0.5f), 
				0f);
		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector3.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
	}

	public float Horizontal(){
		if (inputVector.x != 0f)
			return inputVector.x;
		else
			return Input.GetAxis ("Horizontal"); // for PC
	}

	public float Vertical(){
		if (inputVector.y != 0f)
			return inputVector.y;
		else
			return Input.GetAxis ("Vertical"); // for PC
	}
}
