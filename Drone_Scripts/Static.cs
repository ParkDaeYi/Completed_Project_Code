using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static
{
    public static bool _isStart = true; //시동 확인

    public static Vector3 _dronePosition; //드론 위치
    public static Quaternion _droneRotation; //드론 회전
    public static bool _droneStartUp; //드론 시동

    public static Vector3 _playerPosition; //플레이어 위치
    public static Quaternion _playerRotation; //플레이어 회전
}