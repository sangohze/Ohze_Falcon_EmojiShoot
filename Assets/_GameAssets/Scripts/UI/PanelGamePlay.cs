using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
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
    [SerializeField] private UIMove _textLevelContainer;
    [SerializeField] private TextMeshProUGUI _textLevel;
    [SerializeField] private GameObject _tutHand;
    [SerializeField] private GameObject _progressBar;
    public List<UIEmojiButton> allEmojiButtons;
    [SerializeField] private GameObject _btnRePlay;
    [SerializeField] private UIMove _btnSetting;
    public List<EmojiType> emojiShowRandom;
    public bool _isHome;
    private bool _isClick;

    public TextMeshProUGUI _textPistolLevel ;

    private void Start()
    {
       
        _textLevel.text = "Level " + GamePlayController.I.currentLevelIndexText.ToString();
       
        ShowPanelGameHome(false);
        UIManager.I.Show<PanelGamePlay>();
        SetupEmojiButtons(emojiShowRandom);
    }

    public void ShowPanelGameHome(bool isHome)
    {
        _isHome = isHome;
        _btnRePlay.SetActive(false);
        _groupEmoji.SetActive(false);
        _groupMission.SetActive(false);
        _objTimer.SetActive(false);
        _textLevelContainer.gameObject.SetActive(true);
        _tutHand.SetActive(true);
        _progressBar.SetActive(true);
        _textLevelContainer.Show();
        _btnSetting.Show();

    }

    public void ShowPanelGamePlay(bool isHome)
    {
        _btnSetting.Hide();
        _textLevelContainer.Hide();
        _tutHand.SetActive(false);
        _progressBar.SetActive(false);
        _btnSetting.Tweener?.OnComplete(() =>
        {
            _btnSetting.Show();
            _isHome = isHome;
            _btnRePlay.SetActive(true);
            _groupEmoji.SetActive(true);
            EmojiController.I.ChangeEmoji(EmojiController.I.currentEmoji);
            _groupMission.SetActive(true);
            _objTimer.SetActive(true);
            UIManager.I.Show<PanelGamePlay>();
        });
    }


    public void SetupEmojiButtons(List<EmojiType> emojiTypesToEnable)
    {
        foreach (var btn in allEmojiButtons)
        {
            btn.gameObject.SetActive(emojiTypesToEnable.Contains(btn.emojiType));
        }
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
        if (_isHome)
        {
            UIManager.I.Show<PanelSettingHome>();
        }
        else
        {
            UIManager.I.Show<PanelSetting>();
        }
    }

}
