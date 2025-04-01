using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;


public class PanelBase : MonoBehaviour, IActive
{
    [SerializeField] private UIElementAnim[] m_elementAnims=default;

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
        float maxDuration=0;
        if (m_elementAnims != null && m_elementAnims.Length > 0)
        {
            for (int i = 0; i < m_elementAnims.Length; i++)
            {
                maxDuration = Mathf.Max(maxDuration, m_elementAnims[i].Duration);
                m_elementAnims[i].Show();
            }
        }

        DOTween.To((t) => { }, 0, maxDuration, maxDuration).OnComplete(() => { callBack(); });
    }

    private void Hide(Action callBack = null)
    {
        float maxDuration=0;
        if (m_elementAnims != null && m_elementAnims.Length > 0)
        {
            for (int i = 0; i < m_elementAnims.Length; i++)
            {
                maxDuration = Mathf.Max(maxDuration, m_elementAnims[i].Duration);
                m_elementAnims[i].Hide();
            }
        }

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
