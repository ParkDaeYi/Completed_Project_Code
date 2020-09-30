using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtVRCanvas : MonoBehaviour
{
    public Transform _target;

    void Update()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Vector3 vec = transform.GetChild(i).position - _target.position;
            vec.Normalize();
            Quaternion q = Quaternion.LookRotation(vec);
            transform.GetChild(i).rotation = q;
        }
    }
}
