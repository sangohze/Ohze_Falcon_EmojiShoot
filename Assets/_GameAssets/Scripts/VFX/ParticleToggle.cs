using UnityEngine;

public class ParticleToggle : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    private float timer = 0f;
    [SerializeField] private float duration = 1f; 
    private bool isPlaying = true;

    void Start()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        ps.Play();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isPlaying && timer >= duration)
        {
            ps.Stop();
            isPlaying = false;
            timer = 0f;
        }
        else if (!isPlaying && timer >= duration)
        {
            ps.Play();
            isPlaying = true;
            timer = 0f;
        }
    }
}
