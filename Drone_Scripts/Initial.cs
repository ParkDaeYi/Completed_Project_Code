using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initial : MonoBehaviour
{
    /* Scene 초기화 하기 위한 스크립트 */
    private Transform _drone;
    private Drone _droneScript;
    private Transform Player;

    void Start()
    {
        if (Static._isStart) //시작해야 하는가?
        {
            Static._isStart = false;
            return; //시작이면 아무것도 안함
        }

        _drone = GameObject.Find("Drone_red").GetComponent<Transform>(); //드론을 담음
        _drone.position = Static._dronePosition; //드론 위치 초기화
        _drone.rotation = Static._droneRotation; //드론 회전 초기화

        _droneScript = _drone.GetComponent<Drone>(); //드론 스크립트 담음
        _droneScript._StartUp = Static._droneStartUp; //드론 시동 켬

        if (SceneManager.GetActiveScene().buildIndex == 1) //1인칭 화면인 경우에는
        {
            Player = GameObject.Find("FirstPerson").GetComponent<Transform>();
            Player.position = Static._playerPosition; //플레이어 위치 설정
            Player.rotation = Static._playerRotation; //플레이어 회전 설정
        }
    }
}
