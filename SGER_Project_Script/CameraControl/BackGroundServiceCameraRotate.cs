using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundServiceCameraRotate : MonoBehaviour {

    public GameObject _yAxis;
	// Update is called once per frame
	void Update () {
        transform.RotateAround(_yAxis.transform.position, Vector3.up, 20 * Time.deltaTime);
	}
}
