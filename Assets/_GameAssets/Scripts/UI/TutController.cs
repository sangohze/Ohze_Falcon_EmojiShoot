using UnityEngine;
using UnityEngine.EventSystems; // ← Quan trọng

public class TutController : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Nếu chạm vào UI thì bỏ qua
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            if (touch.phase == TouchPhase.Began)
            {
                gameObject.SetActive(false);
                UIManager.I.Get<PanelGamePlay>().ShowPanelGamePlay(true);
            }
        }
    }
}
