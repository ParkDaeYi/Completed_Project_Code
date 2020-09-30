using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    GameObject _door;
    public Collider[] _hits;
    public GameObject _hit;
    public float speed = 0.02f;
    public Vector3 _dir;

    private Vector3 _size;
    public float _originRotY;
    private float _rot;

    // Use this for initialization
    void Start()
    {
        _door = this.transform.GetChild(0).gameObject;
        _originRotY = _door.transform.localEulerAngles.y;
        Vector3 _forward = Mathf.Abs(this.transform.forward.x) * Vector3.right + Mathf.Abs(this.transform.forward.y) * Vector3.up + Mathf.Abs(this.transform.forward.z) * Vector3.forward;
        _size = this.transform.lossyScale / 4 + _forward * 5f;
    }

    // Update is called once per frame
    void Update()
    {

        if (_hit)
        { //충돌한 사람 객체가 존재하는 경우

            _dir = this.transform.position - _hit.transform.position;
            float _fb = _dir.x * this.transform.forward.x + _dir.y * this.transform.forward.y + _dir.z * this.transform.forward.z;
            if (_fb < 0) //객체가 문 객체 기준 forward에 존재
            {
                _rot = 90;
            }
            else
            {
                _rot = -90;
            }
            _door.transform.localRotation = Quaternion.Slerp(_door.transform.localRotation, Quaternion.Euler(_door.transform.localEulerAngles.x, _originRotY + _rot, _door.transform.localEulerAngles.z), speed*Time.deltaTime);
            //_door.transform.rotation = Quaternion.Slerp(_door.transform.rotation, Quaternion.Euler(Vector3.up*_rot+_door.transform.eulerAngles), speed*Time.deltaTime)
            if (Vector3.Distance(_hit.transform.position, this.transform.position) > 20f)
            {
                _hit = null;
                //_door.transform.localRotation = Quaternion.Slerp(_door.transform.localRotation, Quaternion.Euler(_door.transform.localEulerAngles.x, _originRotY, _door.transform.localEulerAngles.z), speed);
            }
        }
        else
        {
            _door.transform.localRotation = Quaternion.Slerp(_door.transform.localRotation, Quaternion.Euler(_door.transform.localEulerAngles.x, _originRotY, _door.transform.localEulerAngles.z), speed*Time.deltaTime);
            _hits = Physics.OverlapBox(this.transform.position + Vector3.up * 10f, _size);
            for (int i = 0; i < _hits.Length; i++)
            {
                if (_hits[i].transform.tag == "Human")
                {
                    _hit = _hits[i].transform.gameObject;
                    break;
                }
            }
        }
        //Debug.Log(Vector3.Distance(_hit.transform.position, this.transform.position));
    }
}
