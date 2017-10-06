using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(UI_ItemPlacement))]
public class FlightController : MonoBehaviour {
   
    public LayerMask environment;
    [HideInInspector]
    public bool selectingItem = false;

    bool backpanning = false;

    UI_ItemPlacement itemPlacerUI;

    int flightToggle = 0;
    GameObject groundUI;

    PlayerController playerController;
    
    Vector3 targetWalkAmount;
    Vector3 walkAmount;
    Vector3 smoothDampMoveRef;
    //CharacterController controller;
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

    float yaw;
    float pitch;

	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
        //controller = FindObjectOfType<CharacterController>();
        groundUI = GameObject.Find("GroundUI");
        itemPlacerUI = FindObjectOfType<UI_ItemPlacement>();
        playerController.gameObject.transform.Find("TempMesh").Find("PlayerIcon").gameObject.SetActive(false);

	}

    void Update() {
        if(Input.GetKeyDown(KeyCode.T)){
            ToggleFlightMode();
        }

        if(Input.GetKey(KeyCode.LeftShift)){
            speed = fastSpeed;
        } else {
            speed = slowSpeed;
        }

        if(playerController.flyMode){
            if(Input.GetKeyDown(KeyCode.Space)){
                EraseObjHighlight();
                itemPlacerUI.ItemSelectionToggle();
            }
        }
        
    }

    void LateUpdate () {

        if(playerController.flyMode && !selectingItem){

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
                transform.Translate(Vector3.up * speed * Time.fixedDeltaTime, Space.Self);
            }

            if(Input.GetKey(KeyCode.Q)){
                transform.Translate(-Vector3.up * speed * Time.fixedDeltaTime, Space.Self);
            }
            
            if(Input.GetMouseButton(0)){
                ChangePlayerLocation();
            }
            
        }
       
    }

    void EraseObjHighlight() {
        if(itemPlacerUI.objCurrentlyHoveringOver != null) {
            itemPlacerUI.objCurrentlyHoveringOver.GetComponent<Outline>().eraseRenderer = true;
        }
    }

    void ToggleFlightMode(){
        flightToggle = 1 - flightToggle;

        //flight on
        if(flightToggle == 1){
            playerController.flyMode = true; 
            groundUI.SetActive(false);
            
            playerController.gameObject.transform.Find("TempMesh").GetComponent<MeshRenderer>().enabled = true;
            playerController.gameObject.transform.Find("TempMesh").Find("PlayerIcon").gameObject.SetActive(true);
            
            Vector3 targetRot = transform.eulerAngles;
            transform.parent = null;
            
            if(transform.eulerAngles.x > 90){
                pitch = -(360 - transform.eulerAngles.x);
            } else {
                pitch = transform.eulerAngles.x;
            }
            yaw = transform.eulerAngles.y;            
            StartCoroutine("PanBack");
           
        
        //flight off
        } else if (flightToggle == 0){ 
            EraseObjHighlight();

            if(FindObjectOfType<UI_ItemPlacement>().itemSelectionToggle == 1){
                itemPlacerUI.ItemSelectionToggle();
            }
            playerController.gameObject.transform.Find("TempMesh").Find("PlayerIcon").gameObject.SetActive(false);

            StopCoroutine("PanBack");
            playerController.flyMode = false; 
            groundUI.SetActive(true);
            Camera.main.transform.localEulerAngles = Vector3.zero;
            Camera.main.transform.position = GameObject.Find("CameraDock").transform.position;
            playerController.gameObject.transform.Find("TempMesh").GetComponent<MeshRenderer>().enabled = false;
            transform.parent = GameObject.Find("CameraDock").transform;

        }

    }

    IEnumerator PanBack(){
        float time = .25f;
        float speed = 1/time;
        float percent = 0;
        Vector3 targetPos = gameObject.transform.position + -transform.forward * 3;
        while (percent < 1){
            backpanning = true;
            percent += Time.deltaTime * speed;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, percent);
            yield return null;
        }
        backpanning = false;
    }

    void ChangePlayerLocation(){
        Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.SphereCast(ray, 1f, Camera.main.transform.forward, out hit, Mathf.Infinity, environment) && !backpanning){
            playerController.gameObject.transform.position = hit.point + hit.normal * 1.25f;
        }
    }
}
