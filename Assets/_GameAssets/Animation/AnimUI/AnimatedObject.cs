using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class AnimatedObject : MonoBehaviour
{
    [SerializeField] private UnityEvent _onStartShow;
    [SerializeField] private UnityEvent _onStartHide;
    [SerializeField] private float _timeAnim = 0.5f; // Thời gian animation

    private bool isHiding = false; // Tránh gọi Hide liên tục

    public void Show()
    {
        if (isHiding) return; // Không show nếu đang ẩn

        gameObject.SetActive(true);
        _onStartShow?.Invoke(); // Gọi sự kiện khi hiển thị
    }

    public void Hide(Action onComplete = null)
    {
        if (isHiding) return; // Tránh gọi Hide liên tục
        isHiding = true;

        _onStartHide?.Invoke(); // Gọi sự kiện trước khi ẩn
        StartCoroutine(HideWithDelay(() =>
        {
            isHiding = false;
            onComplete?.Invoke(); // Gọi callback sau khi Hide hoàn tất
        }));
    }

    private IEnumerator HideWithDelay(Action callBack)
    {
        yield return new WaitForSeconds(_timeAnim);
        gameObject.SetActive(false);
        callBack?.Invoke();
    }

    public void HideThenShow()
    {
        Hide(() => Show()); // Hide xong mới Show
    }
}
