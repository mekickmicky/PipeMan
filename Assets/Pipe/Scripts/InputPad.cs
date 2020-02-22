using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputPad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Input")]
    public bool isDown = false;
    
    public bool useDrag = false;
    public bool useTap = false;


    [Header("Drag")]
    public bool hasHandle = false;
    public DirectInput enumDirectInput = DirectInput.X;
    public enum DirectInput { X, Z, XZ }
    public Vector3 inputVector;
    public Vector2 inputPoint;
    public Vector2 offsetPoint;

    private Image panelInput;
    private Image joystickImg;

    public bool isOneClick = false;
    //public bool forSteer;

    public Transform objRot;
    public float currentRotX = 0;
    public float speedRot = 20;

    [Header("TapFn")]
    public int countTap;
    public int currentCountTap = 0;
    public float timeOutTap;
    public float tapDelay;
    public bool isTap = false;



    private void Start()
    {
        if (!useDrag)
            return;
        panelInput = GetComponent<Image>();
        if (transform.childCount > 0)
        {
            joystickImg = transform.GetChild(0).GetComponent<Image>();
        }
        //if (forSteer && enumDirectInput == DirectInput.X)
        //{
        //CameraFollow.Instance._InputPadSteer = GetComponent<InputDragPad>();
        //}
    }
    //private void FixedUpdate()
    //{
    //    if(GameManagerGameplay.Instance.isPlatformMobile && Input.touchCount == 0 && isDown)
    //    {
    //        isDown = false;

    //    }
    //}

    public virtual void OnPointerDown(PointerEventData ped)
    {
        isDown = true;
        //OnDrag(ped);

        if (useDrag)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panelInput.rectTransform, ped.position, ped.pressEventCamera, out inputPoint))
            {
                inputPoint.x = (inputPoint.x / panelInput.rectTransform.sizeDelta.x);
                inputPoint.y = (inputPoint.y / panelInput.rectTransform.sizeDelta.y);
            }
        }
        

        if (useTap)
        {
            isOneClick = true;
            currentCountTap++;
            if (countTap <= currentCountTap)
            {
                TapResponse();
            }
        }

    }
    private void TapResponse()
    {
        GameManager.instance.ball.Jump();

        isTap = true;
        StopAllCoroutines();
        StartCoroutine(StartCountOutIsTap(tapDelay));
        SetZeroValue();
        isOneClick = false;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        //isDown = true;

        if (useDrag)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panelInput.rectTransform, ped.position, ped.pressEventCamera, out pos))
            {
                pos.x = (pos.x / panelInput.rectTransform.sizeDelta.x);
                pos.y = (pos.y / panelInput.rectTransform.sizeDelta.y);

                switch (enumDirectInput)
                {
                    case DirectInput.X:
                        inputVector.x = pos.x * 2;
                        break;
                    case DirectInput.Z:
                        inputVector.z = pos.y * 2;
                        break;
                    case DirectInput.XZ:
                        inputVector = new Vector3(pos.x * 2, 0, pos.y * 2);
                        break;
                    default:
                        break;
                }
                offsetPoint = pos - inputPoint;
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

                if (objRot)
                    objRot.eulerAngles = new Vector3(0, 0, currentRotX+(offsetPoint.x * speedRot));

                if (hasHandle && joystickImg != null)
                {
                    switch (enumDirectInput)
                    {
                        case DirectInput.X:
                            joystickImg.rectTransform.anchoredPosition =
                                new Vector3(inputVector.x * (panelInput.rectTransform.sizeDelta.x / 2), 0);
                            break;
                        case DirectInput.Z:
                            joystickImg.rectTransform.anchoredPosition =
                                new Vector3(0, inputVector.z * (panelInput.rectTransform.sizeDelta.y / 2));
                            break;
                        case DirectInput.XZ:
                            joystickImg.rectTransform.anchoredPosition =
                                new Vector3(inputVector.x * (panelInput.rectTransform.sizeDelta.x / 2), inputVector.z * (panelInput.rectTransform.sizeDelta.y / 2));
                            break;
                        default:
                            break;
                    }
                }
            }
        
        
            if (!useTap)
                return;
            isOneClick = false;
        }
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        isDown = false;
        currentRotX = objRot.eulerAngles.z;

        if (useDrag)
        {
            SetZeroValue();
        }

        if (useTap)
        {
            if (!isTap)
                StartCoroutine(StartCountOutTap(timeOutTap));
        }

    }
    public void SetZeroValue()
    {
        offsetPoint = Vector2.zero;
        inputVector = Vector3.zero;
        if (joystickImg != null)
            joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    IEnumerator StartCountOutTap(float timeOut)
    {
        yield return new WaitForSeconds(timeOut);
        isOneClick = false;
        currentCountTap = 0;
    }
    IEnumerator StartCountOutIsTap(float timeOut)
    {
        currentCountTap = 0;
        yield return new WaitForSeconds(timeOut);
        isOneClick = false;
        isTap = false;
    }
    public void ReTapPad()
    {
        currentCountTap = 0;
        isTap = false;
    }

}
