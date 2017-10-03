using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour {
   
    public LayerMask environment;

    int flightToggle = 0;
    GameObject groundUI;

    PlayerController playerController;
    
    Vector3 targetWalkAmount;
    Vector3 walkAmount;
    Vector3 smoothDampMoveRef;
    CharacterController controller;
    float speed;
    float slowSpeed = 8;
    float fastSpeed = 26;
    float velocityY = 0;

    [Header("Look Controls")]
	[Range (-10, 10)]
	public float mouseSensitivityX = 3f;
	[Range (-10, 10)]
	public float mouseSensitivityY = 3f;
	float verticalLookRotation;

    float yaw = 0.0f;
    float pitch = 0.0f;

	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
        controller = FindObjectOfType<CharacterController>();
        groundUI = GameObject.Find("GroundUI");
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.T)){
            ToggleFlightMode();
        }

        if(Input.GetKey(KeyCode.LeftShift)){
            speed = fastSpeed;
        } else {
            speed = slowSpeed;
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

            if(Input.GetMouseButton(0)){
                ChangePlayerLocation();
            }
        }
    }

    void ToggleFlightMode(){
        flightToggle = 1 - flightToggle;

        //flight on
        if(flightToggle == 1){
            playerController.flyMode = true; 
            groundUI.SetActive(false);
            playerController.gameObject.transform.Find("TempMesh").GetComponent<MeshRenderer>().enabled = true;
            transform.parent.DetachChildren();

        
        //flight off
        } else if (flightToggle == 0){
            playerController.flyMode = false; 
            groundUI.SetActive(true);
            Camera.main.transform.localEulerAngles = Vector3.zero;
            Camera.main.transform.position = GameObject.Find("CameraDock").transform.position;
            playerController.gameObject.transform.Find("TempMesh").GetComponent<MeshRenderer>().enabled = false;
            transform.parent = GameObject.Find("CameraDock").transform;

        }

    }

    void ChangePlayerLocation(){
        Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.SphereCast(ray, 1f, Camera.main.transform.forward, out hit, 50f, environment)){
            playerController.gameObject.transform.position = hit.point + hit.normal * 1.25f;
        }
    }
}
