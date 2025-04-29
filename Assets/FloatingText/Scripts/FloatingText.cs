using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
public class FloatingText : MonoBehaviour
{
    public float LifeTime = 2f;
    public float SpawnInterval = 1f;
    public Color TextColor = Color.white;
    public Vector3 NextOffset = new Vector3(0, 1.5f, 0);
    public GameObject floatingTextPrefab;

    private TextMesh textMesh;
    private float lifeTimer = 0f;
    private float spawnTimer = 0f;
    private float alpha = 1f;
    private bool hasSpawnedNext = false;
    private bool isStopped = false;
    public Transform spawnRoot;

    // Danh sách static để lưu các hiệu ứng đã spawn
    private static List<FloatingText> spawnedEffects = new List<FloatingText>();

    private void OnEnable()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMesh>();

        SetText("#@!" + Random.Range(10, 100));
        lifeTimer = 0f;
        spawnTimer = 0f;
        alpha = 1f;
        hasSpawnedNext = false;
        isStopped = false;
        textMesh.color = TextColor;

        // Lưu hiệu ứng này vào danh sách spawnedEffects
        spawnedEffects.Add(this);
    }

    private void Update()
    {
        if (isStopped) return;

        lifeTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        alpha = Mathf.Lerp(1f, 0f, lifeTimer / LifeTime);
        textMesh.color = new Color(TextColor.r, TextColor.g, TextColor.b, alpha);

        if (Camera.main)
        {
            transform.LookAt(Camera.main.transform);
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }

        if (!hasSpawnedNext && spawnTimer >= SpawnInterval)
        {
            hasSpawnedNext = true;

            Vector3 basePos = spawnRoot != null ? spawnRoot.position : transform.position;
            Vector3 nextPos = basePos + NextOffset;

            if (floatingTextPrefab != null)
            {
                GameObject next = Lean.Pool.LeanPool.Spawn(floatingTextPrefab, nextPos, Quaternion.identity);

                FloatingText ft = next.GetComponent<FloatingText>();
                if (ft != null)
                {
                    ft.spawnRoot = this.spawnRoot;
                    ft.floatingTextPrefab = this.floatingTextPrefab;
                }

                spawnedEffects.Add(ft);
            }

            spawnTimer = 0f;
        }

        // Kiểm tra và ẩn hiệu ứng nếu cần
        HideEffectOne();
    }

    public void SetText(string text)
    {
        if (textMesh)
            textMesh.text = text;
    }

    /// <summary>
    /// Ẩn tất cả các hiệu ứng đã spawn từ cái đầu tiên.
    /// </summary>
    public void HideEffectOne()
    {
        if (spawnedEffects.Count == 0)
            return;

        // Nếu cái đầu tiên đã bị tắt hoặc null (do đã despawn)
        if (spawnedEffects[0] == null || !spawnedEffects[0].gameObject.activeSelf)
        {
            foreach (var effect in spawnedEffects)
            {
                if (effect != null)
                {
                    FloatingText ft = effect.GetComponent<FloatingText>();
                    if (ft != null)
                        ft.isStopped = true;

                    if (effect.gameObject.activeSelf)
                        Lean.Pool.LeanPool.Despawn(effect);
                }
            }

            spawnedEffects.Clear();
        }
    }

}
