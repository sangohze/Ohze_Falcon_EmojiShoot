using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEmojiButton : MonoBehaviour
{
    public EmojiType emojiType;
   
    
    public void OnClickEmoji()
    {
        SoundManager.I.PlaySFX(TypeSound.SFX_ClickEmojiButton);
        EmojiController.I.ChangeEmoji(emojiType);
    }
}
