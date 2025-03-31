using UnityEngine;

public class CameraMove : MonoBehaviour {



    [Header("Sitting")]
    //public float StandingHeight = 1.5f;
    public float SittingHeight = 0.5f;
    public float SittingTime = .1f;
    // Sitting
    private float _yPosCurrent;
    private float _yPosTarget;
    private float _yPosRef;
    private bool _isSitting;
    
    public float WalkSpeed = 2f;
  

    void Awake() {
        //_yPosTarget = StandingHeight;
    }

    void Update() {
        Move();

    }

    void Move() {

        float speed = Time.deltaTime * WalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= 2.5f;

        Vector3 forwardProjection = new Vector3(transform.forward.x, 0f, transform.forward.z);
        Vector3 rightProjection = new Vector3(transform.right.x, 0f, transform.right.z);

        if (Move_Up.clickUp)
            transform.position = Vector3.Lerp(transform.position, transform.position + forwardProjection.normalized * speed, 0.1f);
        if (Move_Down.clickDown)
            transform.position = Vector3.Lerp(transform.position, transform.position - forwardProjection.normalized * speed, 0.1f);
        if (Move_Left.clickLeft)
            transform.position = Vector3.Lerp(transform.position, transform.position - rightProjection.normalized * speed, 0.1f);
        if (Move_Right.clickRight)
            transform.position = Vector3.Lerp(transform.position, transform.position + rightProjection.normalized * speed, 0.1f);


        //if (Input.GetKeyDown(KeyCode.LeftControl)) {
        //    Debug.Log("Sitting");
        //    _isSitting = !_isSitting;
        //    _yPosTarget = _isSitting ? SittingHeight : StandingHeight;
        //}
        
        //_yPosCurrent = Mathf.SmoothDamp(_yPosCurrent, _yPosTarget, ref _yPosRef, SittingTime);
        //transform.position = new Vector3(transform.position.x, _yPosCurrent, transform.position.z);
    }

  
}
