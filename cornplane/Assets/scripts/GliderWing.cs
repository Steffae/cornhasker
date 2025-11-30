using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GliderWing : MonoBehaviour
{
    [Header("јтмосфера")]
    [SerializeField] private float _airDensity = 1.225f;

    [Header("—сылка на крыло")]
    [SerializeField] private Transform _wingCp;

    [Header("јэродинамика крыла")]
    [SerializeField] private float _wingArea = 1.5f;
    [SerializeField] private float _aspectRatio = 8f;
    [SerializeField] private float _oswaldEfficiency = 0.85f;
    [SerializeField] private float _cdo = 0.02f;
    [SerializeField] private float _clAlpha = 5.5f;
    [SerializeField] private float _alphaLimitDeg = 18f;

    private Rigidbody _rb;
    private Vector3 _vPoint;
    private float _speed, _alphaRad, _cl, _cd, _qDyn, _lift, _drag;

    public float AlphaDeg => Mathf.Rad2Deg * _alphaRad;
    public float LiftForce => _lift;
    public float DragForce => _drag;
    public float DynamicPressure => _qDyn;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (!_wingCp) return;

        _vPoint = _rb.GetPointVelocity(_wingCp.position);
        _speed = _vPoint.magnitude;
        if (_speed < 0.01f) return;

        Vector3 flow = -_vPoint.normalized;
        Vector3 xChord = _wingCp.forward;
        Vector3 zUp = _wingCp.up;
        Vector3 ySpan = _wingCp.right;

        float alphaRaw = Mathf.Atan2(Vector3.Dot(flow, zUp), Vector3.Dot(flow, xChord));
        float alphaLimit = Mathf.Deg2Rad * Mathf.Abs(_alphaLimitDeg);
        _alphaRad = Mathf.Clamp(alphaRaw, -alphaLimit, alphaLimit);

        _cl = _clAlpha * _alphaRad;
        float kInduced = 1f / (Mathf.PI * Mathf.Max(_aspectRatio, 0f) * Mathf.Max(_oswaldEfficiency, 0f));
        _cd = _cdo + kInduced * _cl * _cl;

        _qDyn = 0.5f * _airDensity * _speed * _speed;
        _lift = _qDyn * _wingArea * _cl;
        _drag = _qDyn * _wingArea * _cd;

        Vector3 liftDir = Vector3.Cross(flow, ySpan).normalized;
        Vector3 lift = _lift * liftDir;
        Vector3 drag = _drag * -flow;

        _rb.AddForceAtPosition(lift + drag, _wingCp.position, ForceMode.Force);
    }
}
