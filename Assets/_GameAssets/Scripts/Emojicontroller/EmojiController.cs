using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiController : Singleton<EmojiController>
{
    
    public List<Material> materialsEmoji = new List<Material>();

    public List<ParticleSystem> emojiEffects = new List<ParticleSystem>();

    public List<Vector3> effectPositions = new List<Vector3>();
    public EmojiType currentEmoji = EmojiType.Love; // Mặc định là Love
    public delegate void EmojiChangedHandler(EmojiType newEmoji);
    public event EmojiChangedHandler OnEmojiChanged;

    

    void Start()
    {
        
        ChangeEmoji(currentEmoji);
    }

    public void ChangeEmoji(EmojiType newEmoji)
    {
        Debug.Log("Emoji hiện tại: " + currentEmoji);
        currentEmoji = newEmoji;
        int emojiIndex = (int)newEmoji;
        if (emojiIndex >= 0 && emojiIndex < materialsEmoji.Count) if (emojiIndex >= 0 && emojiIndex < materialsEmoji.Count)
        {
          
            Debug.Log($"Đã đổi sang emoji: {newEmoji}");
            OnEmojiChanged?.Invoke(newEmoji);
        }
       
    }

   
}

public enum EmojiType
{
    Love,
    Sad,
    Angry,
    Pray,
    Devil,
    Dance,
    Vomit
}


