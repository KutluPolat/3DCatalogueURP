using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Vector3 DeltaPosMouse0 { get; private set; }
    public static Vector3 DeltaPosMouse1 { get; private set; }
    private Vector3 lastInputPos0;
    private Vector3 lastInputPos1;

    private void Update()
    {
        // TO-DO: USE NEW INPUT SYSTEM!
        // TO-DO: IMPLEMENT CONTROLLER!
        //if (Input.GetAxis("Joystick Axis 1") != 0 || Input.GetAxis("Joystick Axis 2") != 0)
        //{
        //    DeltaPosMouse0 = new Vector3(Input.GetAxis("Joystick Axis 1"), Input.GetAxis("Joystick Axis 2"), 0);
        //}
        //else 
        if (Input.GetMouseButtonDown(0))
        {
            lastInputPos0 = Input.mousePosition;
            DeltaPosMouse0 = Vector3.zero;
        }
        else if (Input.GetMouseButton(0))
        {
            DeltaPosMouse0 = Input.mousePosition - lastInputPos0;
            lastInputPos0 = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastInputPos0 = Input.mousePosition;
            DeltaPosMouse0 = Vector3.zero;
        }

        //if (Input.GetAxis("Joystick Axis 4") != 0 || Input.GetAxis("Joystick Axis 5") != 0)
        //{
        //    DeltaPosMouse1 = new Vector3(Input.GetAxis("Joystick Axis 4"), Input.GetAxis("Joystick Axis 5"), 0);
        //}
        //else 
        if (Input.GetMouseButtonDown(1))
        {
            lastInputPos1 = Input.mousePosition;
            DeltaPosMouse1 = Vector3.zero;
        }
        else if (Input.GetMouseButton(1))
        {
            DeltaPosMouse1 = Input.mousePosition - lastInputPos1;
            lastInputPos1 = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            lastInputPos1 = Input.mousePosition;
            DeltaPosMouse1 = Vector3.zero;
        }
    }
}
