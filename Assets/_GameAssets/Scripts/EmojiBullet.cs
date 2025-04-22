using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class EmojiBullet : MonoBehaviour
{
    public float force = 300f;
    public float destroyTime = 1.5f;

    [SerializeField] Rigidbody rb;
    [SerializeField] MeshRenderer emojiBullet;

    private void OnEnable()
    {
        if (EmojiController.I != null)
        {
            //EmojiController.I.OnEmojiChanged += UpdateArrowMaterial;
            UpdateArrowMaterial(EmojiController.I.currentEmoji);
        }
      
    }

    public void LaunchInArc(float velocity)
    {
        transform.parent = null;
        rb.isKinematic = false;
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(transform.forward * velocity, ForceMode.VelocityChange);
        LeanPool.Despawn(gameObject, 2f);
     
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            LeanPool.Despawn(gameObject, 0f);
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

    public void UpdateHeadMaterial(Material newMaterial)
    {
        if (emojiBullet != null)
        {
            emojiBullet.material = newMaterial;
        }
    }

}
