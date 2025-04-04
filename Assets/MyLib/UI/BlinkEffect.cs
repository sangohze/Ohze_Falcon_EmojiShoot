using DG.Tweening;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float blinkInterval = 0.2f;

    private Tween blinkTween;

    void Start()
    {
        StartBlink();
    }

    public void StartBlink()
    {
        if (blinkTween != null && blinkTween.IsActive()) return;

        blinkTween = DOVirtual.DelayedCall(blinkInterval, () => {
            target.SetActive(!target.activeSelf);
        }).SetLoops(-1, LoopType.Restart);
    }

    public void StopBlink()
    {
        if (blinkTween != null) blinkTween.Kill();
        target.SetActive(true); 
    }
}
