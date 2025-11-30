using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlaneEngine : MonoBehaviour
{
    [Header("Точка приложения тяги")]
    [SerializeField] private Transform _nozzle;
    [SerializeField] private float _thrustDry = 79000f;
    [SerializeField] private float _thrustAfterburner = 129000f;
    [SerializeField] private InputActionAsset _actionAsset;

    private Rigidbody _rb;
    private float _throttle;
    private bool _afterburner;
    private float _speed;
    private float _lastThrust;

    private InputAction _throttleUp, _throttleDown, _toggleAB;

    public float Throttle01 => _throttle;
    public bool AfterBurner => _afterburner;
    public float CurrentThrust => _lastThrust;
    public float SpeedMS => _speed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _throttle = 0f;
        _afterburner = false;
        InitializeActions();
    }

    private void InitializeActions()
    {
        var map = _actionAsset.FindActionMap("Aircraft");
        _throttleUp = map.FindAction("ThrottleUp");
        _throttleDown = map.FindAction("ThrottleDown");
        _toggleAB = map.FindAction("ToggleAB");
        _toggleAB.performed += _ => _afterburner = !_afterburner;
    }

    private void OnEnable()
    {
        _throttleUp.Enable();
        _throttleDown.Enable();
        _toggleAB.Enable();
    }

    private void OnDisable()
    {
        _throttleUp.Disable();
        _throttleDown.Disable();
        _toggleAB.Disable();
    }

    private void FixedUpdate()
    {
        _speed = _rb.linearVelocity.magnitude;
        float dt = Time.fixedDeltaTime;

        if (_throttleUp.IsPressed()) _throttle = Mathf.Clamp01(_throttle + dt);
        if (_throttleDown.IsPressed()) _throttle = Mathf.Clamp01(_throttle - dt);

        _lastThrust = _throttle * (_afterburner ? _thrustAfterburner : _thrustDry);
        _rb.AddForce(transform.forward * _lastThrust, ForceMode.Force);
    }
}
