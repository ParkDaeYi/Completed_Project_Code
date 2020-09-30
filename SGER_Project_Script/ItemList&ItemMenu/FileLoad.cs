using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FileLoad : MonoBehaviour {
    /**
* date 2018.07.22
* author Lugub
* desc
*  파일에서 불러오는 방식이 DB에도 쓰이고 Dress에도 쓰이고 Voice에도 쓰이고
*  워낙 많이 쓰여지게 되서 코드를 통합시켜서 인자값과 호출만으로 수행하게끔 만들었음
*  
*  아이템리스트 불러오는 과정은 이밖에도 다른 처리를 해야하기 때문에 이걸 사용하지는 않았음
*  
*  인자 값은 
*  _folderPath   폴더의 위치
*  _extension    확장자
*  _parentObject 생성될 객체의 부모
*  _sampleButton 생성될 객체 
*  _swiVertHori  0 : Vertical
*                1 : Horizon
*/

    public void FileLoadControll(string _folderPath, string _extension, GameObject _parentObject, GameObject _sampleButton, int _swiVertHori)

    {
        RectTransform _rect = _parentObject.GetComponent<RectTransform>();

        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_folderPath);

        _folderPath = _folderPath.Substring(17, _folderPath.Length - 17); //Resources 를 Load 하기 위해 (경로문제)

        int _size = 0;

        foreach (System.IO.FileInfo file in dir.GetFiles())
        {
            if (file.Extension.ToLower().CompareTo(_extension) == 0)
            {
                string _filename = file.Name.Substring(0, file.Name.Length - _extension.Length);

                /* 샘플 버튼 생성 */
                GameObject _instantiateSample = Instantiate(_sampleButton) as GameObject;

                /* 보통 샘플버튼은 비활성화 되어있는 경우가 많으므로 */
                _instantiateSample.SetActive(true);

                /* 샘플 버튼의 이름 설정 */
                _instantiateSample.name = _filename;

                /* 샘플 버튼의 텍스트 설정 */
                _instantiateSample.transform.GetChild(0).GetComponent<Text>().text = _filename;

                /*슬롯의 이미지 (스프라이트) 설정*/
                _instantiateSample.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(_folderPath + _filename);

                /* 샘플 버튼의 부모 설정 */
                _instantiateSample.transform.SetParent(_parentObject.transform);

                /* 샘플 버튼의 스케일 설정 */
                _instantiateSample.transform.localScale = new Vector3(1, 1, 1);

                _size++;

            }
        }

        float _buttonSampleSize = 0;

        switch (_swiVertHori)
        {
            case 0:
                _buttonSampleSize = _sampleButton.GetComponent<RectTransform>().rect.height;
                _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size * _buttonSampleSize + 70);
                break;
            case 1:
                _buttonSampleSize = _sampleButton.GetComponent<RectTransform>().rect.width;
                _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size * _buttonSampleSize + 70);
                break;
        }
        
        
    }

}
