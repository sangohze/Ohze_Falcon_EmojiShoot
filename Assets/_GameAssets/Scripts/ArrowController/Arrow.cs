using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rb;
    public TrailRenderer trailRenderer;
    [SerializeField] MeshRenderer headArrow;

    private void OnEnable()
    {
        if (EmojiController.I != null)
        {
            EmojiController.I.OnEmojiChanged += UpdateArrowMaterial;
        }
    }

    public void SetToRope(Transform ropeTransform, Transform bow)
    {
        transform.parent = ropeTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        rb.isKinematic = true;
        trailRenderer.enabled = false;
    }

    public void Shot(float velocity)
    {
        transform.parent = null;
        rb.isKinematic = false;
        rb.velocity = transform.forward * velocity;
        trailRenderer.Clear();
        trailRenderer.enabled = true;
        Destroy(gameObject, 5f);
    }

    public void UpdateHeadMaterial(Material newMaterial)
    {
        if (headArrow != null)
        {
            headArrow.material = newMaterial;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            Destroy(gameObject, 0.2f);
        }
    }

    public void UpdateArrowMaterial(EmojiType newEmoji)
    {
        int emojiIndex = (int)newEmoji;
        if (emojiIndex >= 0 && emojiIndex < EmojiController.I.materialsEmoji.Count)
        {
            Material newMaterial = EmojiController.I.materialsEmoji[emojiIndex];
            UpdateHeadMaterial(newMaterial);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the emoji change event
        if (EmojiController.I != null)
        {
            EmojiController.I.OnEmojiChanged -= UpdateArrowMaterial;
        }
    }
}
