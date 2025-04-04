using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    public float Tension;
    private bool _pressed;
    [SerializeField] Transform RopeTransform;
    [SerializeField] Vector3 RopeNearLocalPosition;
    [SerializeField] Vector3 RopeFarLocalPosition;
    [SerializeField] AnimationCurve RopeReturnAnimation;
    [SerializeField] float ReturnTime;
    [SerializeField] Arrow CurrentArrow = null;
    [SerializeField] float ArrowSpeed;
    

    [SerializeField] GameObject ar;

    [SerializeField] GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] private float timeshot = 1f;

    private Camera mainCamera;
    public float zoomOutFOV = 70f; 
    public float normalFOV = 60f; 
    public float zoomSpeed = 5f;
    [SerializeField] private float FixedArrowSpeed = 20f;
    [SerializeField] private float ShootingAngle = 30f;
    private bool isShotting = true;
    private AudioCueKey _keysoundbow;
    void Start()
    {
        RopeNearLocalPosition = RopeTransform.localPosition;
        mainCamera = Camera.main; // Lấy camera chính
        SpawnArrow();
    }

    void Update()
    {
        float screenPosition_x = Input.mousePosition.x;
        float screenPosition_y = Input.mousePosition.y;

        if (screenPosition_x > 90 * Screen.width / 100 && screenPosition_y < Screen.width / 10)
            return;

        if (!GameManager.Instance.clickArrow)
            return;

        // Check if the pointer is over a UI element
        if (IsPointerOverUIElement())
            return;

        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (IsPointerOverUIElement(touch.position))
                return;

            switch (touch.phase )
            {
                case TouchPhase.Began:
                    _pressed = true;
                    if (CurrentArrow != null && isShotting)
                    {
                        CurrentArrow.SetToRope(RopeTransform, transform);
                        //BowAudio.pitch = Random.Range(0.8f, 1.2f);
                        _keysoundbow = SoundManager.I.PlaySFX2(TypeSound.SFX_Bow);
                        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                    }
                    break;

                case TouchPhase.Ended:
                    _pressed = false;
                    if (CurrentArrow != null && isShotting )
                    {
                        isShotting = false;
                        StartCoroutine(RopeReturn());
                        //CurrentArrow.Shot(ArrowSpeed * Tension);
                        //Tension = 0;
                        CurrentArrow.Shot(FixedArrowSpeed);
                        //SoundManager.I.StopSFX(_keysoundbow);
                        //ArrowAudio.pitch = Random.Range(0.8f, 1.2f);
                        SoundManager.I.PlaySFX2(TypeSound.SFX_Arrow);
                        HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                        CurrentArrow = null;
                        StartCoroutine(SpawnArrowAfterDelay(timeshot));
                    }
                    break;
            }
        }

        if (_pressed && isShotting)
        {
            if (Tension < 1f)
            {
                Tension += Time.deltaTime;
            }
            RopeTransform.localPosition = Vector3.Lerp(RopeNearLocalPosition, RopeFarLocalPosition, Tension);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
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

    private void SpawnArrow()
    {
        //CurrentArrow = Instantiate(ar).GetComponent<Arrow>();
        CurrentArrow = LeanPool.Spawn(ar).GetComponent<Arrow>();
        CurrentArrow.SetToRope(RopeTransform, transform);
        isShotting = true;
        Tension = 0;
        if (_pressed)
        {
            //BowAudio.pitch = Random.Range(0.8f, 1.2f);
            SoundManager.I.PlaySFX2(TypeSound.SFX_Bow);
            HapticManager.I.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }

    private IEnumerator SpawnArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
    }

    IEnumerator RopeReturn()
    {
        Vector3 startLocalPosition = RopeTransform.localPosition;
        for (float f = 0; f < 1f; f += Time.deltaTime / ReturnTime)
        {
            RopeTransform.localPosition = Vector3.LerpUnclamped(startLocalPosition, RopeNearLocalPosition, RopeReturnAnimation.Evaluate(f));
            yield return null;
        }
        RopeTransform.localPosition = RopeNearLocalPosition;
    }
}





