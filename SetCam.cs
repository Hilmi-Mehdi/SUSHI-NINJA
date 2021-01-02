using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCam : MonoBehaviour
{
    public GameObject Cam;
    public GameObject Cam1;

    private IEnumerator Set()
    {
        Cam1.SetActive(false);
        yield return new WaitForSecondsRealtime(5f);
        Cam.SetActive(false);
        Cam1.SetActive(true);
    }
    private void Update()
    {
        Set();
    }
}
