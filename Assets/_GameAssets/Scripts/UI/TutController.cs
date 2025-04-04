using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutController : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            gameObject.SetActive(false);
            UIManager.I.Get<PanelGamePlay>().ShowPanelGamePlay(true);
        }
    }
}
