using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RootMotion.Demos.CharacterThirdPerson;

public class PanelSetting : PanelBase
{
    
    private void OnEnable()
    {
    }
    public void Close_OnClick()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        DeActiveMe(null);
        //        
    }
    public void Home_OnClick()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        DeActiveMe(null);
        UIManager.I.Get<PanelGamePlay>().ShowPanelGameHome(true);
    }    

    
}
