using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Teleport : MonoBehaviour
{
    /**
* date 2019.07.2
* author GS
* desc
* 집 오브젝트가 복잡해짐에 따라 텔레포트 기능을 구현하기 위해서 제작된 스크립트
* Position 오브젝트에 컴포넌트로 달려있는 스크립트
*/

    [Header("Component")]
    public Transform _camera; //카메라
    public SpriteRenderer _positionSpriteRenderer;
    public Collider _positionCollider; //충돌

    [Header("Variable")]
    public float _distance; //거리

    [Header("Script")]
    public LocateItem _locateItem;
    public TimeController _timeController;

    private void Start()
    {
        _camera = GameObject.Find("CameraController").transform; //카메라 추적
        _positionSpriteRenderer = GetComponent<SpriteRenderer>(); //SpriteRenderer 컴포넌트 담음
        _positionCollider = GetComponent<Collider>(); //Collider 담음
        _locateItem = GameObject.Find("Controllers").transform.Find("ItemController").GetComponent<LocateItem>();
        _timeController = GameObject.Find("Controllers").transform.Find("SchedulerController").GetComponent<TimeController>();
    }

    private void Update()
    {
        _distance = Vector3.Distance(_camera.position, transform.position); //거리를 계산
        transform.LookAt(_camera); //카메라를 계속해서 바라봄
        Transparency(); //투명도 함수
        ComponentControl(); //Component 조절 함수
    }

    /* UI를 클릭하면 실행되는 함수 */
    private void OnMouseDown()
    {
        /* Canvas 상의 UI를 클릭하지 않을 시 */
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _camera.position = new Vector3(transform.position.x, _camera.position.y, transform.position.z); //현재 UI 위치로 변경
        }
    }

    /* 투명도 조절 함수 */
    private void Transparency()
    {
        /* 투명도를 카메라와의 거리에 따라 실시간으로 조절(40 ~ 75) */
        _positionSpriteRenderer.color = new Color(_positionSpriteRenderer.color.r, _positionSpriteRenderer.color.g, _positionSpriteRenderer.color.b, (_distance - 40f) * (1f / 35f));
    }

    /* Component 조절 함수 */
    private void ComponentControl()
    {
        _positionCollider.enabled = !(_distance < 40f) && !_locateItem._placableGameObject && !_timeController._sw; // 거리가 가깝거나 물건을 배치 중이거나 스케줄러가 정지 시에 활성화
        _positionSpriteRenderer.enabled = !_timeController._sw && !_locateItem._placableGameObject; //스케줄러가 재생 중이 아니면써 물건을 배치시키지 않으면 이미지 활성화
    }
}