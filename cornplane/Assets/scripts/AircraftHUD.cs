using UnityEngine;

public class AircraftHUD : MonoBehaviour
{
    private CornplaneIntegrated _aircraft;
    private PlaneEngine _engine;
    private GliderWing _wing;
    private PlaneFlightController _flightController;

    private void Awake()
    {
        _aircraft = FindObjectOfType<CornplaneIntegrated>();
        _engine = FindObjectOfType<PlaneEngine>();
        _wing = FindObjectOfType<GliderWing>();
        _flightController = FindObjectOfType<PlaneFlightController>();
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(10, 10, 350, 500), GUI.skin.box);
        GUILayout.Label("���������� - ����������", GUI.skin.label);
        GUILayout.Space(10);

        if (_aircraft)
        {
            Rigidbody rb = _aircraft.GetComponent<Rigidbody>();
            Vector3 v = rb.linearVelocity;
            GUILayout.Label($"��������: {v.magnitude * 3.6f:0.0} ��/�");
            GUILayout.Label($"������: {_aircraft.transform.position.y:0.0} �");
            GUILayout.Label($"����: {NormalizeAngle(_aircraft.transform.eulerAngles.y):0}�");
            GUILayout.Label($"����: {NormalizeAngle(_aircraft.transform.eulerAngles.z):0}�");
            GUILayout.Label($"������: {NormalizeAngle(_aircraft.transform.eulerAngles.x):0}�");
        }

        if (_engine)
        {
            GUILayout.Label($"�����: {(_engine.AfterBurner ? "������" : "����������")}");
            GUILayout.Label($"����: {_engine.Throttle01 * 100:0}%");
            GUILayout.Label($"���� ����: {_engine.CurrentThrust:0} �");
        }

        if (_wing)
        {
            GUILayout.Label($"���� �����: {_wing.AlphaDeg:0.0}�");
            GUILayout.Label($"��������� ����: {_wing.LiftForce:0.0} �");
            GUILayout.Label($"�������������: {_wing.DragForce:0.0} �");
        }

        if (_flightController)
        {
            GUILayout.Label($"������������: {(_flightController.IsHoldingAttitude ? "���" : "����")}");
        }

        GUILayout.EndArea();
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
}
