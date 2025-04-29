using UnityEngine;
using Lean.Pool;

public class Floating : MonoBehaviour
{
    public Vector3 PositionMult = new Vector3(0, -1.5f, 0); // Gia tốc đi xuống
    public Vector3 PositionDirection = new Vector3(0.5f, 3f, 0); // Vận tốc ban đầu
    public float MaxHeight = 6f; // Chiều cao tối đa trước khi despawn
    private Vector3 startPos;
    private Vector3 positionTemp;

    void OnEnable()
    {
        startPos = transform.position;
        positionTemp = transform.position;
    }

    void Update()
    {
        positionTemp += PositionDirection * Time.deltaTime;
        PositionDirection += PositionMult * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, positionTemp, 3f);

        // Nếu đạt chiều cao so với điểm spawn ban đầu → despawn
        //if (transform.position.y >= startPos.y + MaxHeight)
        //{
        //    LeanPool.Despawn(gameObject);
        //}
    }
}
