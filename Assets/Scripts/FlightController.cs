using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour {

    PlayerController playerController;
    
    Vector3 targetWalkAmount;
    Vector3 walkAmount;
    Vector3 smoothDampMoveRef;
    CharacterController controller;
    float speed = 4;
    public float velocityY;

    [Header("Look Controls")]
	[Range (-10, 10)]
	public float mouseSensitivityX = 6f;
	[Range (-10, 10)]
	public float mouseSensitivityY = 6f;
	float verticalLookRotation;

	// Use this for initialization
	void Start () {
		playerController = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();

	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
	    verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
		Camera.main.transform.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        Vector3 newRight = Vector3.Cross(Vector3.up, gameObject.transform.forward);
        Vector3 newForward = Vector3.Cross(newRight, Vector3.up);	

        Vector3 trueMoveDir = (newRight * Input.GetAxisRaw("Horizontal") + newForward * Input.GetAxisRaw("Vertical"));
        targetWalkAmount = trueMoveDir * speed + Vector3.up * velocityY;
		walkAmount = Vector3.SmoothDamp(walkAmount, targetWalkAmount, ref smoothDampMoveRef, 0.1f);
        controller.Move(walkAmount * Time.fixedDeltaTime);

    }
}
