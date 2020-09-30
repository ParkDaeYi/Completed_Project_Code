using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleControl : MonoBehaviour {
    /**
* date 2018.07.22
* author Lugub
* desc
*  아이템크기조정하는 스크립트
* 
*/
    [Header("ReScale Item")]
    public GameObject _reScaleItem;

    [Header("Slider")]
    public Text _xValueText;
    public Text _yValueText;
    public Text _zValueText;

    public Slider _xSlider;
    public Slider _ySlider;
    public Slider _zSlider;

    public bool _isRenewalScaleSlider;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //ScaleChange();
        TextChange();
    }

    void ScaleChange()
    {
        _reScaleItem.transform.localScale = new Vector3(_xSlider.value, _ySlider.value, _zSlider.value);
    }

    void TextChange()
    {
        _xValueText.text = (Mathf.Round(_xSlider.value * 100f) * 0.01).ToString();
        _yValueText.text = (Mathf.Round(_ySlider.value * 100f) * 0.01).ToString();
        _zValueText.text = (Mathf.Round(_zSlider.value * 100f) * 0.01).ToString();

    }

    public void ScaleStart(GameObject _reScaleItem)
    {
        this._reScaleItem = _reScaleItem;
        _xSlider.value = _reScaleItem.transform.localScale.x;
        _ySlider.value = _reScaleItem.transform.localScale.y;
        _zSlider.value = _reScaleItem.transform.localScale.z;
        
        _isRenewalScaleSlider = false;
    }

    public void ScaleSlider()
    {
        Debug.Log("Check123");
        if (_reScaleItem)
        {
            if (!_isRenewalScaleSlider)
            {
                _xSlider.value = _reScaleItem.transform.localScale.x;
                _ySlider.value = _reScaleItem.transform.localScale.y;
                _zSlider.value = _reScaleItem.transform.localScale.z;
                _isRenewalScaleSlider = true;
            }
            /* 소수점 둘째 자리에서 반올림 한 값으로 변경 */
            float X = Mathf.Floor(_xSlider.value * 100f) / 100f;
            float Y = Mathf.Floor(_ySlider.value * 100f) / 100f;
            float Z = Mathf.Floor(_zSlider.value * 100f) / 100f;

            _reScaleItem.transform.localScale = new Vector3(X, Y, Z); //크기 변경
        }
    }

    public void HistoryUpdate()
    {
        HistoryController.pushObjectHist(_reScaleItem, _reScaleItem.transform.position, _reScaleItem.transform.localScale, _reScaleItem.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _reScaleItem.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _reScaleItem.transform.rotation);
    }

    public void ResetForHistory(GameObject _reScaleItem)
    {
        this._reScaleItem = _reScaleItem;
        _isRenewalScaleSlider = false;
        ScaleSlider();
    }
}
