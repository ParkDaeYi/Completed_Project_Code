using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour {
    /**
* date 2019.03.26
* author GS,INHO
* desc
*  VoiceCanvas와 VoiceMadeCanvas에 대한 모든 컨트롤을 담담하는 스크립트.
*/

    [Header("Script")]
    public StartDBController _startDBController;

    [Header("UI")]
    public GameObject _voiceMadeCanvas;

    [Header("Dynamic")]
    /* VoiceMadeCanvas의 제목 입력필드 */
    public InputField _titleInputField;
    /* VoiceMadeCanvas의 대사 입력필드 */
    public InputField _voiceInputField;

    [Header("Parent Panel")]
    public GameObject _manPanel;
    public GameObject _womanPanel;

    [Header("SampleButton")]
    public GameObject _sampleButton;
    public AudioClip _audioClip;

    GameObject _dynamicPanel = null; //어느 부모 패널에 생성 되는지?
    GameObject _instantiateSample; //어느 버튼에 오디오가 붙는지?

    string dir_path;

    private void Start()
    {
        dir_path = Static.STATIC.dir_path;
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    private void Update()
    {
        InitializeInputField();
        //ConnectAudio();
    }

    /* InputField 초기화 함수 */
    private void InitializeInputField()
    {
        /* VoiceMadeCanvas가 비활성화 상태이면 */
        if (!_voiceMadeCanvas.activeSelf)
        {
            /* 제목 InputField 초기화 */
            _titleInputField.text = "";
            /* 대사 InputField 초기화 */
            _voiceInputField.text = "";
        }
    }

        public void MoveVoiceFile(string path, string title)
    {
        StartDBController _sd = _startDBController;
        DirectoryInfo dir = new DirectoryInfo(path);

        /* 생성되는 파일을 옮길 경로 및 이름 표기 */
        string loadDir = dir_path + "/Resources/Voice";
        string modelDir = null;

        foreach (FileInfo file in dir.GetFiles())
        {
            /* 메타 파일이 생성됬을 때 지우도록! */
            DeleteFile(path, file.Name, ".meta");

            /* 생성된 음성 파일 -> 각 모델 디렉토리 이동 */
            if (file.Extension.ToLower().CompareTo(".wav") == 0)
            {
                // 여기 수정
                if (_sd._now > 0 && modelDir == null)
                {
                    modelDir = _sd.directoryPath[_sd._now].Substring(1, _sd.directoryPath[_sd._now].Length - 1);
                    loadDir += modelDir + "/" + title + ".wav";
                }

                /* 같은 이름을 가진 파일이 있으면, 덮어 씌우도록 설정. */
                if (ExistSameNameFile(loadDir))
                {
                    file.CopyTo(@loadDir, true);
                    file.Delete();
                    /* 유니티 Slot에도 덮어 씌울 수 있도록 코드 수정 필요 */
                }
                else // 새 파일 생성 시 버튼도 같이 생성!
                {
                    file.MoveTo(loadDir);
                    string tmp = file.LastWriteTime.ToString();
                    string[] lastWrite = tmp.Split(' ');

#if UNITY_EDITOR
                    /* 동적으로 폴더에 있는 데이터 임포트! -> 유니티 상에서 바로 사용 할 수 있도록
                     * 에디터 모드에만 적용되는 소스를 전처리기 적용 우회시켜 빌드 모드에서도 적용할 수 있도록 설정!
                     */
                    ImportAsset.NewImportAsset_Dic("Assets/Resources/Voice" + modelDir);
#endif
                    // 여기 수정
                    /* 음성 Slot을 만들어준다. */
                    StartCoroutine(_startDBController.CreateFile(title, ".wav", lastWrite[0]));
                }
                //
            }

            //냅두고
            /* 생성된 그래프 파일 -> 삭제 */
            else if (file.Extension.ToLower().CompareTo(".png") == 0)
            {
                file.Delete(); // 파일 삭제
            }

        }//foreach
    }
    
    /* 해당 경로에 디렉터리가 같은 파일(=> 이름이 같은 파일)이 존재하는지? */
    bool ExistSameNameFile(string path)
    {
        FileInfo check = new FileInfo(path);
        if (check.Exists) return true;
        else return false;
    }

    /* 메타파일을 지우기 위해 만든 메서드 */
    void DeleteFile(string path, string name,string extension)
    {
        /* 지울 메타파일을 경로를 통해 들고온다 */
        FileInfo file = new FileInfo(path + "/" + name + extension);
        if (file.Exists) file.Delete();
    }
    
}