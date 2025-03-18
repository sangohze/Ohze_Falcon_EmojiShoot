using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    public float Tension;
    private bool _pressed;
    public Transform RopeTransform;
    public Vector3 RopeNearLocalPosition;
    public Vector3 RopeFarLocalPosition;
    public AnimationCurve RopeReturnAnimation;
    public float ReturnTime;
    public Arrow CurrentArrow = null;
    public float ArrowSpeed;
    public AudioSource ArrowAudio;
    public AudioSource BowAudio;
    private int ArrowIndex = 0;
    public GameObject ar;

    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    [SerializeField] private float timeshot = 1f;

    

    void Start()
    {
        RopeNearLocalPosition = RopeTransform.localPosition;
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
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

        if (Input.GetMouseButtonDown(0))
        {
            _pressed = true;
            if (CurrentArrow != null)
            {
                CurrentArrow.SetToRope(RopeTransform, transform);
                BowAudio.pitch = Random.Range(0.8f, 1.2f);
                BowAudio.Play();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _pressed = false;

            if (CurrentArrow != null)
            {
                StartCoroutine(RopeReturn());
                CurrentArrow.Shot(ArrowSpeed * Tension);
                Tension = 0;

                BowAudio.Stop();

                ArrowAudio.pitch = Random.Range(0.8f, 1.2f);
                ArrowAudio.Play();
                CurrentArrow = null;

                // Spawn a new arrow after the current arrow is destroyed
                StartCoroutine(SpawnArrowAfterDelay(timeshot)); // Adjust delay as needed
            }
        }

        if (_pressed)
        {
            if (Tension < 1f)
            {
                Tension += Time.deltaTime;
            }
            RopeTransform.localPosition = Vector3.Lerp(RopeNearLocalPosition, RopeFarLocalPosition, Tension);
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

    private void SpawnArrow()
    {
        CurrentArrow = Instantiate(ar).GetComponent<Arrow>();
        CurrentArrow.SetToRope(RopeTransform, transform);

        // Change the material of the arrow based on the current emoji
     
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


