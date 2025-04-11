using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGameWin : PanelBase
{
   
    void OnEnable()
    {
        
        SoundManager.I.PlaySFX(TypeSound.SFX_Win);
    }

    public void ButtonRePlayOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }

    public void ButtonNextLevelOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        LevelManager.I.NextLevel();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }
    private void OnDisable()
    {
     
    }
}
