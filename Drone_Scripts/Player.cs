using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Eye; //눈 객체
    public float MoveSpeed; //이동 속도
    public float RotateSpeed; //회전 속도
    void Start()
    {
        Eye = this.transform.Find("Eye"); //눈 담음
    }

    void Update()
    {
        MovePlayer();
        RotateEye();
    }

    void MovePlayer() //플레이어 움직임
    {
        if (Input.GetKey(KeyCode.W)) //W키를 눌렀을 때
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);
        }

        if(Input.GetKey(KeyCode.A)) //A키를 눌렀을 때
        {
            this.transform.Translate(Vector3.left * Time.deltaTime * MoveSpeed);
        }

        if (Input.GetKey(KeyCode.S)) //S키를 눌렀을 때
        {
            this.transform.Translate(Vector3.back * Time.deltaTime * MoveSpeed);
        }

        if (Input.GetKey(KeyCode.D)) //D키를 눌렀을 때
        {
            this.transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
        }
    }

    void RotateEye() //눈 움직임
    {
        float MouseX = Input.GetAxis("Mouse X"); //마우스 X축
        float MouseY = Input.GetAxis("Mouse Y"); //마우스 Y축

        this.transform.Rotate(new Vector3(0, MouseX * Time.deltaTime * RotateSpeed, 0)); //X축 회전
        Eye.Rotate(new Vector3(-MouseY * Time.deltaTime * RotateSpeed, 0, 0)); //Y축 회전
    }
}
