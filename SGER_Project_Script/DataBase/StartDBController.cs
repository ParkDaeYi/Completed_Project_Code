using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StartDBController : MonoBehaviour {

    /**
 * @date : 2020.04.26
 * @author : Day
 * @desc :  DB 혼선이 생겨
 *          SaveController.cs 와 VoiceFileController.cs 통합
 *          시작 시 TotalTable.sqlite , VoiceDirectory.sqlite, VoiceFile.sqlite
 *          를 방문해 변수 생성
 */

    [Header("VoiceObject")]
    public GameObject _directorySampleBtn;
    public GameObject _fileSampleBtn;
    public Transform _viewPort;
    public Transform _voiceContent;
    public Transform _content;
    public Text _pathText;
    public string _filePath;
    public GameObject _scroll_view;
    public GameObject _voice_canvas;
    public int _now;
    public string _start_path;
    public Transform _current_content;
    public Transform _all_content;
    public InputField _createInputField;

    [Header("SaveObject")]
    public GameObject _sampleButton; //저장 버튼
    public Transform _saveContent; //저장 버튼 생성하는 위치

    [Header("Script")]
    public ClickedItemControl _clickedItemControl;
    public HiddenMenuControl _hiddenMenuControl;
    public CurrentSearch _currentSearch;
    public AllSearch _allSearch;
    public VRCanvas _vrCanvas;
    public VRCanvas _vrAndroidCanvas;

    [Header("SaveInfo")]
    /*
     * GameDBController 에서 playedDic과 keyDic에 값을 넘겨줌
     */
    public Dictionary<string, int> playedDic = new Dictionary<string, int>();
    public Dictionary<string, int> keyDic = new Dictionary<string, int>();
    public Dictionary<string, string> timeDic = new Dictionary<string, string>();
    public List<string> saveFileName = new List<string>();

    /*
     * directoryInfo : string 은 directory name, int 는 key
     * directoryPoint : 첫 int 는 key, 나머지는 이전 key
     * directoryTime : string 은 directory name, 나머지는 마지막 수정 시간
     * directoryName : directory name 집합
     * fileInfo : int 는 key, 해당 key 에 있는 file들을 List<string>에 저장
     * fileTime : int 는 key, 해당 key 에 있는 file들의 마지막 수정 시간을 저장
     * fileName : file name 집합
     */
    [Header("VoiceInfo")]
    public Dictionary<string, int> directoryString = new Dictionary<string, int>();
    public Dictionary<int, string> directoryInteger = new Dictionary<int, string>();
    public Dictionary<int, int> directoryPoint = new Dictionary<int, int>();
    public Dictionary<int, Dictionary<string, string>> directoryTime = new Dictionary<int, Dictionary<string, string>>();
    public List<string> directoryName = new List<string>();
    public Dictionary<int, List<string>> fileInfo = new Dictionary<int, List<string>>();
    public Dictionary<int, Dictionary<string, string>> fileTime = new Dictionary<int, Dictionary<string, string>>();
    public Dictionary<int, Transform> contentInfo = new Dictionary<int, Transform>();
    public List<int> idxInfo = new List<int>();
    /*
     * int는 key 값, string은 filename (확장자 제외)
     */
    public Dictionary<int, Dictionary<string, AudioClip>> audioInfo = new Dictionary<int, Dictionary<string, AudioClip>>();
    public Dictionary<int, string> directoryPath = new Dictionary<int, string>();
    public Dictionary<int, List<int>> directoryPathIdx = new Dictionary<int, List<int>>();

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("StartDBController.cs 시작");
        _hiddenMenuControl = GameObject.Find("HiddenMenu").GetComponent<HiddenMenuControl>();
        /*
         * HiddenMenu에서 TotalTable 설정(?)을 마침.
         */
        yield return StartCoroutine(_hiddenMenuControl.Hidden_Start());

        foreach (string file in saveFileName)
        {
            //같은 파일 이름으로 key 값이 설정 되있어야함.
            if (playedDic.ContainsKey(file) && keyDic.ContainsKey(file))
            {
                GameObject tmp = Instantiate(_sampleButton) as GameObject;

                tmp.name = file;    //실행 횟수를 추가하기 위해 GameObject를 검색해야함.
                tmp.transform.GetChild(0).GetComponent<Text>().text = "<color=#0000ff>" + keyDic[file] + ". </color>" + file + " : <color=#ff0000>" + playedDic[file] + "</color>";
                tmp.transform.GetChild(2).GetComponent<Text>().text = timeDic[file];
                tmp.transform.SetParent(_saveContent);
                tmp.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                _vrCanvas._saveFileName.Add(file); //VRCanvas DataBase 파일 이름 리스트에 추가
                _vrAndroidCanvas._saveFileName.Add(file); //VRCanvas DataBase 파일 이름 리스트에 추가
            }
        }
        StartCoroutine(_vrCanvas.Init()); //VRCanvas 초기화 코루틴 함수 실행
        StartCoroutine(_vrAndroidCanvas.Init()); //VRCanvas 초기화 코루틴 함수 실행

        _clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();
        _currentSearch._fileTable = new Dictionary<int, Dictionary<string, GameObject>>();
        _allSearch._fileTable = new Dictionary<string, List<GameObject>>();

        yield return StartCoroutine(_clickedItemControl.ClickedItem_Start());

        _pathText.text = _filePath = ".";

        contentInfo.Add(0, _voiceContent);
        /*
         * Directory 컨텐츠 생성
         */
        foreach(string dir_name in directoryName)
        {
            if (!directoryString.ContainsKey(dir_name)) continue;
            int idx = directoryString[dir_name];
            if (!directoryPoint.ContainsKey(idx)) continue;
            int prev_idx = directoryPoint[idx];

            Transform _parent = _viewPort;
            Transform tmp_content = Instantiate(_content);
            if (!contentInfo.ContainsKey(idx))
            {
                tmp_content.SetParent(_viewPort);
                tmp_content.name = dir_name;
                tmp_content.localScale = new Vector3(1, 1, 1);
                tmp_content.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                tmp_content.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                tmp_content.GetChild(0).GetComponent<VoiceDirectoryButton>()._key = idx;
                tmp_content.GetChild(0).GetComponent<VoiceDirectoryButton>()._prev_key = prev_idx;
                tmp_content.gameObject.SetActive(false);
                contentInfo.Add(idx, tmp_content);
            }
        }

        StartCoroutine(MakePath(0, ""));

        /*
         * Directory 버튼 생성
         */
        foreach (string dir_name in directoryName)
        {
            if (!directoryString.ContainsKey(dir_name)) continue;
            int idx = directoryString[dir_name];
            if (!directoryPoint.ContainsKey(idx) || !contentInfo.ContainsKey(idx) || !directoryTime.ContainsKey(idx) || !directoryTime[idx].ContainsKey(dir_name))
                continue;
            int prev_idx = directoryPoint[idx];
            if (!contentInfo.ContainsKey(prev_idx) || !contentInfo.ContainsKey(idx)) continue;
            if (idx == 0 && prev_idx == 0) continue;

            Transform _parent = contentInfo[prev_idx];
            if (prev_idx == 0) _parent.gameObject.SetActive(true);

            GameObject tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
            tmp_btn.name = dir_name;
            tmp_btn.transform.SetParent(_parent);
            tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
            tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[idx][dir_name];
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = idx;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = prev_idx;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
            tmp_btn.SetActive(true);
            tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            string _lower = dir_name.ToLower();
            if (!_currentSearch._fileTable.ContainsKey(prev_idx))
            {
                Dictionary<string, GameObject> sg = new Dictionary<string, GameObject>();
                _currentSearch._fileTable.Add(prev_idx, sg);
            }
            if (!_currentSearch._fileTable[prev_idx].ContainsKey(_lower))
            {
                tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
                tmp_btn.name = _lower;
                tmp_btn.transform.SetParent(_current_content);
                tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
                tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[idx][dir_name];
                tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = idx;
                tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = prev_idx;
                tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
                tmp_btn.SetActive(false);
                tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                _currentSearch._fileTable[prev_idx].Add(_lower, tmp_btn);
            }
            if (!_allSearch._fileTable.ContainsKey(_lower))
            {
                _allSearch._fileTable[_lower] = new List<GameObject>();
            }
            tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
            tmp_btn.name = _lower;
            tmp_btn.transform.SetParent(_all_content);
            tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
            tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[idx][dir_name];
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = idx;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = prev_idx;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
            tmp_btn.SetActive(false);
            tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            _allSearch._fileTable[_lower].Add(tmp_btn);
        }

        /*
         * file 버튼 생성
         */
        foreach (string dir_name in directoryName)
        {
            if (!directoryString.ContainsKey(dir_name)) continue;
            int idx = directoryString[dir_name];

            if (!fileInfo.ContainsKey(idx) || !fileTime.ContainsKey(idx) || !audioInfo.ContainsKey(idx)) continue;
            Transform _parent = contentInfo[idx];

            foreach (string file_name in fileInfo[idx])
            {
                string[] tmp_name = file_name.Split('.');
                if (!fileTime[idx].ContainsKey(file_name) || !audioInfo[idx].ContainsKey(tmp_name[0])) continue;
                GameObject tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
                tmp_btn.name = file_name;
                tmp_btn.transform.SetParent(_parent);
                tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name;
                tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[idx][file_name];
                tmp_btn.GetComponent<AudioSource>().clip = audioInfo[idx][tmp_name[0]];
                tmp_btn.GetComponent<VoiceFileButton>()._key = idx;
                tmp_btn.GetComponent<VoiceFileButton>()._file_name = tmp_name[0];
                tmp_btn.SetActive(true);
                tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                string _lower = file_name.ToLower();
                if (!_currentSearch._fileTable.ContainsKey(idx))
                {
                    Dictionary<string, GameObject> sg = new Dictionary<string, GameObject>();
                    _currentSearch._fileTable.Add(idx, sg);
                }
                if (!_currentSearch._fileTable[idx].ContainsKey(_lower))
                {
                    tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
                    tmp_btn.name = _lower;
                    tmp_btn.transform.SetParent(_current_content);
                    tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name;
                    tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[idx][file_name];
                    tmp_btn.GetComponent<AudioSource>().clip = audioInfo[idx][tmp_name[0]];
                    tmp_btn.GetComponent<VoiceFileButton>()._key = idx;
                    tmp_btn.GetComponent<VoiceFileButton>()._file_name = tmp_name[0];
                    tmp_btn.SetActive(false);
                    tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                    _currentSearch._fileTable[idx].Add(_lower, tmp_btn);
                }
                if (!_allSearch._fileTable.ContainsKey(_lower))
                {
                    _allSearch._fileTable[_lower] = new List<GameObject>();
                }
                tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
                tmp_btn.name = _lower;
                tmp_btn.transform.SetParent(_all_content);
                tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name;
                tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[idx][file_name];
                tmp_btn.GetComponent<AudioSource>().clip = audioInfo[idx][tmp_name[0]];
                tmp_btn.GetComponent<VoiceFileButton>()._key = idx;
                tmp_btn.GetComponent<VoiceFileButton>()._file_name = tmp_name[0];
                tmp_btn.SetActive(false);
                tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                _allSearch._fileTable[_lower].Add(tmp_btn);
            }
        }

        _allSearch.root = new AllSearch.Trie();
        //idxInfo.Sort();
        foreach(int i in idxInfo)
        {
            if (directoryInteger.ContainsKey(i)) _allSearch.root.insert(directoryInteger[i] + '\0', 0);
            if (fileInfo.ContainsKey(i))
            {
                foreach (string str in fileInfo[i])
                {
                    _allSearch.root.insert(str + '\0', 0);
                }
            }
        }
    }

    IEnumerator MakePath(int idx,string path) 
    {
        if (idx == 0) path += ".";
        else path += "/" + contentInfo[idx].name;

        if (!directoryPath.ContainsKey(idx))
        {
            directoryPath.Add(idx, path);
        }
        if (directoryPathIdx.ContainsKey(idx))
        {
            for (int i = 0; i < directoryPathIdx[idx].Count; ++i)
            {
                StartCoroutine(MakePath(directoryPathIdx[idx][i], path));
            }
        }

        yield return null;
    }

    public void OnClickCancel()
    {
        _pathText.text = _filePath = _start_path;
        contentInfo[_now].gameObject.SetActive(false);
        if (contentInfo.ContainsKey(0))
        {
            contentInfo[0].gameObject.SetActive(true);
            _scroll_view.GetComponent<Transform>().GetComponent<ScrollRect>().content = contentInfo[0].GetComponent<RectTransform>();
        }
        _now = 0;
        AllFalse();
    }

    public void AllFalse()
    {
        _scroll_view.SetActive(false);
        _voice_canvas.SetActive(false);
    }

    public void DeleteDirectory(int idx)
    {
        if (directoryPath.ContainsKey(idx)) directoryPath.Remove(idx);
        if (directoryPoint.ContainsKey(idx)) directoryPoint.Remove(idx);
        if (directoryTime.ContainsKey(idx)) directoryTime.Remove(idx);
        if (fileInfo.ContainsKey(idx)) fileInfo.Remove(idx);
        if (fileTime.ContainsKey(idx)) fileTime.Remove(idx);
        if (idxInfo.Contains(idx)) idxInfo.Remove(idx);
        if (audioInfo.ContainsKey(idx)) audioInfo.Remove(idx);
        if (directoryInteger.ContainsKey(idx))
        {
            if (directoryString.ContainsKey(directoryInteger[idx])) directoryString.Remove(directoryInteger[idx]);
            if (directoryName.Contains(directoryInteger[idx])) directoryName.Remove(directoryInteger[idx]);
            directoryInteger.Remove(idx);
        }
        if (contentInfo.ContainsKey(idx)) contentInfo.Remove(idx);

        if (directoryPathIdx.ContainsKey(idx))
        {
            foreach (int i in directoryPathIdx[idx])
            {
                DeleteDirectory(i);
            }
            directoryPathIdx.Remove(idx);
        }

        VRDBController.VoiceDirectory_DeleteKey(idx);

        VRDBController.VoiceFile_DeleteKey(idx);
    }

    public void DeleteFile(int idx, string name)
    {
        string[] name2 = name.Split('.');
        if (audioInfo.ContainsKey(idx))
            if (audioInfo[idx].ContainsKey(name2[0])) audioInfo[idx].Remove(name2[0]);
        if (fileInfo.ContainsKey(idx)) fileInfo[idx].Remove(name);
        if (fileTime.ContainsKey(idx))
            if (fileTime[idx].ContainsKey(name)) fileTime[idx].Remove(name);

        VRDBController.VoiceFile_DeleteName(idx, name);
    }

    public IEnumerator CreateDirectory(string cur_path, string dir_name)
    {
        int cur_idx = -1;
        for (int i = 0; i < idxInfo.Count; ++i)
        {
            if (cur_idx < idxInfo[i]) cur_idx = idxInfo[i];
        }
        idxInfo.Add(++cur_idx);
        if (!directoryString.ContainsKey(dir_name)) directoryString.Add(dir_name, cur_idx);
        if (!directoryInteger.ContainsKey(cur_idx)) directoryInteger.Add(cur_idx, dir_name);
        if (!directoryPoint.ContainsKey(cur_idx)) directoryPoint.Add(cur_idx, _now);
        DirectoryInfo dir = new DirectoryInfo(@"" + Static.STATIC.dir_path + "/Resources/Voice" + cur_path + "/" + dir_name);
        string dir_time = dir.LastWriteTime.ToString();
        string[] lastWrite = dir_time.Split(' ');
        if (!directoryTime.ContainsKey(cur_idx))
        {
            Dictionary<string, string> ss = new Dictionary<string, string>();
            directoryTime.Add(cur_idx, ss);
        }
        if (!directoryTime[cur_idx].ContainsKey(dir_name)) directoryTime[cur_idx].Add(dir_name, lastWrite[0]);
        directoryName.Add(dir_name);
        if (!directoryPath.ContainsKey(cur_idx)) directoryPath.Add(cur_idx, "." + cur_path + "/" + dir_name);
        directoryPathIdx[_now].Add(cur_idx);

        // DB
        VRDBController.ConIn(Static.STATIC.dir_path + "/DataBase/VoiceDirectory");
        VRDBController.VoiceDirectory_add(dir_name, lastWrite[0], cur_idx, _now);
        Debug.Log("Name: " + dir_name + ", Time:" + lastWrite[0] + ", idx:" + cur_idx + ", prev_idx:" + _now);

        // 컨텐츠 생성
        Transform _parent = _viewPort;
        Transform tmp_content = Instantiate(_content);
        if (!contentInfo.ContainsKey(cur_idx))
        {
            tmp_content.SetParent(_viewPort);
            tmp_content.name = dir_name;
            tmp_content.localScale = new Vector3(1, 1, 1);
            tmp_content.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            tmp_content.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            tmp_content.GetChild(0).GetComponent<VoiceDirectoryButton>()._key = cur_idx;
            tmp_content.GetChild(0).GetComponent<VoiceDirectoryButton>()._prev_key = _now;
            tmp_content.gameObject.SetActive(false);
            contentInfo.Add(cur_idx, tmp_content);
        }

        // 버튼 생성
        _parent = contentInfo[_now];

        GameObject tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
        tmp_btn.name = dir_name;
        tmp_btn.transform.SetParent(_parent);
        tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
        tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[cur_idx][dir_name];
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = cur_idx;
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _now;
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
        tmp_btn.SetActive(true);
        tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        string _lower = dir_name.ToLower();
        if (!_currentSearch._fileTable.ContainsKey(_now))
        {
            Dictionary<string, GameObject> sg = new Dictionary<string, GameObject>();
            _currentSearch._fileTable.Add(_now, sg);
        }
        if (!_currentSearch._fileTable[_now].ContainsKey(_lower))
        {
            tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
            tmp_btn.name = _lower;
            tmp_btn.transform.SetParent(_current_content);
            tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
            tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[cur_idx][dir_name];
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = cur_idx;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _now;
            tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
            tmp_btn.SetActive(false);
            tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            _currentSearch._fileTable[_now].Add(_lower, tmp_btn);
        }
        if (!_allSearch._fileTable.ContainsKey(_lower))
        {
            _allSearch._fileTable[_lower] = new List<GameObject>();
        }
        tmp_btn = Instantiate(_directorySampleBtn) as GameObject;
        tmp_btn.name = _lower;
        tmp_btn.transform.SetParent(_all_content);
        tmp_btn.transform.GetChild(0).GetComponent<Text>().text = dir_name;
        tmp_btn.transform.GetChild(2).GetComponent<Text>().text = directoryTime[cur_idx][dir_name];
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._key = cur_idx;
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._prev_key = _now;
        tmp_btn.transform.GetComponent<VoiceDirectoryButton>()._change_btn = false;
        tmp_btn.SetActive(false);
        tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        _allSearch._fileTable[_lower].Add(tmp_btn);
        _allSearch.root.insert(_lower + '\0', 0);

        yield return null;
    }

    public IEnumerator CreateFile(string file_name,string extension, string lastWrite)
    {
        // 새로 만든 폴더일 경우 fileInfo엔 정보가 없을 거임.
        if (!fileInfo.ContainsKey(_now)) fileInfo[_now] = new List<string>();     
        fileInfo[_now].Add(file_name + extension);
        if (!fileTime.ContainsKey(_now))
        {
            Dictionary<string, string> ss = new Dictionary<string, string>();
            fileTime.Add(_now, ss);
        }
        if (!fileTime[_now].ContainsKey(file_name + extension)) fileTime[_now].Add(file_name + extension, lastWrite);
        string _path = "Voice" + directoryPath[_now].Substring(1, directoryPath[_now].Length - 1);
        AudioClip tmp_audio = Resources.Load<AudioClip>(_path + "/" + file_name);
        if (!audioInfo.ContainsKey(_now))
        {
            Dictionary<string, AudioClip> sa = new Dictionary<string, AudioClip>();
            audioInfo.Add(_now, sa);
        }
        if (!audioInfo[_now].ContainsKey(file_name)) audioInfo[_now].Add(file_name, tmp_audio);

        // DB
        VRDBController.ConIn(Static.STATIC.dir_path + "/DataBase/VoiceFile");
        VRDBController.VoiceFile_add(file_name + extension, lastWrite, _now);
        VRDBController.VoiceAudio_add(file_name, _path + "/", _now);

        // 버튼 생성
        Transform _parent = contentInfo[_now];
        GameObject tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
        tmp_btn.name = file_name + extension;
        tmp_btn.transform.SetParent(_parent);
        tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name + extension;
        tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[_now][file_name + extension];
        tmp_btn.GetComponent<AudioSource>().clip = audioInfo[_now][file_name];
        tmp_btn.GetComponent<VoiceFileButton>()._key = _now;
        tmp_btn.GetComponent<VoiceFileButton>()._file_name = file_name;
        tmp_btn.SetActive(true);
        tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        string _lower = (file_name + extension).ToLower();
        if (!_currentSearch._fileTable.ContainsKey(_now))
        {
            Dictionary<string, GameObject> sg = new Dictionary<string, GameObject>();
            _currentSearch._fileTable.Add(_now, sg);
        }
        if (!_currentSearch._fileTable[_now].ContainsKey(_lower))
        {
            tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
            tmp_btn.name = _lower;
            tmp_btn.transform.SetParent(_current_content);
            tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name + extension;
            tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[_now][file_name + extension];
            tmp_btn.GetComponent<AudioSource>().clip = audioInfo[_now][file_name];
            tmp_btn.GetComponent<VoiceFileButton>()._key = _now;
            tmp_btn.GetComponent<VoiceFileButton>()._file_name = file_name;
            tmp_btn.SetActive(false);
            tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            _currentSearch._fileTable[_now].Add(_lower, tmp_btn);
        }
        if (!_allSearch._fileTable.ContainsKey(_lower))
        {
            _allSearch._fileTable[_lower] = new List<GameObject>();
        }
        tmp_btn = Instantiate(_fileSampleBtn) as GameObject;
        tmp_btn.name = _lower;
        tmp_btn.transform.SetParent(_all_content);
        tmp_btn.transform.GetChild(0).GetComponent<Text>().text = file_name + extension;
        tmp_btn.transform.GetChild(2).GetComponent<Text>().text = fileTime[_now][file_name + extension];
        tmp_btn.GetComponent<AudioSource>().clip = audioInfo[_now][file_name];
        tmp_btn.GetComponent<VoiceFileButton>()._key = _now;
        tmp_btn.GetComponent<VoiceFileButton>()._file_name = file_name;
        tmp_btn.SetActive(false);
        tmp_btn.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        _allSearch._fileTable[_lower].Add(tmp_btn);
        _allSearch.root.insert(_lower + '\0', 0);

        yield return null;
    }
}
