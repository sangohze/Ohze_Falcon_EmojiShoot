using UnityEngine;

public class TouchCameraRotation : MonoBehaviour
{
    public float sensitivity = 0.2f;
    public float minY = -80f, maxY = 80f;

    private Vector2 currentRotation;
    private Vector2 firstPoint;

    void Start()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        currentRotation = new Vector2(euler.y, euler.x); // chú ý: x là pitch (up/down), y là yaw (left/right)
    }

    void Update()
    {
        if (!GameManager.Instance.clickArrow)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                firstPoint = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 secondPoint = touch.position;

                float deltaX = FilterGyroValues(secondPoint.x - firstPoint.x);
                float deltaY = FilterGyroValues(secondPoint.y - firstPoint.y);

                currentRotation.x += deltaX * sensitivity;
                currentRotation.y += -deltaY * sensitivity;

                currentRotation.y = Mathf.Clamp(currentRotation.y, minY, maxY);

                firstPoint = secondPoint;

                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);
            }
        }
    }

    float FilterGyroValues(float axis)
    {
        float threshold = 0.3f;
        if (Mathf.Abs(axis) < threshold)
        {
            return axis * 0.2f;
        }
        return axis;
    }
}
