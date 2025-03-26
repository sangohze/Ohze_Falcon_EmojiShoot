using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGamePlay : PanelBase
{
    [SerializeField] Image PreviewAvatar;
    [SerializeField] Image PreviewEmoji;
    [SerializeField] Image PreviewAvatar2;


    private void Start()
    {
        PreviewAvatar.sprite = LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewCharaterTarget;
        PreviewEmoji.sprite = LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewEmojiTarget;
        if (LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewCharaterTarget2 != null)
        {
            PreviewAvatar2.gameObject.SetActive(true);
            PreviewAvatar2.sprite = LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewCharaterTarget2;
        }
        else
        {
            PreviewAvatar2.gameObject.SetActive(false);
        }
    }
    public void ButtonNextLevelOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        LevelManager.I.NextLevel();
    }

    public void PlayAgianOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        //LevelManager.I.NextLevel();
    }
    public void ShowPanelSetting()
    {
        UIManager.I.Show<PanelSetting>();
    }

}
