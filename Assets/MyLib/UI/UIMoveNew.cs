using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIMoveNew : UIElementAnim
{
    [SerializeField] private Vector3 _hidePos = default;
    [SerializeField] private Vector3 _showPos = default;
    [SerializeField] private Vector3 _middlePos = default; // Vị trí trung gian
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private float _middleDurationPercent = 0.4f;

    private RectTransform m_curTrf;
    private Tween tweener;

    public Vector3 HidePos => _hidePos;
    public Vector3 ShowPos => _showPos;
    public Vector3 MiddlePos => _middlePos;

    protected void Awake()
    {
        m_curTrf = GetComponent<RectTransform>();
    }

    [SerializeField] bool _queuedShow = false;

    public override void Hide()
    {
        if (!gameObject.activeInHierarchy)
        {
            if (_queuedShow) Show();
            return;
        }

        float durationToMiddle = Duration * _middleDurationPercent;

        tweener = m_curTrf.DOAnchorPos3D(_middlePos, durationToMiddle)
                          .SetEase(_ease)
                          .OnComplete(() =>
                          {
                              m_curTrf.anchoredPosition3D = _hidePos;

                              if (_queuedShow)
                              {
                                  _queuedShow = false;
                                  Show();
                              }
                          });
    }

    public override void Show()
    {
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            _queuedShow = true;
            return;
        }

        tweener = m_curTrf.DOAnchorPos3D(_showPos, Duration)
                          .SetEase(_ease)
                          .ChangeStartValue(_hidePos);
    }


    private void OnDisable()
    {
        tweener?.Kill();
    }
}
