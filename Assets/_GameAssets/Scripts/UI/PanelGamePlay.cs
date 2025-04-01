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
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }

    public void PlayAgianOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }
    public void ShowPanelSetting()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        UIManager.I.Show<PanelSetting>();
    }

}
