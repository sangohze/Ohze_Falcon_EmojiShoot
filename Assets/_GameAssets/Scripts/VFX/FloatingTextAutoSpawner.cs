using System.Collections;
using UnityEngine;

public class FloatingTextAutoSpawner : Singleton<FloatingTextAutoSpawner>
{
    public GameObject floatingTextPrefab; // Prefab của FloatingText
    public float spawnInterval = 1f;       // Thời gian giữa mỗi lần spawn
    public Vector3 spawnOffset = new Vector3(0, 1f, 0); // Độ lệch mỗi lần spawn

    private Coroutine spawnRoutine;

    // Gọi hàm này để bắt đầu spawn liên tục
    public void StartSpawning(Vector3? customPosition = null)
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop(customPosition));
        }
    }

    // Gọi hàm này để ngừng spawn
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop(Vector3? customPosition = null)
    {
        while (true)
        {
            SpawnFloatingText(customPosition);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFloatingText(Vector3? customPosition = null)
    {
        if (floatingTextPrefab == null) return;

        Vector3 spawnPos;

        if (customPosition.HasValue)
        {
            spawnPos = customPosition.Value;
        }
        else
        {
            spawnPos = transform.position + spawnOffset * Random.Range(0.8f, 1.2f);
        }
       
        GameObject newText = Lean.Pool.LeanPool.Spawn(floatingTextPrefab, spawnPos, Quaternion.identity);
    }

}
