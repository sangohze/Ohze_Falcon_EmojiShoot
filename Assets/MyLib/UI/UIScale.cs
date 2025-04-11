using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIScale : UIElementAnim
{
    [SerializeField] private Vector3 _hidePos = default;
    [SerializeField] private Vector3 _showPos = default;
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private bool loop = false; // Thêm boolean loop để kiểm soát lặp lại

    private RectTransform m_curTrf;

    public Vector3 _HidePos { get { return _hidePos; } }
    public Vector3 _ShowPos { get { return _showPos; } }

    protected void Awake()
    {
        m_curTrf = GetComponent<RectTransform>();
    }

    public override void Hide()
    {
        if (!gameObject.activeSelf || !gameObject.gameObject.activeInHierarchy) return;

        Tweener = m_curTrf.DOScale(_HidePos, Duration)
            .SetEase(_ease)
            .OnComplete(() =>
            {
                if (loop) Show(); // Nếu loop bật, gọi lại Show sau khi Hide hoàn tất
            });
    }

    public override void Show()
    {
        if (!gameObject.activeSelf || !gameObject.gameObject.activeInHierarchy) return;

        Tweener = m_curTrf.DOScale(_ShowPos, Duration)
            .SetEase(_ease)
            .OnComplete(() =>
            {
                if (loop) Hide(); // Nếu loop bật, gọi lại Hide sau khi Show hoàn tất
            });
    }

    private void OnDisable()
    {
        Tweener.Kill();
    }

    public void SetLoop(bool value)
    {
        loop = value; // Cho phép bật/tắt loop từ bên ngoài
    }
}
