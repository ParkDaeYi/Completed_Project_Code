using UnityEngine;
using UnityEngine.UI;

public class ClickedPlaceControl : MonoBehaviour
{
    /**
* date 2019.08.18
* author GS
* desc
* ClickedPlaceCanvas에 달려있는 스크립트로써 자식으로 포함 모든 UI를 다룬다.
* UI를 조작할 시 PlaceController 스크립트와 연동된다.
*/
    [Header("Variable")]
    public bool _activeBasePanelOnce; //ClickedPlaceCanvas의 BasePanel을 한번만 실행시킬 수 있도록 하는 함수
    public bool _isRenewalScaleSlider; //크기 조절 Slider를 갱신 중인지 판단하는 변수
    public bool[] _checkPlacementAsix; //X, Y, Z축 방향으로 위치를 조정하기 위해 사용하는 판단 변수
    public bool _isDocking; //도킹 기능이 활성화되어 있는지 확인하는 변수

    [Header("UI")]
    public GameObject _basePanel; //클릭된 건물을 조정하는 ClickedPlaceCanvas의 BasePanel
    public GameObject _buildButtons; //건물을 생성하는 build 버튼들을 담는 오브젝트
    public GameObject _planeButtons; //텍스처를 변경하는 Plane 버튼들을 담는 오브젝트

    [Header("ClickedPlaceCanvas")]
    public Text _clickedPlaceName; //클릭된 Place의 이름을 나타내는 Text
    public Slider _xSlider; //클릭된 Place의 X축 크기를 변경하는 Slider
    public Slider _ySlider; //클릭된 Place의 Y축 크기를 변경하는 Slider
    public Slider _zSlider; //클릭된 Place의 Z축 크기를 변경하는 Slider
    public InputField _xSliderValue; //클릭된 Place의 X축 크기를 출력하는 InputField
    public InputField _ySliderValue; //클릭된 Place의 Y축 크기를 출력하는 InputField
    public InputField _zSliderValue; //클릭된 Place의 Z축 크기를 출력하는 InputField
    public InputField _xTilingField; //클릭된 Place의 X축 Tiling 크기를 출력하는 InputField
    public InputField _yTilingField; //클릭된 Place의 Y축 Tiling 크기를 출력하는 InputField
    public Image _dockingButtonImage; //도킹 기능을 활성화/비활성화 하는 버튼에 달려있는 Image 컴포넌트
    public InputField _xPositionField; //클릭된 Place의 X축 위치를 나타내는 InputField
    public InputField _yPositionField; //클릭된 Place의 Y축 위치를 나타내는 InputField
    public InputField _zPositionField; //클릭된 Place의 Z축 위치를 나타내는 InputField

    [Header("Script")]
    public PlaceController _placeController; //PlaceController 스크립트
    public CameraMoveAroun _cameraMoveAroun; //CameraMoveAroun 스크립트
    public ItemListControl _itemListControl; //ItemListControl 스크립트

    private void Start()
    {
        _checkPlacementAsix = new bool[6]; //크기 할당
    }

    private void Update()
    {
        ActiveClickedPlaceCanvas();
        RenewalClickedPlaceName();
        if (_xSliderValue.isFocused || _ySliderValue.isFocused || _zSliderValue.isFocused) _cameraMoveAroun._cameraAroun = false; //InputField 활성화일 시 카메라 조작할 수 없음
        PlacementAxis();
    }

    /* ClickedPlaceCanvas를 실행시키는 함수 */
    private void ActiveClickedPlaceCanvas()
    {
        /* PlaneButtons나 BuildButtons가 활성화 중이면 */
        if ((_buildButtons.activeInHierarchy || _planeButtons.activeInHierarchy) && !_activeBasePanelOnce)
        {
            /* BasePanel 활성화 */
            _basePanel.SetActive(true);

            /* BasePanel 1회 활성화 완료 */
            _activeBasePanelOnce = true;
        }
        /* PlaneButtons와 BuildButtons가 모두 비활성화 중이면 */
        else if ((!_buildButtons.activeInHierarchy && !_planeButtons.activeInHierarchy) && _activeBasePanelOnce)
        {
            /* BasePanel 비활성화 */
            _basePanel.SetActive(false);

            /* BasePanel 1회 비활성화 완료 */
            _activeBasePanelOnce = false;

            /* 크기 조절 Slider를 갱신 */
            ResetScaleSlider();

            /* Tiling InputField를 갱신 */
            ResetTilingInputField();

            /* 위치 InputField를 갱신 */
            ResetPositionInputField();
        }
    }

    /* 이름을 갱신하는 함수 */
    public void RenewalClickedPlaceName()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            _clickedPlaceName.text = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).name;
            _clickedPlaceName.color = new Color(0f, 222f / 255f, 1f);
        }
        /* 클릭된 건물이 없으면 */
        else
        {
            _clickedPlaceName.text = "None";
            _clickedPlaceName.color = Color.red;
        }
    }

    /* 크기를 조절하는 Slider가 조정될 때마다 실행되는 함수 */
    public void ScaleSlider()
    {
        /* Slider 갱신 중이지 않으면 */
        if (_placeController._clickedPlace && !_isRenewalScaleSlider)
        {
            /* 소수점 둘째 자리에서 반올림 한 값으로 변경 */
            float X = Mathf.Floor(_xSlider.value * 100f) * 0.01f;
            float Y = Mathf.Floor(_ySlider.value * 100f) * 0.01f;
            float Z = Mathf.Floor(_zSlider.value * 100f) * 0.01f;

            _placeController._clickedPlace.GetComponent<Transform>().localScale = new Vector3(X, Y, Z); //크기 변경

            /* 소수점 둘째 자리에서 반올림 한 값을 출력 */
            _xSliderValue.text = string.Format("{0:f2}", X);
            _ySliderValue.text = string.Format("{0:f2}", Y);
            _zSliderValue.text = string.Format("{0:f2}", Z);
        }
    }

    /* 크기를 조절하는 Slider를 초기화하는 함수 */
    public void ResetScaleSlider()
    {
        /* 클릭된 Place가 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 갱신 중*/
            _isRenewalScaleSlider = true;

            Vector3 _defaultScale = _placeController._clickedPlace.GetComponent<Transform>().localScale; //부모의 기본 크기

            _xSlider.value = _defaultScale.x; //클릭된 객체의 X축 크기에 맞도록 변경
            _ySlider.value = _defaultScale.y; //클릭된 객체의 Y축 크기에 맞도록 변경
            _zSlider.value = _defaultScale.z; //클릭된 객체의 Z축 크기에 맞도록 변경

            /* 소수점 둘째 자리에서 반올림 한 값을 출력 */
            _xSliderValue.text = "" + string.Format("{0:f2}", _defaultScale.x);
            _ySliderValue.text = "" + string.Format("{0:f2}", _defaultScale.y);
            _zSliderValue.text = "" + string.Format("{0:f2}", _defaultScale.z);

            /* 갱신 종료 */
            _isRenewalScaleSlider = false;
        }
    }

    /* 입력 값을 통해 X축 크기를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldScaleXSlider()
    {
        float Result; //결과를 담을 변수

        /* Float 형으로 변환할 수 있는 값을 적으면 */
        if (float.TryParse(_xSliderValue.text, out Result) && (Result >= _xSlider.minValue && Result <= _xSlider.maxValue))
        {
            /* 결과를 Float 형으로 변환하여 Slider 값 변경 */
            _xSlider.value = Mathf.Floor(Result * 100f) * 0.01f;

            /* Slider 값을 나타내는 InputField 변경 */
            _xSliderValue.text = string.Format("{0:f2}", Result);
        }

        /* Float 형으로 변환할 수 없는 값을 적으면 */
        else
        {
            /* Slider의 값으로 재 변경 */
            _xSliderValue.text = string.Format("{0:f2}", _xSlider.value);
        }
    }

    /* 입력 값을 통해 Y축 크기를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldScaleYSlider()
    {
        float Result; //결과를 담을 변수

        /* Float 형으로 변환할 수 있는 값을 적으면서 범위 내의 수이면 */
        if (float.TryParse(_ySliderValue.text, out Result) && (Result >= _ySlider.minValue && Result <= _ySlider.maxValue))
        {
            /* 결과를 Float 형으로 변환하여 Slider 값 변경 */
            _ySlider.value = Mathf.Floor(Result * 100f) * 0.01f;

            /* Slider 값을 나타내는 InputField 변경 */
            _ySliderValue.text = string.Format("{0:f2}", Result);
        }

        /* Float 형으로 변환할 수 없는 값을 적으면 */
        else
        {
            /* Slider의 값으로 재 변경 */
            _ySliderValue.text = string.Format("{0:f2}", _ySlider.value);
        }
    }

    /* 입력 값을 통해 Z축 크기를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldScaleZSlider()
    {
        float Result; //결과를 담을 변수

        /* Float 형으로 변환할 수 있는 값을 적으면 */
        if (float.TryParse(_zSliderValue.text, out Result) && (Result >= _zSlider.minValue && Result <= _zSlider.maxValue))
        {
            /* 결과를 Float 형으로 변환하여 Slider 값 변경 */
            _zSlider.value = Mathf.Floor(Result * 100f) * 0.01f;

            /* Slider 값을 나타내는 InputField 변경 */
            _zSliderValue.text = string.Format("{0:f2}", Result);
        }

        /* Float 형으로 변환할 수 없는 값을 적으면 */
        else
        {
            /* Slider의 값으로 재 변경 */
            _zSliderValue.text = string.Format("{0:f2}", _zSlider.value);
        }
    }

    /* ClickedPlaceCanvas의 Delete 버튼을 누르면 실행되는 함수 */
    public void DeleteClickedPlace()
    {
        /* 클릭된 Place가 있으면 */
        if (_placeController._clickedPlace)
        {
            HistoryController.pushObjectDeleteHist(_placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem.itemName, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.eulerAngles, _placeController._clickedPlace.transform.localScale);
            /* DB에서 제거 */
            _itemListControl.DeleteDBWall(_placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber);

            /* 제거 */
            Destroy(_placeController._clickedPlace);
        }
    }

    /* ClickedPlaceCanvas의 Replace 버튼을 누르면 실행되는 함수 */
    public void ReplaceClickedPlace()
    {
        /* 클릭된 객체가 있으면 */
        if (_placeController._clickedPlace)
        {
            Debug.Log("hist push");
            HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
            /* Replace라고 알림 */
            _placeController._isRelocated = true;

            /* 클릭된 Place를 이동할 Building으로 지정 */
            _placeController._locatedBuilding = _placeController._clickedPlace;

            /* RayCast 무시 */
            _placeController._locatedBuilding.GetComponent<Transform>().GetChild(0).gameObject.layer = 2;

            /* 카메라 움직일 수 없음 */
            _placeController._cameraMoveAroun._cameraAroun = false;

            /* 기준 Plane 활성화 */
            _placeController._measurePlane.SetActive(true);
        }
    }

    /* 각 축의 위치를 실시간으로 조절하는 함수 */
    public void PlacementAxis()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 각 축에 맞게 매 프레임마다 이동 */
            if (_checkPlacementAsix[0])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(7.5f * Time.deltaTime, 0f, 0f);
                ResetPositionInputField();
            }
            if (_checkPlacementAsix[1])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(-7.5f * Time.deltaTime, 0f, 0f);
                ResetPositionInputField();
            }
            if (_checkPlacementAsix[2])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(0f, 7.5f * Time.deltaTime, 0f);
                ResetPositionInputField();
            }
            if (_checkPlacementAsix[3])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(0f, -7.5f * Time.deltaTime, 0f);
                ResetPositionInputField();
            }
            if (_checkPlacementAsix[4])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(0f, 0f, 7.5f * Time.deltaTime);
                ResetPositionInputField();
            }
            if (_checkPlacementAsix[5])
            {
                _placeController._clickedPlace.GetComponent<Transform>().position += new Vector3(0f, 0f, -7.5f * Time.deltaTime);
                ResetPositionInputField();
            }
        }
    }

    /* X축 상승 버튼을 눌렀을 때 */
    public void AxisXUpDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[0] = true; //활성화
    }

    /* X축 상승 버튼을 땠을 때 */
    public void AxisXUpUp()
    {
        _checkPlacementAsix[0] = false; //비활성화
    }

    /* X축 하강 버튼을 눌렀을 때 */
    public void AxisXDownDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[1] = true; //활성화
    }

    /* X축 하강 버튼을 땠을 때 */
    public void AxisXDownUp()
    {
        _checkPlacementAsix[1] = false; //비활성화
    }

    /* Y축 상승 버튼을 눌렀을 때 */
    public void AxisYUpDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[2] = true; //활성화
    }

    /* Y축 상승 버튼을 땠을 때 */
    public void AxisYUpUp()
    {
        _checkPlacementAsix[2] = false; //비활성화
    }

    /* Y축 하강 버튼을 눌렀을 때 */
    public void AxisYDownDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[3] = true; //활성화
    }

    /* Y축 하강 버튼을 땠을 때 */
    public void AxisYDownUp()
    {
        _checkPlacementAsix[3] = false; //비활성화
    }

    /* Z축 상승 버튼을 눌렀을 때 */
    public void AxisZUpDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[4] = true; //활성화
    }

    /* Z축 상승 버튼을 땠을 때 */
    public void AxisZUpUp()
    {
        _checkPlacementAsix[4] = false; //비활성화
    }

    /* Z축 하강 버튼을 눌렀을 때 */
    public void AxisZDownDown()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
        _checkPlacementAsix[5] = true; //활성화
    }

    /* Z축 하강 버튼을 땠을 때 */
    public void AxisZDownUp()
    {
        _checkPlacementAsix[5] = false; //비활성화
    }

    /* Tiling을 조절하는 InputField를 초기화하는 함수 */
    public void ResetTilingInputField()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 클릭된 건물의 Mesh 담음 */
            MeshRenderer _mesh = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();

            /* X축 값을 소수 점 두 자리까지 출력 */
            _xTilingField.text = string.Format("{0:f2}", _mesh.material.mainTextureScale.x);

            /* Y축 값을 소수 점 두 자리까지 출력 */
            _yTilingField.text = string.Format("{0:f2}", _mesh.material.mainTextureScale.y);
        }
    }

    /* 입력 값을 통해 Tiling X축 크기를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldTilingX()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 실수인지 확인 후 결과 값을 담는 변수 */
            float _result;

            /* 클릭된 건물의 Mesh 담음 */
            MeshRenderer _mesh = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();
            HistoryController.pushTilingHist(_placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _mesh.material.mainTextureScale, -1);

            /* 실수 값을 입력하면 */
            if (float.TryParse(_xTilingField.text, out _result))
            {
                /* 입력 X축 값을 소수점 둘째 자리까지 할당 */
                _mesh.material.mainTextureScale = new Vector3(Mathf.Floor(_result * 100f) * 0.01f, _mesh.material.mainTextureScale.y);

                /* InputField 값을 소수점 둘째 자리까지 출력 */
                _xTilingField.text = string.Format("{0:f2}", _result);

                /* ItemObject 컴포넌트의 TilingX 값 지정 */
                _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._tilingX = Mathf.Floor(_result * 100f) * 0.01f;
            }
            /* 실수 값이 아니면 */
            else
            {
                /* 원래 InputField 값을 소수점 둘째 자리까지 출력 */
                _xTilingField.text = string.Format("{0:f2}", _mesh.material.mainTextureScale.x);
            }
        }
    }

    /* 입력 값을 통해 Tiling Y축 크기를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldTilingY()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 실수인지 확인 후 결과 값을 담는 변수 */
            float _result;

            /* 클릭된 건물의 Mesh 담음 */
            MeshRenderer _mesh = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();
            HistoryController.pushTilingHist(_placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _mesh.material.mainTextureScale, -1);

            /* 실수 값을 입력하면 */
            if (float.TryParse(_yTilingField.text, out _result))
            {
                /* 입력 Y축 값을 소수점 둘째 자리까지 할당 */
                _mesh.material.mainTextureScale = new Vector3(_mesh.material.mainTextureScale.x, Mathf.Floor(_result * 100f) * 0.01f);

                /* InputField 값을 소수점 둘째 자리까지 출력 */
                _yTilingField.text = string.Format("{0:f2}", _result);

                /* ItemObject 컴포넌트의 TilingY 값 지정 */
                _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._tilingY = Mathf.Floor(_result * 100f) * 0.01f;
            }
            /* 실수 값이 아니면 */
            else
            {
                /* 원래 InputField 값을 소수점 둘째 자리까지 출력 */
                _yTilingField.text = string.Format("{0:f2}", _mesh.material.mainTextureScale.y);
            }
        }
    }

    /* 도킹 버튼을 누르면 실행되는 함수 */
    public void DockingButton()
    {
        /* 도킹 기능이 활성화되어 있을 때 버튼을 누르면 */
        if (_isDocking)
        {
            /* 적색으로 색상 변경 */
            _dockingButtonImage.color = new Color(200f / 255f, 0f, 0f);

            /* 도킹 비활성화 */
            _isDocking = false;
        }

        /* 도킹 기능이 비활성화되어 있을 때 버튼을 누르면 */
        else
        {
            /* 녹색으로 색상 변경 */
            _dockingButtonImage.color = new Color(0f, 200f / 255f, 0f);

            /* 도킹 활성화 */
            _isDocking = true;
        }
    }

    /* 위치를 조절하는 InputField를 초기화하는 함수 */
    public void ResetPositionInputField()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 클릭된 건물의 위치 담음 */
            Vector3 _clickedPlacePosition = _placeController._clickedPlace.GetComponent<Transform>().position;

            /* X 위치 값을 소수 점 두 자리까지 출력 */
            _xPositionField.text = string.Format("{0:f2}", _clickedPlacePosition.x);

            /* Y 위치 값을 소수 점 두 자리까지 출력 */
            _yPositionField.text = string.Format("{0:f2}", _clickedPlacePosition.y);

            /* Z 위치 값을 소수 점 두 자리까지 출력 */
            _zPositionField.text = string.Format("{0:f2}", _clickedPlacePosition.z);
        }
    }

    /* 입력 값을 통해 X축 위치를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldXPosition()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 실수인지 확인 후 결과 값을 담는 변수 */
            float _result;

            /* 클릭된 건물의 Transform 컴포넌트 가져옴 */
            Transform _clickedPlaceTransform = _placeController._clickedPlace.GetComponent<Transform>();

            /* 실수 값을 입력하면 */
            if (float.TryParse(_xPositionField.text, out _result))
            {
                HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
                /* 입력 X축 값을 소수점 둘째 자리까지 할당 */
                _clickedPlaceTransform.position = new Vector3(Mathf.Floor(_result * 100f) * 0.01f, _clickedPlaceTransform.position.y, _clickedPlaceTransform.position.z);

                /* InputField 값을 소수점 둘째 자리까지 출력 */
                _xPositionField.text = string.Format("{0:f2}", _result);
            }
            /* 실수 값이 아니면 */
            else
            {
                /* 원래 InputField 값을 소수점 둘째 자리까지 출력 */
                _xPositionField.text = string.Format("{0:f2}", _clickedPlaceTransform.position.x);
            }
        }
    }

    /* 입력 값을 통해 Y축 위치를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldYPosition()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 실수인지 확인 후 결과 값을 담는 변수 */
            float _result;

            /* 클릭된 건물의 Transform 컴포넌트 가져옴 */
            Transform _clickedPlaceTransform = _placeController._clickedPlace.GetComponent<Transform>();

            /* 실수 값을 입력하면 */
            if (float.TryParse(_yPositionField.text, out _result))
            {
                HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
                /* 입력 Y축 값을 소수점 둘째 자리까지 할당 */
                _clickedPlaceTransform.position = new Vector3(_clickedPlaceTransform.position.x, Mathf.Floor(_result * 100f) * 0.01f, _clickedPlaceTransform.position.z);

                /* InputField 값을 소수점 둘째 자리까지 출력 */
                _yPositionField.text = string.Format("{0:f2}", _result);
            }
            /* 실수 값이 아니면 */
            else
            {
                /* 원래 InputField 값을 소수점 둘째 자리까지 출력 */
                _yPositionField.text = string.Format("{0:f2}", _clickedPlaceTransform.position.y);
            }
        }
    }

    /* 입력 값을 통해 Z축 위치를 조절할 수 있도록 하는 함수로써 OnEndEdit에 부착되어 있다 */
    public void InputFieldZPosition()
    {
        /* 클릭된 건물이 있으면 */
        if (_placeController._clickedPlace)
        {
            /* 실수인지 확인 후 결과 값을 담는 변수 */
            float _result;

            /* 클릭된 건물의 Transform 컴포넌트 가져옴 */
            Transform _clickedPlaceTransform = _placeController._clickedPlace.GetComponent<Transform>();

            /* 실수 값을 입력하면 */
            if (float.TryParse(_zPositionField.text, out _result))
            {
                HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
                /* 입력 Z축 값을 소수점 둘째 자리까지 할당 */
                _clickedPlaceTransform.position = new Vector3(_clickedPlaceTransform.position.x, _clickedPlaceTransform.position.y, Mathf.Floor(_result * 100f) * 0.01f);

                /* InputField 값을 소수점 둘째 자리까지 출력 */
                _zPositionField.text = string.Format("{0:f2}", _result);
            }
            /* 실수 값이 아니면 */
            else
            {
                /* 원래 InputField 값을 소수점 둘째 자리까지 출력 */
                _zPositionField.text = string.Format("{0:f2}", _clickedPlaceTransform.position.z);
            }
        }
    }

    public void SliderHistoryUpdate()
    {
        HistoryController.pushObjectHist(_placeController._clickedPlace, _placeController._clickedPlace.transform.position, _placeController._clickedPlace.transform.localScale, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.transform.rotation);
    }

    public void ResetForHistory()
    {

        /* 크기 조절 Slider를 갱신 */
        ResetScaleSlider();

        /* Tiling InputField를 갱신 */
        ResetTilingInputField();

        /* 위치 InputField를 갱신 */
        ResetPositionInputField();
    }
}