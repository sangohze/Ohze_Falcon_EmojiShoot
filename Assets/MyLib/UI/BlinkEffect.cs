using UnityEngine;
using DG.Tweening;
using TMPro;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float blinkInterval = 0.2f;
    [SerializeField] Color blinkColor = Color.red;

    private Tween blinkTween;
    private Color originalColor = Color.white;
    private TMP_Text tmpText;

    void OnEnable()
    {
        StartBlink();
    }

    public void StartBlink()
    {
        if (blinkTween != null && blinkTween.IsActive()) return;

        tmpText = target.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            originalColor = tmpText.color;
            tmpText.color = blinkColor;


            blinkTween = DOTween
    .ToAlpha(() => tmpText.color, c => tmpText.color = c, 0f, blinkInterval / 2f)
    .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopBlink()
    {
        if (blinkTween != null) blinkTween.Kill();

        if (tmpText != null)
        {
            tmpText.color = originalColor;
        }
    }

    private void OnDisable()
    {
        StopBlink();
    }
}
