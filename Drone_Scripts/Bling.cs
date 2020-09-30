using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bling : MonoBehaviour
{
    /* 폰트 반짝이게 하는 스크립트 */
    public Text _object;
    private bool _trigger = true;

    private void Start()
    {
        _object.color = Color.white;
    }

    private void Update()
    {
        if (_object.color.r <= 0f)
        {
            _trigger = true;
        }
        else if (_object.color.r >= 1f)
        {
            _trigger = false;
        }

        if (_trigger)
        {
            _object.color = new Color(_object.color.r + Time.deltaTime * 2, _object.color.g + Time.deltaTime * 2, _object.color.b + Time.deltaTime * 2);
        }
        else
        {
            _object.color = new Color(_object.color.r - Time.deltaTime * 2, _object.color.g - Time.deltaTime * 2, _object.color.b - Time.deltaTime * 2);
        }
    }
}
