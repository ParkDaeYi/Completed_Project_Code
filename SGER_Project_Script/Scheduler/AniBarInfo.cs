using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniBarInfo : MonoBehaviour {

    public GameObject _detailContent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnMouseEnter()
    {
        _detailContent.SetActive(true);
    }
    public void OnMouseExit()
    {
        _detailContent.SetActive(false);
    }
}
