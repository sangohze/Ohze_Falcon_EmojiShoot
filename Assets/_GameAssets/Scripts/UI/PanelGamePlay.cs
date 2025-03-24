using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGamePlay : PanelBase
{
    [SerializeField] Image PreviewAvatar;
    [SerializeField] Image PreviewEmoji;

    private void Start()
    {
        PreviewAvatar.sprite = LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewCharaterTarget;
        PreviewEmoji.sprite = LevelManager.I.levels[LevelManager.I.currentLevelIndex].PreviewEmojiTarget;
    }
    public void ButtonNextLevelOnClick()
    {
        GamePlayManager.I.GoToGamePlayFW();
        LevelManager.I.NextLevel();
    }
   
}
