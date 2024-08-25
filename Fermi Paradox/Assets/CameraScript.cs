using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过鼠标和键盘控制摄像机的移动
/// </summary>
/// <remarks>注释者：唐天乙 2021/6/2</remarks>
public class CameraScript : MonoBehaviour {

    [SerializeField] private float moveSpeed;//moveSpeed:相机移动速度
    [SerializeField] private float moveTime;//moveTime：缓冲时间，数值越小缓冲时间越长

    private Vector3 newPos;
    private Transform cameraTrans; //cameraTrans：用于缩放，将相机的trans从local改为global
    [SerializeField] private Vector3 zoomAmount;//zoomAmount:存储每次按缩放件改变YZ的数值值,Y为负数，Z为正数
    private Vector3 newZoom;

    private Vector3 dragStartPos, dragCurrentPos;//dragStartPos：鼠标拖拽的起始点；dragCurrentPos：鼠标拖拽的当前位置

    private void Start()
    {
        newPos = transform.position;

        cameraTrans = transform.GetChild(0);
        newZoom = cameraTrans.localPosition;
    }

    private void Update()
    {
        keyboardMovementInput();
        MouseMovementInput();
    }

    /// <summary>
    /// 通过键盘来使相机进行上下左右的平行移动和相机的缩放；并对缩放和移动限制范围
    /// </summary>s
    private void keyboardMovementInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            newPos += transform.forward * moveSpeed * Time.deltaTime; //摄像机平移向上
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            newPos -= transform.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            newPos += transform.right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            newPos -= transform.right * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))//按E放大功能
            newZoom += zoomAmount * moveSpeed;
        if (Input.GetKey(KeyCode.Q))//按Q缩小功能
            newZoom -= zoomAmount * moveSpeed;

        newPos.x = Mathf.Clamp(newPos.x, -25, 25);//限制移动范围
        newPos.z = Mathf.Clamp(newPos.z, -72, 15);
        transform.position = Vector3.Lerp(transform.position, newPos, moveTime * Time.deltaTime);

        newZoom.y = Mathf.Clamp(newZoom.y, 1, 15);//限制缩放范围
        newZoom.z = Mathf.Clamp(newZoom.z, -15, 1);
        cameraTrans.localPosition = Vector3.Lerp(cameraTrans.localPosition, newZoom, moveTime * Time.deltaTime);
        
    }

    /// <summary>
    /// 通过鼠标左键进行拖拽
    /// </summary>
    private void MouseMovementInput()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))//当鼠标左键按下的一瞬间
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                dragStartPos = ray.GetPoint(distance);//获取鼠标初始位置
            }
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))//当鼠标左键一直按着
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                dragCurrentPos = ray.GetPoint(distance);

                Vector3 difference = dragStartPos - dragCurrentPos;
                newPos = transform.position + difference;
            }
        }

        newZoom += Input.mouseScrollDelta.y * zoomAmount;
    }
}
