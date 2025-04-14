using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGameLose : PanelBase
{
    private bool _isClick;

    void OnEnable()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Lose);
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
        GamePlayManager.I.GoToGamePlayScreen();
        LevelManager.I.NextLevel();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }

    public void ButtonRevive()
    {
        CountdownTimer.InvokeRevive();
        UIManager.I.Show<PanelGamePlay>();
        gameObject.SetActive(false); 
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }
    private void OnDisable()
    {
     
    }
}
