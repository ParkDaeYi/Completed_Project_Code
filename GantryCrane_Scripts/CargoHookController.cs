using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoHookController : MonoBehaviour
{
    public CraneControl _craneControl;
    public RaycastHit[] DownCheck;
    public RaycastHit[] DropCheck;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("container"))
        {
            if (!_craneControl.isCatching)
                _craneControl._other = other;
            _craneControl._cargohookMove_Down = false;
        }
        else
        {
            _craneControl._cargohookMove_Down = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(_craneControl._other && !_craneControl.isCatching)
        {
            _craneControl._other = null;
        }
        if (other.gameObject.CompareTag("container"))
        {
            _craneControl._cargohookMove_Down = true;
        }
    }

    private void Update()
    {
        CargoHookControl();
    }

    private void CargoHookControl()
    {
        if (_craneControl._other)
        {
            DownCheck = Physics.BoxCastAll(GetComponent<Transform>().position, new Vector3(10.8f, 2.5f, 1f) * 0.5f, -GetComponent<Transform>().forward, GetComponent<Transform>().rotation, 2.8f);

            if (DownCheck.Length > 1)
            {
                for (int i = 0; i < DownCheck.Length; i += 1)
                {
                    //Debug.Log(DownCheck.Length + " : " + DownCheck[i].transform.tag);
                    if (DownCheck[i].transform.tag == "container" || DownCheck[i].transform.tag == "truck") //컨테이너와 닿으면
                    {
                        _craneControl._cargohookMove_Down = false;
                        break;
                    }
                }
            }
            else
            {
                _craneControl._cargohookMove_Down = true;
            }
        }


        DropCheck = Physics.RaycastAll(GetComponent<Transform>().position, -GetComponent<Transform>().forward, 3.3f);
        
        if (DropCheck.Length > 1)   //카고훅으로 어짜피 하나 밖에 못 잡음
        {
            for(int i = 0; i < DropCheck.Length; i++)
            {
                //Debug.Log(DropCheck.Length+" : "+DropCheck[i].transform.tag);
                if (DropCheck[i].transform.tag == "container" || DropCheck[i].transform.tag == "truck") //컨테이너와 닿으면
                {
                    _craneControl._cargohookMove_Drop = true;
                    break;
                }
            }
        }
        else
        {
            _craneControl._cargohookMove_Drop = false;
        }
    }
}
