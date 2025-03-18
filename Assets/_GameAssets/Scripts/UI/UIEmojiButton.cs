using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEmojiButton : MonoBehaviour
{
    [SerializeField] private EmojiType emojiType;

    public void OnClickEmoji()
    {
        EmojiController.I.ChangeEmoji(emojiType);
    }
}
