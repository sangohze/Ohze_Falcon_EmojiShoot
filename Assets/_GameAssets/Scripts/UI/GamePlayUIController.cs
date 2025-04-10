using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUIController : MonoBehaviour
{
    private void OnEnable()
    {
        EmojiController.I.ChangeEmoji(EmojiController.I.currentEmoji);
    }
}
