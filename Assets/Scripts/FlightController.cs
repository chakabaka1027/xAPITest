using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour {

    PlayerController playerController;
    
    Vector3 targetWalkAmount;
    Vector3 walkAmount;
    Vector3 smoothDampMoveRef;
    CharacterController controller;
    float speed = 13;
    public float velocityY;

    [Header("Look Controls")]
	[Range (-10, 10)]
	public float mouseSensitivityX = 3f;
	[Range (-10, 10)]
	public float mouseSensitivityY = 3f;
	float verticalLookRotation;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
        controller = FindObjectOfType<CharacterController>();

	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.T)){
            playerController.flyMode = true; 
        }

        if(playerController.flyMode){

            yaw += mouseSensitivityX * Input.GetAxis("Mouse X");
            pitch -= mouseSensitivityY * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);

            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            Vector3 newRight = Vector3.Cross(Vector3.up, transform.worldToLocalMatrix.MultiplyVector(transform.forward));
            Vector3 newForward = Vector3.Cross(newRight, Vector3.up);	

            Vector3 trueMoveDir = (newRight * Input.GetAxisRaw("Horizontal") + newForward * Input.GetAxisRaw("Vertical"));
            targetWalkAmount = trueMoveDir * speed + Vector3.up * velocityY;
            walkAmount = Vector3.SmoothDamp(walkAmount, targetWalkAmount, ref smoothDampMoveRef, 0.1f);
            transform.Translate(walkAmount * Time.fixedDeltaTime);


            if(Input.GetKey(KeyCode.E)){
                transform.Translate(Vector3.up * 13 * Time.fixedDeltaTime);
            }

            if(Input.GetKey(KeyCode.Q)){
                transform.Translate(-Vector3.up * 13 * Time.fixedDeltaTime);
            }
        }
    }
}
