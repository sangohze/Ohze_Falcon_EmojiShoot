using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiController : Singleton<EmojiController>
{

    public List<Material> materialsEmoji = new List<Material>();
    public EmojiType currentEmoji = EmojiType.Dance;
    public delegate void EmojiChangedHandler(EmojiType newEmoji);
    public event EmojiChangedHandler OnEmojiChanged;
    public List<Sprite> spritesEmojiSingle = new List<Sprite>();
    public List<Sprite> spritesEmojiCombo = new List<Sprite>();
    [SerializeField] private List<GameObject> emojiObjectsEff = new List<GameObject>();
    [SerializeField] private List<Image> emojiButtons = new List<Image>();
    private Vector3 localscaleShow = new Vector3(0.65f, 0.65f, 0.65f);
    private Vector3 localscaleHide = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] List<Sprite> spriteHide = new List<Sprite>();
    [SerializeField] List<Sprite> spriteShow = new List<Sprite>();

    public List<EmojiType> selectedEmojiTypesPerCharacter = new List<EmojiType>();

    private void Start()
    {
        RandomEmojiType();
        ChangeEmoji(currentEmoji);
    }
    public void ChangeEmoji(EmojiType newEmoji)
    {
        currentEmoji = newEmoji;
        int emojiIndex = (int)newEmoji;
        if (emojiIndex >= 0 && emojiIndex < materialsEmoji.Count) if (emojiIndex >= 0 && emojiIndex < materialsEmoji.Count)
            {

                OnEmojiChanged?.Invoke(newEmoji);
            }
        for (int i = 0; i < emojiObjectsEff.Count; i++)
        {
            emojiObjectsEff[i].SetActive(i == emojiIndex);
        }
        for (int i = 0; i < emojiButtons.Count; i++)
        {
            Canvas canvas = emojiButtons[i].GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = emojiButtons[i].gameObject.AddComponent<Canvas>();
            }

            emojiButtons[i].transform.localScale = (i == emojiIndex) ? localscaleShow : localscaleHide;
            emojiButtons[i].sprite = (i == emojiIndex) ? spriteShow[i] : spriteHide[i];
            canvas.overrideSorting = (i == emojiIndex ) ? true : false;               
            canvas.sortingOrder = (i == emojiIndex) ? 2 : 1;              
        }
    }
    public void RandomEmojiType()
    {
        if (selectedEmojiTypesPerCharacter != null && selectedEmojiTypesPerCharacter.Count > 0)
        {
            int index = Random.Range(0, selectedEmojiTypesPerCharacter.Count);
            currentEmoji = selectedEmojiTypesPerCharacter[index];
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


