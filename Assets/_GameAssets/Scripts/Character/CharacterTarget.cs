using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CharacterTarget
{
    public EmojiType EmojiTypeTarget; 
    public List<CharacterController> EnemyTarget;
    [PreviewField]
    public Sprite PreviewCharaterTarget;
    [PreviewField]
    public Sprite PreviewEmojiTarget;
    [PreviewField]
    public Sprite PreviewCharaterTarget2;
}