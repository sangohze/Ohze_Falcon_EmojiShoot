using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGameWin : PanelBase
{
   
    void OnEnable()
    {
        
        //SoundManager.I.PlaySFX(TypeSound.SFX_PhaoGiay);
    }

    public void ButtonRePlayOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
      
    }

    public void ButtonNextLevelOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        LevelManager.I.NextLevel();
    }
    private void OnDisable()
    {
     
    }
}
