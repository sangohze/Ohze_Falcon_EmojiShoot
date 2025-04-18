using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGameWin : PanelBase
{
    private bool _isClick = false;

    void OnEnable()
    {
        UIManager.I.Hide<PanelSetting>();
        UIManager.I.Hide<PanelSettingHome>();
        SoundManager.I.PlaySFX(TypeSound.SFX_Win);
    }

    public void ButtonRePlayOnClick()
    {
        if (_isClick) return;
        _isClick = true;
        GamePlayManager.I.GoToGamePlayScreen();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }

    public void ButtonNextLevelOnClick()
    {
        if (_isClick) return;
        _isClick = true;
        GamePlayManager.I.GoToGamePlayScreen();
        LevelManager.I.NextLevel();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }
    private void OnDisable()
    {
     
    }
}
