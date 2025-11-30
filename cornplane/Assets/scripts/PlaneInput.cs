using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneInput : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset actions;

    InputAction pitchAction;
    InputAction rollAction;
    InputAction yawAction;
    InputAction throttleAction;
    InputAction brakeAction;

    [Header("Runtime values")]
    public float pitch;       // -1..1
    public float roll;        // -1..1
    public float yaw;         // -1..1
    public float throttle;    // 0..1
    public bool brake;

    private void Awake()
    {
        var map = actions.FindActionMap("Flight");

        pitchAction = map.FindAction("Pitch");
        rollAction = map.FindAction("Roll");
        yawAction = map.FindAction("Yaw");
        throttleAction = map.FindAction("Throttle");
        brakeAction = map.FindAction("Brake");

        map.Enable();
    }

    private void Update()
    {
        pitch = pitchAction.ReadValue<float>();
        roll = rollAction.ReadValue<float>();
        yaw = yawAction.ReadValue<float>();

        // throttle — интегрируем, т.к. это изменение, а не абсолютное значение
        float throttleDelta = throttleAction.ReadValue<float>() * Time.deltaTime;
        throttle = Mathf.Clamp01(throttle + throttleDelta);

        brake = brakeAction.IsPressed();
    }
}
