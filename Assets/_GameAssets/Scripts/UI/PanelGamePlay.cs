using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGamePlay : PanelBase
{
    public Image PreviewAvatar;
    public Image PreviewEmoji;
    public Image PreviewAvatar2;


   
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
