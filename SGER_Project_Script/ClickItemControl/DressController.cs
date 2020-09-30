using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DressController : MonoBehaviour {
    /**
* date 2018.08.08
* author INHO
* desc
*   드레스 또는, 왼손, 오른손 선택에 따라 보여지는 창이 달라지도록
*   해주는 스크립트.
*/

    [Header("Dress")]
    public GameObject _rightHandPanel;
    public GameObject _leftHandPanel;
    public GameObject _cloth; //클릭된 옷의 파트
    public GameObject _shirtButtons;
    public GameObject _pantButtons;
    public GameObject _shoesButtons;
    public Transform _colourization;
    public string _activeDress;
    public Sprite _check;

    [Header("ClickedItem")]
    public ClickedItemControl _clickedItemControl;
    public ItemListControl _itemListControl;

    [Header("UI")]
    public Toggle _colorToggle; //색 지정 토글
    public GameObject _colorChangeButton; //색 변경 버튼
    public GameObject _dressCanvas; //DressCanvas
    public GameObject _colorChangeCanvas; //색 변경 Canvas

    [Header("Material")]
    public Material _dressMaterial; //옷 메터리얼

    [Header("Color")]
    public Slider _r;
    public Slider _g;
    public Slider _b;
    public Image _colorSample; //색 샘플 이미지
    public Text _status; //색 정보

    public void Start()
    {
        /* 색 변경 버튼은 초기에 비활성화 */
        //_colorChangeButton.SetActive(false);
    }
    public void OnClickRightHand()
    {
        _rightHandPanel.SetActive(true);
        _leftHandPanel.SetActive(false);
    }

    public void OnClickLeftHand()
    {
        _leftHandPanel.SetActive(true);
        _rightHandPanel.SetActive(false);
    }

    /* date 2019.09.28
     * author DAY
     * desc
     *   ResetColorToggle() 필요없어서 삭제
     *   _cloth 는 각 함수를 통해 찾음
     *   _activeDress 를 통해 현재 활성화 되어있는 파츠를 구분하여
     *   OnClickColorToggle() 및 OnClickColorApplyButton() 을 통해
     *   색상에 대한 정보를 HumanItem에 저장해줌
     */

    /* 색 토글을 클릭했을 때 */
    public void OnClickColorToggle()
    {
        if (_cloth == null)
        {
            _cloth = _clickedItemControl._clickedHumanItem._shirt;
        }
        _colourization = _cloth.transform.Find("Colourization");
        if (_colourization == null) //색상화 객체가 없을 경우
        {
            GameObject _clothes = Instantiate(_cloth);

            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            Material _copyMaterial = Instantiate(_dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(0f, 0f, 0f, 0f);
        }

        /* 옷이 바뀔 때마다 HumanItem 정보를 갱신해줘야함*/
        if (_colourization != null)
        {
            SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();

            if (_activeDress.Equals("shirt"))
            {
                _clickedItemControl._clickedHumanItem._shirt_R = _colourizationSkin.material.color.r * 255f;
                _clickedItemControl._clickedHumanItem._shirt_G = _colourizationSkin.material.color.g * 255f;
                _clickedItemControl._clickedHumanItem._shirt_B = _colourizationSkin.material.color.b * 255f;
            }
            else if (_activeDress.Equals("pant"))
            {
                _clickedItemControl._clickedHumanItem._pant_R = _colourizationSkin.material.color.r * 255f;
                _clickedItemControl._clickedHumanItem._pant_G = _colourizationSkin.material.color.g * 255f;
                _clickedItemControl._clickedHumanItem._pant_B = _colourizationSkin.material.color.b * 255f;
            }
            else if (_activeDress.Equals("shoes"))
            {
                _clickedItemControl._clickedHumanItem._shoes_R = _colourizationSkin.material.color.r * 255f;
                _clickedItemControl._clickedHumanItem._shoes_G = _colourizationSkin.material.color.g * 255f;
                _clickedItemControl._clickedHumanItem._shoes_B = _colourizationSkin.material.color.b * 255f;
            }
        }
    }

    /* 색 변경 버튼을 눌렀을 때 */
    public void OnClickColorChangeButton()
    {
        OnClickColorToggle();
        if (_clickedItemControl._clickedHumanItem._shirt != null || _clickedItemControl._clickedHumanItem._pant != null || _clickedItemControl._clickedHumanItem._shoes != null)
        {
            SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();

            //해당 옷에 대한 색상 정보 표시
            _r.value = _colourizationSkin.material.color.r * 255f;
            _g.value = _colourizationSkin.material.color.g * 255f;
            _b.value = _colourizationSkin.material.color.b * 255f;

            _dressCanvas.SetActive(false); //Dress Canvas 비활성화
            _colorChangeCanvas.SetActive(true); //옷 변경 Canvas 활성화
        }
    }

    /* 뒤로 버튼을 눌렀을 때*/
    public void OnClickColorBackButton()
    {
        /* 캔버스 교체 */
        _colorChangeCanvas.SetActive(false);
        _dressCanvas.SetActive(true);
    }

    /* RGB Slider를 슬라이드 할 때 */
    public void SlideRGB() 
    {
        _colorSample.color = new Color(_r.value / 255f, _g.value / 255f, _b.value / 255f); //이미지 색 변경
        _status.text = "(" + (int)_r.value + ", " + (int)_g.value + ", " + (int)_b.value + ")"; //정보 창 변경
    }

    /* 적용 버튼을 눌렀을 때*/
    public void OnClickColorApplyButton()
    {
        SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
        _colourizationSkin.material.color = new Color(_r.value / 255f, _g.value / 255f, _b.value / 255f, 40f / 255f); //메터리얼 색 변경

        /*
         *  date 2020.01.29
         *  author skyde47
         *  desc
         *  Push History 작성
         */

        if (_activeDress.Equals("shirt"))
        {
            HistoryController.pushDressRGBHist(_clickedItemControl._clickedItem._originNumber, _clickedItemControl._clickedItem._objectNumber, 0, new Color(_clickedItemControl._clickedHumanItem._shirt_R / 255f, _clickedItemControl._clickedHumanItem._shirt_G / 255f, _clickedItemControl._clickedHumanItem._shirt_B / 255f, 40f / 255f));
            _clickedItemControl._clickedHumanItem._shirt_R = _colourizationSkin.material.color.r * 255f;
            _clickedItemControl._clickedHumanItem._shirt_G = _colourizationSkin.material.color.g * 255f;
            _clickedItemControl._clickedHumanItem._shirt_B = _colourizationSkin.material.color.b * 255f;
        }
        else if (_activeDress.Equals("pant"))
        {
            HistoryController.pushDressRGBHist(_clickedItemControl._clickedItem._originNumber, _clickedItemControl._clickedItem._objectNumber, 1, new Color(_clickedItemControl._clickedHumanItem._pant_R / 255f, _clickedItemControl._clickedHumanItem._pant_G / 255f, _clickedItemControl._clickedHumanItem._pant_B / 255f, 40f / 255f));
            _clickedItemControl._clickedHumanItem._pant_R = _colourizationSkin.material.color.r * 255f;
            _clickedItemControl._clickedHumanItem._pant_G = _colourizationSkin.material.color.g * 255f;
            _clickedItemControl._clickedHumanItem._pant_B = _colourizationSkin.material.color.b * 255f;
        }
        else if(_activeDress.Equals("shoes"))
        {
            HistoryController.pushDressRGBHist(_clickedItemControl._clickedItem._originNumber, _clickedItemControl._clickedItem._objectNumber, 2, new Color(_clickedItemControl._clickedHumanItem._shoes_R / 255f, _clickedItemControl._clickedHumanItem._shoes_G / 255f, _clickedItemControl._clickedHumanItem._shoes_B / 255f, 40f / 255f));
            _clickedItemControl._clickedHumanItem._shoes_R = _colourizationSkin.material.color.r * 255f;
            _clickedItemControl._clickedHumanItem._shoes_G = _colourizationSkin.material.color.g * 255f;
            _clickedItemControl._clickedHumanItem._shoes_B = _colourizationSkin.material.color.b * 255f;
        }

        /* 캔버스 교체 */
        _colorChangeCanvas.SetActive(false);
        _dressCanvas.SetActive(true);
    }

    /* date 2019.08.02
     * author HR
     * desc
     *  각 colorToggle 버튼을 눌렀을 때 _cloth에 값 삽입
     *  sophia 객체 중심으로 작성되었기 때문에 이후 수정필요!
     */

    public void onClickShirtButton()
    {
        _activeDress = "shirt";
        if (_clickedItemControl._clickedHumanItem._shirt != null)
        {
            _cloth = _clickedItemControl._clickedHumanItem._shirt;
            OnClickColorToggle();
        }
        _shirtButtons.SetActive(true);
        _pantButtons.SetActive(false);
        _shoesButtons.SetActive(false);
    }
    public void onClickPantButton()
    {
        _activeDress = "pant";
        if (_clickedItemControl._clickedHumanItem._pant != null)
        {
            _cloth = _clickedItemControl._clickedHumanItem._pant;
            OnClickColorToggle();
        }
        _shirtButtons.SetActive(false);
        _pantButtons.SetActive(true);
        _shoesButtons.SetActive(false);
    }
    public void onClickShoesButton()
    {
        _activeDress = "shoes";
        if (_clickedItemControl._clickedHumanItem._shoes != null)
        {
            _cloth = _clickedItemControl._clickedHumanItem._shoes;
            OnClickColorToggle();
        }
        _shirtButtons.SetActive(false);
        _pantButtons.SetActive(false);
        _shoesButtons.SetActive(true);
    }

    /* TookOff() ==> 옷을 입지 않은 상태(파츠마다)로 되돌려줌
     * 그러므로 해당 되는 부분들을 다 초기화 시켜줘야함 */
    public void onShirtTookOff()
    {
        if (_clickedItemControl._clickedHumanItem._shirt != null)
        {
            _clickedItemControl._clickedHumanItem._shirt.SetActive(false);
            _shirtButtons.transform.GetChild(0).GetChild(0).Find(_clickedItemControl._clickedHumanItem._shirt.name).gameObject.transform.GetChild(1).GetComponent<Image>().sprite = null;
            _clickedItemControl._clickedHumanItem._shirt = null;
            Destroy(_cloth.transform.GetChild(0).gameObject);
            _cloth = null;
            _colourization = null;
            _clickedItemControl._clickedHumanItem._shirt_R = 0f;
            _clickedItemControl._clickedHumanItem._shirt_G = 0f;
            _clickedItemControl._clickedHumanItem._shirt_B = 0f;
        }
    }
    public void onPantTookOff()
    {
        if (_clickedItemControl._clickedHumanItem._pant != null)
        {
            _clickedItemControl._clickedHumanItem._pant.SetActive(false);
            _pantButtons.transform.GetChild(0).GetChild(0).Find(_clickedItemControl._clickedHumanItem._pant.name).gameObject.transform.GetChild(1).GetComponent<Image>().sprite = null;
            _clickedItemControl._clickedHumanItem._pant = null;
            Destroy(_cloth.transform.GetChild(0).gameObject);
            _cloth = null;
            _colourization = null;
            _clickedItemControl._clickedHumanItem._pant_R = 0f;
            _clickedItemControl._clickedHumanItem._pant_G = 0f;
            _clickedItemControl._clickedHumanItem._pant_B = 0f;
        }
    }
    public void onShoseTookOff()
    {
        if (_clickedItemControl._clickedHumanItem._shoes != null) //현재 신발을 신고있는 경우에만 적용
        {
            _clickedItemControl._clickedHumanItem._shoes.SetActive(false);
            _shoesButtons.transform.GetChild(0).GetChild(0).Find(_clickedItemControl._clickedHumanItem._shoes.name).gameObject.transform.GetChild(1).GetComponent<Image>().sprite = null;
            _clickedItemControl._clickedHumanItem._shoes = null;
            Destroy(_cloth.transform.GetChild(0).gameObject);
            _cloth = null;
            _colourization = null;
            _clickedItemControl._clickedHumanItem._shoes_R = 0f;
            _clickedItemControl._clickedHumanItem._shoes_G = 0f;
            _clickedItemControl._clickedHumanItem._shoes_B = 0f;
        }
        _clickedItemControl._clickedItem.item3d.transform.Find("shoes").GetChild(1).gameObject.SetActive(false);
        _clickedItemControl._clickedItem.item3d.transform.Find("shoes").GetChild(0).gameObject.SetActive(true);
        _clickedItemControl._clickedItem.item3d.transform.Find("shoes").GetChild(0).GetChild(0).gameObject.SetActive(false);

        //position rollback
        Vector3 _tmp = _clickedItemControl._clickedItem.item3d.transform.parent.transform.position;
        _tmp.y = _clickedItemControl._clickedItem.item3d.GetComponent<ItemObject>()._humanInitPosition.y;
        _clickedItemControl._clickedItem.item3d.transform.parent.transform.position = _tmp;

        CheckUI();
    }


    /*
     * date 2020.01.14
     * desc
     * dress 버튼을 탐색하며 check ui설정 및 비설정
     */

    //dress의 ui를 제어하는 함수
    public void CheckUI()
    {
        //shirt
        if (_clickedItemControl._clickedHumanItem._shirt != null)
        {
            for (int i = 0; i < _shirtButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_clickedItemControl._clickedHumanItem._shirt.name.Equals(_shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
        //pant
        if (_clickedItemControl._clickedHumanItem._pant != null)
        {
            for (int i = 0; i < _pantButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_clickedItemControl._clickedHumanItem._pant.name.Equals(_pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
        //shoes
        if (_clickedItemControl._clickedHumanItem._shoes != null)
        {
            for (int i = 0; i < _shoesButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_clickedItemControl._clickedHumanItem._shoes && _clickedItemControl._clickedHumanItem._shoes.name.Equals(_shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else if (!_clickedItemControl._clickedHumanItem._shoes && _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name.Equals("tookOff"))
                {
                    _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
    }

    /*
     * date 2020.01.20
     * author skyde47
     * desc
     * 기존 CheckUI함수는 clickedItem에 종속적이여서 히스토리에서는 NULL Reference Exception으로 사용불가능 
     * 따라서 히스토리용 새로운 함수 작성
     */

    public void ForHistoryCheckUI(HumanItem _change, int _changeNum)
    {
        //shirt
        if (_changeNum == 1)
        {
            for (int i = 0; i < _shirtButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_change._shirt.name.Equals(_shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _shirtButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
        //pant
        else if (_changeNum == 2)
        {
            for (int i = 0; i < _pantButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_change._pant.name.Equals(_pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _pantButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
        //shoes
        else
        {
            for (int i = 0; i < _shoesButtons.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                if (_change._shoes && _change._shoes.name.Equals(_shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name))
                {
                    _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else if (!_change._shoes && _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.name.Equals("tookOff"))
                {
                    _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = _check;
                }
                else _shoesButtons.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
    }
}
