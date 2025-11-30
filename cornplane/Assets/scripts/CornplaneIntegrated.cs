using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CornplaneIntegrated : MonoBehaviour
{
    [Header("������������")]
    [SerializeField] private GliderWing _mainWing;
    [SerializeField] private GliderWing _tailWing;

    [Header("���������")]
    [SerializeField] private PlaneEngine _engine;

    [Header("����������")]
    [SerializeField] private PlaneFlightController _flightController;

    [Header("����������� ����������")]
    [SerializeField] private Transform _elevator;
    [SerializeField] private Transform _rudder;
    [SerializeField] private float _maxSurfaceAngle = 25f;

    private Rigidbody _rb;
    private AircraftHUD _hud;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _hud = FindObjectOfType<AircraftHUD>();

        // ������� ����� ���� ���� ��� ������������
        _rb.centerOfMass = Vector3.down * 0.3f;
    }

    private void Update()
    {
        UpdateControlSurfaces();
        UpdateHUD();
    }

    private void UpdateControlSurfaces()
    {
        if (_elevator && _flightController)
        {
            float pitchInput = _flightController.GetPitchInput();
            _elevator.localRotation = Quaternion.Euler(pitchInput * _maxSurfaceAngle, 0, 0);
        }

        if (_rudder && _flightController)
        {
            float yawInput = _flightController.GetYawInput();
            _rudder.localRotation = Quaternion.Euler(0, yawInput * _maxSurfaceAngle, 0);
        }
    }

    private void UpdateHUD()
    {
        if (_hud)
        {
            _hud.UpdateTelemetry(
                airSpeed: _rb.linearVelocity.magnitude,
                altitude: transform.position.y,
                pitch: NormalizeAngle(transform.eulerAngles.x),
                roll: NormalizeAngle(transform.eulerAngles.z),
                heading: NormalizeAngle(transform.eulerAngles.y),
                throttle: _engine ? _engine.Throttle01 : 0f
            );
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
}
