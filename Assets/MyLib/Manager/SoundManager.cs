using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using MoreMountains.Tools;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioCueEventChannelSO _playSFXOn = default;
    [SerializeField] private AudioConfigurationSO _audioConfig = default;
    [SerializeField] private AudioConfigurationSO _audioConfig_3D = default;
    [SerializeField] private SoundDataDictionary _mapper;
    


    public AudioCueKey PlaySFX(AudioCueSO audioCueSO)
    {
        return _playSFXOn.RaisePlayEvent(audioCueSO, _audioConfig);
    }

    public AudioCueKey PlaySFX(TypeSound typeSound, AudioConfigurationSO audioConfig)
    {
        return _playSFXOn.RaisePlayEvent(_mapper[typeSound], audioConfig);
    }

    public AudioCueKey PlaySFX(TypeSound typeSound)
    {
        return _playSFXOn.RaisePlayEvent(_mapper[typeSound], _audioConfig);
    }

    public AudioCueKey PlaySFX2(TypeSound typeSound)
    {
        return _playSFXOn.RaisePlayEvent(_mapper[typeSound], _audioConfig);
    }

    public AudioCueKey PlaySFX(TypeSound typeSound, Vector3 position)
    {
        return _playSFXOn.RaisePlayEvent(_mapper[typeSound], _audioConfig_3D, position);
    }

    public void StopSFX(AudioCueKey key)
    {
        _playSFXOn.RaiseStopEvent(key);
    }

    public void StopAllSFX()
    {
        _playSFXOn.RaiseStopAllEvent();
    }    
}

public enum TypeSound
{
    SFX_Click,
    SFX_ClickEmojiButton,
    SFX_Love_Man,
    SFX_Love_Girl,
    SFX_Sad_Man,
    SFX_Sad_Girl,
    SFX_Angry_Man,
    SFX_Angry_Girl,
    SFX_Pray,
    SFX_Devil,
    SFX_Dance,
    SFX_Vomit,
    SFX_Lovers,
    SFX_Fight,
    SFX_God,
    SFX_Summon,
    SFX_Stinky,
    SFX_Bow,
    SFX_Arrow,
    SFX_Win,
    SFX_Lose,
    SFX_ClockWarning,
    SFX_GunSound,
    SFX_Talkative_male,
    SFX_Talkative_female,
    SFX_Emoji_Shit,
    SFX_Scared,
    SFX_Combo_Emoji_Shit,
}

[System.Serializable] public class SoundDataDictionary : UnitySerializedDictionary<TypeSound, AudioCueSO> { }
