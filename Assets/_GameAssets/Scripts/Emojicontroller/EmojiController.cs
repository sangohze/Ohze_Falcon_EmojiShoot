using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiController : MonoBehaviour
{
    public static  EmojiController Instance;
    [SerializeField] List<Texture> texturesEmoji = new List<Texture>();

    public EmojiType currentEmoji = EmojiType.Happy; // Mặc định là Happy
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("Emoji hiện tại: " + currentEmoji);
    }

    public void ChangeEmoji(EmojiType newEmoji)
    {
        currentEmoji = newEmoji;
        Debug.Log("Đã đổi sang emoji: " + currentEmoji);
    }
}

public enum EmojiType
{
    Happy,
    Sad,
    Angry,
    Laughing,
    Surprised
}
