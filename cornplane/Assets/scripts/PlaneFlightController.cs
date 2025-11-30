using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlaneFlightController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [Header("Макс. угловые скорости")]
    [SerializeField] private Vector3 _maxRateDeg = new Vector3(90, 90, 120);
    [SerializeField] private Vector3 _kp = new Vector3(3, 2, 3);
    [SerializeField] private Vector3 _kd = new Vector3(0.8f, 0.6f, 0.9f);
    [SerializeField] private Vector3 _maxTorque = new Vector3(30, 25, 35);
    [SerializeField] private float _deadZone = 0.05f;

    [Header("Удержание положения")]
    [SerializeField] private Vector2 _attHoldKp = new Vector2(2, 2);
    [SerializeField] private float _attHoldMaxRate = 45f;

    private Rigidbody _rb;
    private InputAction _yaw, _pitch, _roll, _hold;
    private bool _isHolding;
    public Vector3 CurrentRateCommand { get; private set; }
    public bool IsHoldingAttitude => _isHolding;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        var map = _playerInput.actions.FindActionMap("Aircraft");
        _pitch = map.FindAction("PitchUp");
        _roll = map.FindAction("RollRight");
        _yaw = map.FindAction("YawRight");
        _hold = map.FindAction("HoldAttribute");
    }

    private void OnEnable()
    {
        _pitch?.Enable();
        _roll?.Enable();
        _yaw?.Enable();
        _hold?.Enable();
    }

    private void OnDisable()
    {
        _pitch?.Disable();
        _roll?.Disable();
        _yaw?.Disable();
        _hold?.Disable();
    }

    private void FixedUpdate()
    {
        Vector3 omega = _rb.angularVelocity;
        Vector3 omegaBody = transform.InverseTransformDirection(omega);

        CurrentRateCommand = ReadRateCommandDeg();
        Vector3 rateCmdDeg = _isHolding ? GenerateHoldRateDeg() : CurrentRateCommand;

        ApplyPDControl(rateCmdDeg, omegaBody);
    }

    private void ApplyPDControl(Vector3 rateCmdDeg, Vector3 omegaBody)
    {
        Vector3 rateCmdRad = rateCmdDeg * Mathf.Deg2Rad;
        Vector3 omegaRad = omegaBody;

        Vector3 error = rateCmdRad - omegaRad;
        Vector3 torque = new Vector3(
            error.x * _kp.x - omegaRad.x * _kd.x,
            error.y * _kp.y - omegaRad.y * _kd.y,
            error.z * _kp.z - omegaRad.z * _kd.z
        );

        torque = Vector3.Min(torque, _maxTorque);
        torque = Vector3.Max(torque, -_maxTorque);

        _rb.AddRelativeTorque(torque, ForceMode.Force);
    }

    private Vector3 GenerateHoldRateDeg()
    {
        var (pitch, roll) = GetLocalPitchRollDeg();
        float pitchRate = Mathf.Clamp(-pitch * _attHoldKp.x, -_attHoldMaxRate, _attHoldMaxRate);
        float rollRate = Mathf.Clamp(-roll * _attHoldKp.y, -_attHoldMaxRate, _attHoldMaxRate);
        return new Vector3(pitchRate, 0, rollRate);
    }

    private (float pitch, float roll) GetLocalPitchRollDeg()
    {
        Vector3 e = transform.localEulerAngles;
        return (NormalizeAngle(e.x), NormalizeAngle(e.z));
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }

    private Vector3 ReadRateCommandDeg()
    {
        float uPitch = _pitch.ReadValue<float>();
        float uRoll = _roll.ReadValue<float>();
        float uYaw = _yaw.ReadValue<float>();

        if (Mathf.Abs(uPitch) < _deadZone) uPitch = 0f;
        if (Mathf.Abs(uRoll) < _deadZone) uRoll = 0f;
        if (Mathf.Abs(uYaw) < _deadZone) uYaw = 0f;

        return new Vector3(uPitch * _maxRateDeg.x, uYaw * _maxRateDeg.y, uRoll * _maxRateDeg.z);
    }

    public void SetHoldMode(bool hold) => _isHolding = hold;
    public float GetPitchInput() => _pitch.ReadValue<float>();
    public float GetRollInput() => _roll.ReadValue<float>();
    public float GetYawInput() => _yaw.ReadValue<float>();
}
