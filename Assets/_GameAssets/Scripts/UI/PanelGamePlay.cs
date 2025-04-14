using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelGamePlay : PanelBase
{
    public Image PreviewAvatar;
    public Image PreviewEmoji;
    public Image PreviewAvatar2;
    [SerializeField] private GameObject _groupEmoji;
    [SerializeField] private GameObject _groupMission;
    [SerializeField] private GameObject _objTimer;
    [SerializeField] private GameObject _textLevelContainer;
    [SerializeField] private TextMeshProUGUI _textLevel;
    [SerializeField] private GameObject _tutHand;
    [SerializeField] private GameObject _progressBar;

    public bool _isHome;
    private bool _isClick;

    private void Start()
    {
        _textLevel.text = "Level " + GamePlayController.I.currentLevelIndexText.ToString();
        ShowPanelGameHome(false);
        UIManager.I.Show<PanelGamePlay>();
    }
    public void ShowPanelGameHome(bool isHome)
    {
        _isHome = isHome;
        _groupEmoji.SetActive(false);
        _groupMission.SetActive(false);
        _objTimer.SetActive(false);
        _textLevelContainer.SetActive(true);
        _tutHand.SetActive(true);
        _progressBar.SetActive(true);
    }

    public void ShowPanelGamePlay(bool isHome)
    {
        _isHome = isHome;
        _groupEmoji.SetActive(true);
        _groupMission.SetActive(true);
        _objTimer.SetActive(true);
        _textLevelContainer.SetActive(false);
        _tutHand.SetActive(false);
        _progressBar.SetActive(false);
    }
    public void ButtonNextLevelOnClick()
    {
        GamePlayManager.I.GoToGamePlayScreen();
        LevelManager.I.NextLevel();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }

    public void PlayAgianOnClick()
    {
        if (_isClick) return;
        _isClick = true;
        GamePlayManager.I.GoToGamePlayScreen();
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
    }
    public void ShowPanelSetting()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_Click);
        UIManager.I.Show<PanelSetting>();
    }

}
