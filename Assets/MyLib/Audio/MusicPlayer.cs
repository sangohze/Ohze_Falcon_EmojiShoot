using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] private VoidEventChannelSO _onSceneReady = default;
    [SerializeField] private AudioCueEventChannelSO _playMusicOn = default;
    [SerializeField] private AudioCueSO _musicTrack = default;

    [SerializeField] private AudioConfigurationSO _audioConfig = default;



    private AudioCueKey _keyBG;


    private void OnEnable()
    {
        //_onSceneReady.OnEventRaised += PlayMusic;
    }

    private void OnDisable()
    {
        //_onSceneReady.OnEventRaised -= PlayMusic;

        //_playMusicOn.RaiseStopEvent(_keyBG);
    }

    public void PlayMusic()
    {
        _keyBG = _playMusicOn.RaisePlayEventMusic(_musicTrack, _audioConfig);
    }

    public void PauseMusic()
    {
        _playMusicOn.RaisePauseEvent(_keyBG);
    }

    public void ResumeMusic()
    {
        _playMusicOn.RaisePauseEvent(_keyBG);
    }
}
