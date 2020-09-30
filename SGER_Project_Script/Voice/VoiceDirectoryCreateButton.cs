using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class VoiceDirectoryCreateButton : MonoBehaviour {

    [Header("Voice Panel")]
    public Button _createButton;
    public InputField _createField;

    [Header("Controller")]
    public StartDBController _startDBController;

    [Header("Voice Object")]
    public GameObject _sampleBtn;

    private void Start()
    {
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    public void OnClickPlusButton()
    {
        _createButton.gameObject.SetActive(false);
        _createField.gameObject.SetActive(true);
    }

    public void OnClickCreateDirectory()
    {
        Debug.Log("여긴 되나?");
        string _dir_name = _createField.text;
        if (_dir_name == "")
        {
            _createButton.gameObject.SetActive(true);
            _createField.gameObject.SetActive(false);
            _createField.text = "";
            return;
        }
        _dir_name = _createField.text;
        StartDBController _sd = _startDBController;
        if (_sd.directoryString.ContainsKey(_dir_name))
        {
            Debug.Log("여기서 막히나?");
            // 이미 이 이름의 디렉토리가 있어서 빠꾸쳐야함
            return;
        }
        /*
         * Directory 버튼 생성 부분인데 이거 솔직히 idx를 내가 직접 insert 해준다해도 prev_idx하고 다 찾아야하는데 난 자신없어서 냄겨둡니다.
         */
        //int _idx = _sd.directoryString[_createField.text];
        //int _prev_idx = _sd.directoryPoint[_idx];
        //Transform _parent = _sd.contentInfo[_prev_idx];
        //GameObject _tmp_btn = Instantiate(_startDBController._directorySampleBtn) as GameObject;
        //_tmp_btn.name = _dir_name;
        //_tmp_btn.transform.SetParent(_parent);
        //_tmp_btn.transform.GetChild(0).GetComponent<Text>().text = _dir_name;
        //_tmp_btn.transform.GetChild(2).GetComponent<Text>().text = _sd.directoryTime[_idx][_dir_name];
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = _idx;
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _prev_idx;
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
        //_tmp_btn.SetActive(true);
        //_tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        //string _lower = _dir_name.ToLower();
        //if (!_sd._currentSearch._fileTable.ContainsKey(_prev_idx))
        //{
        //    Dictionary<string, GameObject> sg = new Dictionary<string, GameObject>();
        //    _sd._currentSearch._fileTable.Add(_prev_idx, sg);
        //}
        //if (!_sd._currentSearch._fileTable[_prev_idx].ContainsKey(_lower))
        //{
        //    _tmp_btn = Instantiate(_sd._directorySampleBtn) as GameObject;
        //    _tmp_btn.name = _lower;
        //    _tmp_btn.transform.SetParent(_sd._current_content);
        //    _tmp_btn.transform.GetChild(0).GetComponent<Text>().text = _dir_name;
        //    _tmp_btn.transform.GetChild(2).GetComponent<Text>().text = _sd.directoryTime[_idx][_dir_name];
        //    _tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = _idx;
        //    _tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _prev_idx;
        //    _tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
        //    _tmp_btn.SetActive(false);
        //    _tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        //    _sd._currentSearch._fileTable[_prev_idx].Add(_lower, _tmp_btn);
        //}
        //if (!_sd._allSearch._fileTable.ContainsKey(_lower))
        //{
        //    _sd._allSearch._fileTable[_lower] = new List<GameObject>();
        //}
        //_tmp_btn = Instantiate(_sd._directorySampleBtn) as GameObject;
        //_tmp_btn.name = _lower;
        //_tmp_btn.transform.SetParent(_sd._all_content);
        //_tmp_btn.transform.GetChild(0).GetComponent<Text>().text = _dir_name;
        //_tmp_btn.transform.GetChild(2).GetComponent<Text>().text = _sd.directoryTime[_idx][_dir_name];
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = _idx;
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _prev_idx;
        //_tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
        //_tmp_btn.SetActive(false);
        //_tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        //_sd._allSearch._fileTable[_lower].Add(_tmp_btn);

        string _voiceDirectoryCreatePath;
        if (_sd._now > 0) _voiceDirectoryCreatePath = _sd.directoryPath[_sd._now].Substring(1, _sd.directoryPath[_sd._now].Length - 1);
        else _voiceDirectoryCreatePath = "";

        string _folderPath = @"" + Static.STATIC.dir_path + "/Resources/Voice" + _voiceDirectoryCreatePath + "/" + _dir_name;
        Directory.CreateDirectory(_folderPath);
        Debug.Log(_folderPath);
#if UNITY_EDITOR
        ImportAsset.NewImportAsset_File("Assets/Resources/Voice" + _voiceDirectoryCreatePath + "/" + _dir_name);
#endif

        //버튼, 컨텐츠 생성
        StartCoroutine(_sd.CreateDirectory(_voiceDirectoryCreatePath, _dir_name));

        _createButton.gameObject.SetActive(true);
        _createField.gameObject.SetActive(false);
        _createField.text = "";
    }

    public void OnClickCreateCancel()
    {
        _createButton.gameObject.SetActive(true);
        _createField.gameObject.SetActive(false);
    }
}
