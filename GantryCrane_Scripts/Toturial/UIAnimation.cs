using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{

    Animator animator; // 현재 애니메이션

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("None"))
            gameObject.SetActive(false);
    }
    public void StartAnimation(string str)
    {
        animator.Play(str);
    }

}
