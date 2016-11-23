using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class TouchControl : MonoBehaviour {//, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler, IPointerDownHandler {

    Vector2 lastPosition;
    CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
    CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
    public string horizontalAxisName = "Mouse X"; // The name given to the horizontal axis for the cross platform input
    public string verticalAxisName = "Mouse Y"; // The name given to the vertical axis for the cross platform input
    bool isPressed = false;
    float pressedTime = 0;
    bool isDraging = false;
    Touch lastTouch;

    public void OnPointerClick(PointerEventData eventData) {

    }
    public void OnBeginDrag(PointerEventData eventData) {
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        UpdateVirtualAxes(eventData.position - lastPosition);
        lastPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        UpdateVirtualAxes(Vector3.zero);
    }
    public void Update() {
        float widthBoarder = Screen.width * 0.10f;
        float heightBoarder = Screen.height * 0.10f;
        int validTouchCount = 0;
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                Touch touch = Input.GetTouch(i);
                lastTouch = touch;
                if (touch.position.x > widthBoarder && touch.position.x < Screen.width - widthBoarder && touch.position.y > heightBoarder) {
                    validTouchCount++;
                    if (isDraging && lastPosition != null) {
                        UpdateVirtualAxes(new Vector2(touch.position.x, touch.position.y) - lastPosition);
                    }
                    pressedTime += Time.deltaTime;
                    if (pressedTime > 0.1f) {
                        isDraging = true;
                        lastPosition = touch.position;
                    }
                    if (isPressed == false) {
                        isPressed = true;
                    }
                    lastPosition = touch.position;
                }
            }
        }
        if (validTouchCount <= 0) {
            if (isPressed && pressedTime < 0.3f) {
                if ((DateTime.Now - lastClickTime).TotalMilliseconds < 500) {
                    GameLogicHandler.onRightClick(lastTouch.position);
                }
                lastClickTime = DateTime.Now;
            }
            if (isPressed) {
                UpdateVirtualAxes(Vector3.zero);
                pressedTime = 0;
                isDraging = false;
                isPressed = false;
            }
        }
    }
    void OnEnable() {
        CreateVirtualAxes();
    }

    void CreateVirtualAxes() {
        // create new axes based on axes to use
        m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
        CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
        m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
        CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
    }
    void UpdateVirtualAxes(Vector3 value) {
        if (Math.Abs(value.x) < 4 && Math.Abs(value.y) < 4) {
            value = Vector3.zero;
            GameLogicHandler.onLeftClick(lastPosition);
        }
#if UNITY_EDITOR
        value = value * 0.1f;
#else
        value = value * 0.1f;
#endif
        m_HorizontalVirtualAxis.Update(value.x);

        m_VerticalVirtualAxis.Update(value.y);
    }

    DateTime lastClickTime = DateTime.Now;
    public void OnPointerDown(PointerEventData eventData) {
        if ((DateTime.Now - lastClickTime).TotalMilliseconds < 500) {
            GameLogicHandler.onRightClick(eventData.position);
        }
        lastClickTime = DateTime.Now;
    }
}
