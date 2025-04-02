using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PanelBase : MonoBehaviour, IUIActive
{
    [SerializeField] private UIElementAnim[] m_elementAnims = default;
    [SerializeField] private UnityEvent _onStartShow;
    [SerializeField] private UnityEvent _onStartHide;
    [SerializeField] private float _TimeAnim;


    [Button]
    public virtual void ActiveMe(System.Action callBack)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        Show(() =>
        {
            callBack?.Invoke();
        });
    }
    [Button]
    public virtual void DeActiveMe(System.Action callBack)
    {
        Hide(() =>
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            callBack?.Invoke();
        });
    }
    private void Show(Action callBack = null)
    {
        float maxDuration = 0;
        if (m_elementAnims != null && m_elementAnims.Length > 0)
        {
            for (int i = 0; i < m_elementAnims.Length; i++)
            {
                maxDuration = Mathf.Max(maxDuration, m_elementAnims[i].Duration);
                m_elementAnims[i].Show();
            }
        }
        _onStartShow?.Invoke();
        DOTween.To((t) => { }, 0, maxDuration, maxDuration).OnComplete(() => { callBack(); });

    }
    private void Hide(Action callBack = null)
    {
        float maxDuration = _TimeAnim;
        if (m_elementAnims != null && m_elementAnims.Length > 0)
        {
            for (int i = 0; i < m_elementAnims.Length; i++)
            {
                maxDuration = Mathf.Max(maxDuration, m_elementAnims[i].Duration);
                m_elementAnims[i].Hide();
            }
        }

        _onStartHide?.Invoke();
        DOTween.To((t) => { }, 0, maxDuration, maxDuration).OnComplete(() => { callBack(); });
    }
#if UNITY_EDITOR
    [Button]
    public void CacheUIElementAnim()
    {
        m_elementAnims = GetComponentsInChildren<UIElementAnim>();
    }
#endif
}