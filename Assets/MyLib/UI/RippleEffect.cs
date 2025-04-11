using UnityEngine;
using DG.Tweening;

public class RippleEffect : MonoBehaviour
{
    public RectTransform targetTransform;
    public CanvasGroup canvasGroup;

    [Header("Effect Settings")]
    public float scaleUpSize = 1.5f;
    public float scaleUpDuration = 0.3f;
    public float fadeOutDuration = 0.5f;

    [Header("Loop Settings")]
    public float loopInterval = 1.5f;
    public bool loopForever = true;

    private Sequence rippleSequence;

    void OnEnable()
    {
        if (loopForever)
        {
            StartLoop();
        }
        else
        {
            PlayRipple();
        }
    }

    public void StartLoop()
    {
        rippleSequence = DOTween.Sequence();
        rippleSequence.SetLoops(-1); // lặp vô hạn

        rippleSequence.AppendCallback(() => PlayRipple());
        rippleSequence.AppendInterval(loopInterval);
    }

    public void StopLoop()
    {
        if (rippleSequence != null && rippleSequence.IsActive())
        {
            rippleSequence.Kill();
        }
    }

    public void PlayRipple()
    {
        // Reset về trạng thái ban đầu
        targetTransform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;

        // Scale lên + fade out
        Sequence effect = DOTween.Sequence();
        effect.Append(targetTransform.DOScale(scaleUpSize, scaleUpDuration).SetEase(Ease.OutQuad));
        effect.Join(canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.OutQuad));

        // Khi fade xong → reset lại
        effect.AppendCallback(() =>
        {
            targetTransform.localScale = Vector3.one;
            canvasGroup.alpha = 1f;
        });
    }
}
