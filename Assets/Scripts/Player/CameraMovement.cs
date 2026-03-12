using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private PlayerControls inputs;

    public float sensX;
    public float sensY;
    public float sensControllerX;
    public float sensControllerY;

    public float angle = 4f;
    public float angleSpeed = 10f;

    public Transform oriantation;
    [SerializeField] private Player player;

    float xRotation;
    float yRotation;

    public bool lockOn;
    public bool rotatingGrabbedObject;
    public Vector3 lastLockOnTrans;
    private Vector2 camMove, camMoveController;

    #region Controls

    private void Awake()
    {
        inputs = new PlayerControls();

        inputs.Controls.CameraMove.performed += ctx => camMove = ctx.ReadValue<Vector2>();
        inputs.Controls.CameraMove.canceled += ctx => camMove = Vector2.zero;


        inputs.Controls.CameraMoveController.performed += ctx => camMoveController = ctx.ReadValue<Vector2>();
        inputs.Controls.CameraMoveController.canceled += ctx => camMoveController = Vector2.zero;
    }

    private void OnEnable()
    {
        inputs.Controls.Enable();
    }

    private void OnDisable()
    {
        inputs.Controls.Disable();
    }

    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        if (!rotatingGrabbedObject)
        {
            if (!lockOn)
            {
                if (lastLockOnTrans != Vector3.zero)
                {
                    yRotation = lastLockOnTrans.y;
                    xRotation = lastLockOnTrans.x;
                    lastLockOnTrans = Vector3.zero;
                }

                float mouseX = (camMove.x * sensX * Time.deltaTime) + (camMoveController.x * sensControllerX * Time.deltaTime);
                float mouseY = (camMove.y * sensY * Time.deltaTime) + (camMoveController.y * sensControllerY * Time.deltaTime);

                yRotation += mouseX;
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                oriantation.rotation = Quaternion.Euler(0, yRotation, 0);
            }
            else
            {
                lastLockOnTrans = transform.eulerAngles;
            }
        }      
    }
}
