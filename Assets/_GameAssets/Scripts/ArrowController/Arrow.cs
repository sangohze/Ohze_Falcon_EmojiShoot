using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] MeshRenderer headArrow;


    private void OnEnable()
    {
        if (EmojiController.I != null)
        {
            EmojiController.I.OnEmojiChanged += UpdateArrowMaterial;
            UpdateArrowMaterial(EmojiController.I.currentEmoji);
        }
        headArrow.GetComponent<SphereCollider>().enabled = true;
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
        // Đợi 1 frame để tránh lỗi trail bị reset
        StartCoroutine(EnableTrail());
        LeanPool.Despawn(gameObject, 2f);  
    }

    private IEnumerator EnableTrail()
    {
        yield return null; // Chờ 1 frame
        trailRenderer.Clear();
        trailRenderer.enabled = true;
    }

    public void UpdateHeadMaterial(Material newMaterial)
    {
        if (headArrow != null)
        {
            headArrow.material = newMaterial;
        }
    }
    void FixedUpdate()
    {
        if (!rb.isKinematic && rb.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            headArrow.GetComponent<SphereCollider>().enabled = false;
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

    void OnDestroy()
    {
        // Unsubscribe from the emoji change event
        if (EmojiController.I != null)
        {
            EmojiController.I.OnEmojiChanged -= UpdateArrowMaterial;
        }
    }
}
