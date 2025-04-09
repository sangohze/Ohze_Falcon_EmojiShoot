using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraRotation : MonoBehaviour
{
    private float verticalRotation = 0f; // Lưu trữ góc xoay dọc hiện tại
    public const float MIN_VERTICAL_ANGLE = -35f; // góc trên
    public const float MAX_VERTICAL_ANGLE = 15f;  // góc dưới

    public void RotateUpDown(float axis)
    {
        // Cập nhật góc quay dọc
        verticalRotation += axis * Time.deltaTime;

        // Giới hạn góc quay trong khoảng cho phép
        verticalRotation = Mathf.Clamp(verticalRotation, MIN_VERTICAL_ANGLE, MAX_VERTICAL_ANGLE);

        // Áp dụng góc quay mới
        transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    //rotate the camera right and left (y rotation)
    public void RotateRightLeft(float axis)
    {
        transform.RotateAround(transform.position, Vector3.up, -axis * Time.deltaTime);
    }
}

