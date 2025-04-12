using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public InputActionProperty leftTriggerPress;
    public InputActionProperty rightTriggerPress;
    public InputActionProperty thumbstickAction;
    public Transform leftController;
    public Transform rightController;
    public Transform splats;
    public Transform Camera;
    public float rotationSpeed = 90f; // Degrees per second

    private Vector3 offset;
    private Transform MovingController;
    private int status;
    private float initialDistance;
    private Vector3 initialScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ControllerStatus();
        if (status == 1)
            MoveCamera();
        else if (status == 2)
        {
            ScaleScene();
            RotateScene();
        }

    }

    void MoveCamera()
    {
        Camera.position = MovingController.position + offset;
    }

    void StartScaling()
    {
        if (leftController != null && rightController != null)
        {
            initialDistance = Vector3.Distance(leftController.position, rightController.position);
            initialScale = splats.localScale;
        }
    }

    void ScaleScene()
    {
        float currentDistance = Vector3.Distance(leftController.position, rightController.position);
        if (Mathf.Approximately(initialDistance, 0)) return;

        float scaleFactor = currentDistance / initialDistance;
        splats.localScale = initialScale * scaleFactor;
    }

    void RotateScene()
    {
        Vector2 thumbstickInput = thumbstickAction.action.ReadValue<Vector2>();

        float inputX = thumbstickInput.x;

        // Rotate object around Y-axis based on thumbstick X
        float rotationY = inputX * rotationSpeed * Time.deltaTime;
        splats.Rotate(Vector3.up, rotationY, Space.World);
    }

    // 0: not controller pressed, 1: left pressed or right pressed, 2: both pressed
    void ControllerStatus()
    {
        float left = leftTriggerPress.action.ReadValue<float>();
        float right = rightTriggerPress.action.ReadValue<float>();

        if (left < 0.5f && right < 0.5f && status !=0)
        {
            status = 0;
        }else if (left > 0.5f && right < 0.5f && status !=1)
        {
            offset = Camera.position - leftController.position;
            MovingController = leftController;
            status = 1;
        }
        else if(left > 0.5f && right > 0.5f && status != 2)
        {
            StartScaling();
            status = 2;
        }
    }

    void OnEnable()
    {
        leftTriggerPress.action.Enable();
        rightTriggerPress.action.Enable();
    }

    void OnDisable()
    {
        leftTriggerPress.action.Disable();
        rightTriggerPress.action.Disable();
    }

}
