using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [Header("Components")]
    public Transform _droneTransform; //드론 Transform
    public Drone _droneDrone; //드론 Drone 스크립트

    public Vector3 _initialDronePosition; //초기 드론 Position
    public Quaternion _initialDroneRotation; //초기 드론 Rotation

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            OnClickResetButton();
        }

        OnClickSceneChange();
    }
    private void Start()
    {
        _initialDronePosition = _droneTransform.position; //초기 드론 Position 저장
        _initialDroneRotation = _droneTransform.rotation; //초기 드론 Rotation 저장
    }

    /* Reset 버튼을 클릭했을 시*/
    public void OnClickResetButton()
    {
        _droneTransform.position = _initialDronePosition; //초기 Position으로
        _droneTransform.rotation = _initialDroneRotation; //초기 Rotation으로
        _droneDrone._StartUp = false; //드론 스크립트 활성화
    }

    public void OnClickSceneChange()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) //인칭 변환을 누르면
        { 
            Static._dronePosition = _droneTransform.position; //드론 위치 담음
            Static._droneRotation = _droneTransform.rotation; //드론 회전 담음
            Static._droneStartUp = _droneDrone._StartUp; //드론 시동 담음

            if (SceneManager.GetActiveScene().buildIndex == 1) //1인칭 씬이면
            {
                Transform _player = GameObject.Find("FirstPerson").GetComponent<Transform>();
                Static._playerPosition = _player.position; //플레이어 위치 담음
                Static._playerRotation = _player.rotation; //플레이어 회전 담음

                SceneManager.LoadScene(2);
            }
            else //3인칭 씬이면
            {
                SceneManager.LoadScene(1);
            }

        }
    }
}
