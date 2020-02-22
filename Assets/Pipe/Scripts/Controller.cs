using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    public float speed = 20;


    public int currentCount = 0;
    private float timeout = .4f;
    private float timeCount = 0;

    void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
    }
    void Update()
    {
        if (currentCount > 0)
            timeCount += Time.deltaTime;

        if (timeCount >= timeout)
        {
            SetOut();
        }
        if (Input.GetMouseButton(0))
        {
            _mouseOffset = ((Vector3.right* Input.GetAxis("Mouse X") * speed) - _mouseReference);
            _rotation.z = (_mouseOffset.x + _mouseOffset.y) * _sensitivity;
            transform.Rotate(_rotation);
        }
        if(Input.GetMouseButtonDown(0))
            currentCount++;
    }
    public void SetOut()
    {
        timeCount = 0;
        currentCount = 0;
    }

}
