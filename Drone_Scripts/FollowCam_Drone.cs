using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam_Drone : MonoBehaviour
{

    public Transform _Target;
    public float dist;
    public float height;
    public float smoothRotate;
    private Transform tr;
    public bool _isVR;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        float currYAngle = Mathf.LerpAngle(tr.eulerAngles.y, _Target.eulerAngles.y, smoothRotate * Time.deltaTime);
        Quaternion rot = Quaternion.Euler(0, currYAngle, 0);
        tr.position = _Target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);
        if (_isVR)
        {
            tr.LookAt(new Vector3(_Target.position.x, _Target.position.y + (transform.position.y - _Target.position.y), _Target.position.z));
        }
        else
        {
            tr.LookAt(_Target);
        }
    }
}