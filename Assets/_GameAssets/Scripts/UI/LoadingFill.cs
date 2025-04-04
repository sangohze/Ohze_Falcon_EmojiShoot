using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingFill : MonoBehaviour
{
    public Image LoadingBarFill;

    public static bool IsOn = false;



    private void Awake()
    {
        LoadingBarFill.fillAmount = 0f;
        IsOn = false;
        Load();
    }
    public void Load()
    {
        float timeload = 3f;
        //Debug.Log("CountOpenGame " + AdsManager.Instance.CountOpenGame);
        //if (AdsManager.Instance.CountOpenGame >= 2 && AdsManager.Instance.IsCanShowAOA())
        //{
        //    timeload = 6f;
        //}
        LoadingBarFill.fillAmount = 0f;
        LoadingBarFill.DOFillAmount(1, timeload).SetEase(Ease.Linear).OnComplete(() =>
        {
            IsOn = true;
        });
    }
}
