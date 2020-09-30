using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRInput : MonoBehaviour
{
    [Header("Binding")]
    public SteamVR_Action_Vector2 LeftJoystickAction;
    public SteamVR_Action_Vector2 RightJoystickAction;

    [Header("Input")]
    public Vector2 LeftJoystick;
    public Vector2 RightJoystick;

    void Update()
    {
        LeftJoystick = LeftJoystickAction.GetAxis(SteamVR_Input_Sources.Any);
        RightJoystick = RightJoystickAction.GetAxis(SteamVR_Input_Sources.Any);
    }
}