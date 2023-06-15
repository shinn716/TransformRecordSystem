using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private float camRotSpeed = 0.5f;

    private Vector3 prevMousePos;
    private Transform mainCamParent;
    private Transform mainCamera;
    private float camMoveSpeed = .015f;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        mainCamParent = Camera.main.transform.parent;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            prevMousePos = Input.mousePosition;

        else if (Input.GetMouseButton(1))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 deltaPos = (mousePos - prevMousePos) * camRotSpeed;
            Vector3 rot = mainCamParent.localEulerAngles;

            while (rot.x > 180f)
                rot.x -= 360f;
            while (rot.x < -180f)
                rot.x += 360f;

            rot.x = Mathf.Clamp(rot.x - deltaPos.y, -89.8f, 89.8f);
            rot.y += deltaPos.x;
            rot.z = 0f;

            mainCamParent.localEulerAngles = rot;
            prevMousePos = mousePos;
        }

        else if (Input.GetMouseButton(2))
            Pan(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            mainCamera.Translate(new Vector3(0, 0, camMoveSpeed));

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            mainCamera.Translate(-new Vector3(0, 0, camMoveSpeed));
    }

    private void Pan(float right, float up)
    {
        mainCamera.Translate(camRotSpeed * right * camMoveSpeed * Vector3.left);
        mainCamera.Translate(camRotSpeed * up * camMoveSpeed * Vector3.up, Space.World);
    }
}
