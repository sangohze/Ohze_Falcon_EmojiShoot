using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSettingHome : PanelBase
{
    // Start is called before the first frame update
    public void Close_OnClick()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        DeActiveMe(null);
        //        
    }
    public void Home_OnClick()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        UIManager.I.Get<PanelGamePlay>().ShowPanelGameHome(false);
        DeActiveMe(null);
    }
}
