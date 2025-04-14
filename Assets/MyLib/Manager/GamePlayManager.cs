using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

public class GamePlayManager : Singleton<GamePlayManager>
{
    //public bool isCallADS = false;
    public bool m_isTutModeDIY = false;

    public bool m_isTutModeFWShow = false;
    public bool m_isTutModeBomb = false;
    
    [SerializeField] private GameSceneSO _gamePlay;
   

    [SerializeField] private LoadEventChannelSO _loadGamePlayEvent;

    [Header("Listening")]
    [SerializeField] private VoidEventChannelSO _onGameReady;

    [Header("Broadcasting")]
    [SerializeField] private VoidEventChannelSO _onGameInit;
    [SerializeField] private VoidEventChannelSO _onGameActive;
    [SerializeField] private VoidEventChannelSO _onGameReload;
    [SerializeField] private VoidEventChannelSO _onGameWin;
    [SerializeField] private VoidEventChannelSO _onGameLose;
    [SerializeField] private VoidEventChannelSO _onGameRevive;
    [SerializeField] private VoidEventChannelSO _onChangeCoin;
    [SerializeField] private VoidEventChannelSO _onGoToTestFW;

    public bool SecondPlay ;
    public TypeStateGame StateGame { get; set; }
    public bool IscanRevive { get; private set; }

    public long COIN { get { return DataManager.I.SaveData.Coin; } set { DataManager.I.SaveData.Coin = value; _onChangeCoin.RaiseEvent(); } }
    public int LEVEL { get { return DataManager.I.SaveData.Level; } set { DataManager.I.SaveData.Level = value; } }

 

    



    protected override void Awake()
    {
        base.Awake();
        SecondPlay = false;
        //if (ES3.KeyExists("isCallADS"))
        //{
        //    isCallADS = ES3.Load<bool>("isCallADS");
        //}      
      
        //m_isTutModeDIY = ES3.Load<bool>("m_isTutModeDIY",true);
        //m_isTutModeFWShow = ES3.Load<bool>("m_isTutModeFWShow", true);
        //m_isTutModeBomb = ES3.Load<bool>("m_isTutModeBomb", true);

    }

    public bool IsCreateNew { get; set; }

    private void OnEnable()
    {
        _onGameReady.OnEventRaised += InitGame;
    }

    private void OnDisable()
    {
        _onGameReady.OnEventRaised -= InitGame;
    }

    public void InitGame()
    {
        StateGame = TypeStateGame.Lobby;
        IscanRevive = true;
        _onGameInit.RaiseEvent();
    }

    public void StartGame()
    {
        StateGame = TypeStateGame.Playing;
        _onGameActive.RaiseEvent();
    }

    public void GameWin()
    {
        if (StateGame != TypeStateGame.Playing)
            return;
        StateGame = TypeStateGame.GameWin;
        LEVEL++;
        _onGameWin.RaiseEvent();
    }

    public void GameOver()
    {
        if (StateGame != TypeStateGame.Playing)
            return;
        StateGame = TypeStateGame.GameOver;
        _onGameLose.RaiseEvent();
    }

    public void Revive()
    {
        StateGame = TypeStateGame.Playing;
        IscanRevive = false;
        _onGameRevive.RaiseEvent();
    }

    public void ReloadGame()
    {
        _onGameReload.RaiseEvent();
    }

    [Button]
    public void GoToGamePlayDIY()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        SecondPlay = true;
       
        _loadGamePlayEvent.RaiseEvent(_gamePlay, false);
    }
    public void GoToGamePlayDIYHome()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        SecondPlay = false;
        _loadGamePlayEvent.RaiseEvent(_gamePlay, false);
    }    
    [Button]
    public void GoToGamePlayScreen()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
         _loadGamePlayEvent.RaiseEvent(_gamePlay, false);

    }
    
    //Main
    [Button]
    public void GoToGamePlayMain()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        _onGoToTestFW.RaiseEvent();


    }
}
