using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * content : 상세적인 스케줄러바
 */


public class BigSchedulerBar : MonoBehaviour
{

    public int _objectNumber; //생성된 순서
    public int _originNumber; //생성된 캐릭의 형식
    public int _humanNumber; //사람 번호(로드에 활용)
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CloseClick()
    {
        this.gameObject.SetActive(false);

    }
}
