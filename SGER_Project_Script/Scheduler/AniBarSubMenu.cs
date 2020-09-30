using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AniBarSubMenu : MonoBehaviour
{

    public InputField _inputField;
    public ItemListControl _itemListControl;

    public GameObject _smallAniBar;
    public GameObject _bigAniBar;

    public void ClickRename()
    {
        Static.STATIC._clickAniBar.transform.GetChild(0).GetComponent<Text>().text = _inputField.text;
        _inputField.text = "";
    }

    public void CloseClick()
    {
        this.transform.parent.parent.gameObject.SetActive(false);
    }

    public void DeleteClick()
    {

        if (_smallAniBar.GetComponent<SmallAniBar>()._voice)
        {
            foreach (BigAniBar A in _itemListControl._dataBaseBigVoice)
            {
                Debug.Log("a = " + A);
                if (A == _bigAniBar.GetComponent<BigAniBar>())
                {
                    _itemListControl._dataBaseBigVoice.Remove(A);
                    break;
                }
            }
            foreach (SmallAniBar A in _itemListControl._dataBaseSmallVoice)
            {
                if (A == _smallAniBar.GetComponent<SmallAniBar>())
                {
                    _itemListControl._dataBaseSmallVoice.Remove(A);
                    break;
                }
            }
        }
        else
        {
            foreach (BigAniBar A in _itemListControl._dataBaseBigAnimation)
            {
                if (A == _bigAniBar.GetComponent<BigAniBar>())
                {
                    Debug.Log(1);
                    _itemListControl._dataBaseBigAnimation.Remove(A);
                    break;
                }
            }
            foreach (SmallAniBar A in _itemListControl._dataBaseSmallAnimation)
            {
                if (A == _smallAniBar.GetComponent<SmallAniBar>())
                {
                    Debug.Log(2);
                    _itemListControl._dataBaseSmallAnimation.Remove(A);
                    break;
                }
            }
        }
        if (_smallAniBar.GetComponent<SmallAniBar>()._voice)
        {
            BigAniBar _tmp = _bigAniBar.GetComponent<BigAniBar>();
            SmallAniBar _tmp1 = _smallAniBar.GetComponent<SmallAniBar>();
            float _barX = _tmp.gameObject.transform.localPosition.x;
            float _barWidth = _tmp1._aniBarWidth;
            string _voiceName = Static.STATIC._clickAniBar.transform.GetChild(0).GetComponent<Text>().text;
            int _dir_key = _tmp1._dir_key;
            int _originNumber = _tmp1._item._originNumber;
            string _parentName = "";
            if (_originNumber == 2000) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 8);
            if (_originNumber == 2001) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 3);
            if (_originNumber == 2002) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 5);

            HistoryController.pushAniBarVoiceDeleteHist(_barX, _barWidth, _voiceName, _parentName,_dir_key,_originNumber);
        }
        else
        {
            BigAniBar _tmp = _bigAniBar.GetComponent<BigAniBar>();
            SmallAniBar _tmp1 = _smallAniBar.GetComponent<SmallAniBar>();
            float _barX = _tmp.gameObject.transform.localPosition.x;
            float _barWidth = _tmp1._aniBarWidth;
            string _animationName = _tmp1._animationName;
            string _animationText = _tmp.transform.GetChild(0).GetComponent<Text>().text;
            int _moveOrState;
            int _actionOrFace;
            int _rotation;
            if (_tmp1._moveCheck)
                _moveOrState = 1;
            else
                _moveOrState = 0;
            if (_tmp1._actionOrFace)
                _actionOrFace = 1;
            else
                _actionOrFace = 0;
            if (_tmp1._rotation)
                _rotation = 1;
            else
                _rotation = 0;

            int _layerNumber = _tmp1._layerNumber;
            float _arriveX = _tmp1._arriveLocation.x;
            float _arriveY = _tmp1._arriveLocation.y;
            float _arriveZ = _tmp1._arriveLocation.z;

            int _originNumber = _tmp1._item._originNumber;
            string _parentName = "";
            if (_originNumber == 2000) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 8);
            if (_originNumber == 2001) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 3);
            if (_originNumber == 2002) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 5);

            HistoryController.pushAniBarDeleteHist(_barX, _barWidth, _animationName, _parentName, _animationText, _moveOrState, _actionOrFace, _layerNumber, _arriveX, _arriveY, _arriveZ, _originNumber, _rotation);
        }
        Destroy(_smallAniBar);
        Destroy(_bigAniBar);
        Static.STATIC._clickAniBar = null;
        this.transform.parent.parent.gameObject.SetActive(false);
    }
}
