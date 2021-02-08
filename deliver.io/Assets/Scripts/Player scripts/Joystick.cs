using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Joystick : MonoBehaviour
{
    const float rectValue = 300;

    Vector2 startPos = Vector2.zero, endPos = Vector2.zero;

    [SerializeField]GameObject bgImg = null, joystick = null;
    RectTransform bgRect = null, joystickRect = null;
    Image joystickImg = null;
    [SerializeField] RectTransform minImgRect = null;

    [SerializeField] [Range(0, 1)] float TravelDistance = 1f;
    float joyRadius;

    bool HaveInput = false;

    public static float InputDistance { get; private set; }
    public static float angle { get; private set; }

    private void Start()
    {
        bgImg.SetActive(false);

        bgRect = bgImg.GetComponent<RectTransform>();
        joystickRect = joystick.GetComponent<RectTransform>();
        joystickImg = joystick.GetComponent<Image>();

        //rectValue = slide.value;
        bgRect.sizeDelta = Vector2.one * rectValue;
        joyRadius = rectValue * .5f;

        joystickRect.sizeDelta = Vector2.one * rectValue * 0.33f;

        Vector2 sizeRect = bgRect.sizeDelta * TravelDistance * .5f;
        minImgRect.sizeDelta = new Vector2(sizeRect.x + (joystickRect.sizeDelta.x), sizeRect.y + (joystickRect.sizeDelta.y));
    }

    private void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject() == false && GameManager.gameState == GameState.InGame)
        {
#if UNITY_EDITOR
            getMouseInput();
#endif
#if UNITY_ANDROID
            if(Input.touchCount > 0)
                getTouchInput();
#endif
        }
            getMouseInput();
    }

    private void getTouchInput()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            inputStart(touch.position);
        }
        else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            if (!HaveInput)
                inputStart(touch.position);

            endPos = touch.position;

            setJoyPos(touch.position);
        }
        else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            bgImg.SetActive(false);
        }
       
    }

    private void getMouseInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            inputStart(Input.mousePosition);
        }
        else if(Input.GetMouseButton(0))
        {
            if (!HaveInput)
                inputStart(Input.mousePosition);

            endPos = Input.mousePosition;

            setJoyPos(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            HaveInput = false;
            bgImg.SetActive(false);
        }
    }

    void inputStart(Vector2 startPosition)
    {
        startPos = startPosition;

        bgImg.SetActive(true);

        bgRect.position = startPos;
        joystickRect.position = startPos;

        HaveInput = true;
    }

    void setJoyPos(Vector2 cursorPos)
    {
        Vector2 pos = bgRect.InverseTransformPoint(cursorPos);

        float magni = pos.magnitude;
        magni = Mathf.Clamp(magni,0, joyRadius * TravelDistance);

        pos = pos.normalized * magni;

        joystickRect.localPosition = Vector2.Lerp(joystickRect.localPosition, pos, .5f);

        InputDistance = magni / (joyRadius * TravelDistance);

        Vector2 dir = joystickRect.localPosition;
        dir.Normalize();

        angle = Vector2.Angle(Vector2.up, dir);
        angle = dir.x > 0 ? angle : 360 - angle;

    }

    public bool minDisMoved(float MinDis)
    {
        if (!HaveInput)
            return false;

        if (InputDistance >= MinDis)
        {
            return true;
        }
        else
            return false;
    }
}
