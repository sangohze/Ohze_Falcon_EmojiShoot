using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Lean.Pool;

public class RifleGun : MonoBehaviour
{
    public Transform shootBulletPostation;
    public Transform pistolPostation;
    public Transform shootFXPosition;
    public ParticleSystem shootingFX;
    public GameObject emojiBulletPrefab;
    public Vector3 shootRotation;
    public float recoilSpeed = 0.05f;
    public float recoilAngle = 3f;
    public float magazineRotationSpeed = 30f;
    public float emojiSpawnRate = 0.1f;

    private Camera mainCamera;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Transform magazineGunTransform;
    private bool _pressed;
    private float emojiSpawnTimer;
    [SerializeField] private float fixedBulletSpeed = 20f;
    [SerializeField] private Vector3 bulletDirection = Vector3.forward; // Hướng mặc định sẽ là forward.


    void Start()
    {
        initialScale = transform.localScale;
        initialRotation = pistolPostation.localRotation;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!GameManager.Instance.clickArrow || IsPointerOverUIElement())
            return;

        HandleTouchInput();

        if (_pressed)
        {
            ShotAnim();
            ShotFx();
            SpawnEmojiBullet();
        }
    }

    private bool IsPointerOverUIElement()
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    private bool IsPointerOverUIElement(Vector2 touchPosition)
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = touchPosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsPointerOverUIElement(touch.position)) return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _pressed = true;
                    emojiSpawnTimer = 0;
                    break;

                case TouchPhase.Ended:
                    _pressed = false;
                    pistolPostation.localRotation = initialRotation;
                    break;
            }
        }
    }

    private void ShotAnim()
    {
        // Súng rung qua lại bằng xoay Z
        float recoilAmount = Mathf.Sin(Time.time * (1f / recoilSpeed));
        Vector3 angle = shootRotation * recoilAmount;

        pistolPostation.localRotation = initialRotation * Quaternion.Euler(angle);

        // Quay băng đạn
        magazineGunTransform.Rotate(Vector3.forward * -magazineRotationSpeed * Time.deltaTime);
    }

    private void ShotFx()
    {
        if (shootingFX != null)
        {
            shootingFX.Play();
        }
    }


    private void SpawnEmojiBullet()
    {
        emojiSpawnTimer += Time.deltaTime;
        if (emojiSpawnTimer >= emojiSpawnRate)
        {
            Quaternion spawnRotation = shootBulletPostation.rotation;
            GameObject bullet = LeanPool.Spawn(emojiBulletPrefab, shootBulletPostation.position, spawnRotation);
           
            EmojiBullet bulletScript = bullet.GetComponent<EmojiBullet>();
            if (bulletScript != null)
            {
                bulletScript.LaunchInArc(fixedBulletSpeed); 
            }

            // Tránh va chạm với các bullet khác
            Collider bulletCol = bullet.GetComponent<Collider>();
            if (bulletCol != null)
            {
                Physics.IgnoreLayerCollision(bullet.layer, bullet.layer);
            }

            emojiSpawnTimer = 0;
        }
    }
}
