using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TruckController : MonoBehaviour
{
    [Header("Component")]
    public PlayableDirector _truckIn;
    public PlayableDirector _truckOut;
    public CraneControl _craneControl;
    public RaycastHit[] _containerCheck;
    public MainButtonController _mainButtonController;

    [Header("튜토리얼용")]
    public bool TutorialComplete;

    [Header("Audio")]
    public AudioSource _audioSource_truck;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_craneControl._other)
        {
            _containerCheck = Physics.RaycastAll(GetComponent<Transform>().position - new Vector3(0, 0, 3f), GetComponent<Transform>().up, 1.6f);
            if (_containerCheck.Length > 0)
            {
                for (int i = 0; i < _containerCheck.Length; i += 1)
                {
                    if (_containerCheck[i].transform.tag == "container") //컨테이너와 닿으면
                    {
                        _containerCheck[i].transform.SetParent(this.transform);
                        StartCoroutine(Test());

                        break;
                    }
                }
            }
            else
            {
                _craneControl._cargohookMove_Down = true;
            }
        }
    }
    public IEnumerator Test()
    {
        if(_audioSource_truck != null)
        {
            if (!_audioSource_truck.isPlaying)
                _audioSource_truck.Play();
        }


        _truckOut.Play();
        yield return new WaitForSeconds(5f);
        
        if (!TutorialComplete) TutorialComplete = true;

        _truckIn.Play();
        try
        {
            Destroy(this.transform.GetChild(0).gameObject);
        }
        catch (System.Exception e) { }
        yield return null;
    }
}
