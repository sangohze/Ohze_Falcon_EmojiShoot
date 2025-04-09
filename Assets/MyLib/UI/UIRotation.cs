using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIRotation : UIElementAnim
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 _rotationStep = new Vector3(0, 0, 45); // mỗi bước xoay
    [SerializeField] private float _delayBetween = 0.2f; // delay giữa các bước
    [SerializeField] private float _pauseAfterFullRotation = 3f; // nghỉ 3s sau mỗi vòng
    [SerializeField] private Ease _ease = Ease.Linear;

    private RectTransform m_rectTransform;
    private Coroutine rotationCoroutine;

    protected void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    public override void Show()
    {
        if (!gameObject.activeInHierarchy) return;

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(LoopRotateByStep());
    }

    public override void Hide()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }

        Tweener?.Kill();
    }

    private IEnumerator LoopRotateByStep()
    {
        float accumulatedRotation = 0f;

        while (true)
        {
            Vector3 newRotation = m_rectTransform.localEulerAngles + _rotationStep;
            Tweener = m_rectTransform.DOLocalRotate(newRotation, Duration).SetEase(_ease);
            yield return Tweener.WaitForCompletion();

            accumulatedRotation += Mathf.Abs(_rotationStep.z); // chỉ tính độ quay trục Z (hoặc tính max XYZ nếu cần)

            if (accumulatedRotation >= 360f)
            {
                accumulatedRotation = 0f;
                yield return new WaitForSeconds(_pauseAfterFullRotation); // nghỉ 3s sau mỗi vòng
            }
            else
            {
                yield return new WaitForSeconds(_delayBetween);
            }
        }
    }

    private void OnDisable()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }

        Tweener?.Kill();
    }
}
