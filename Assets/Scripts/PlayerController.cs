using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(GravityShift))]
[RequireComponent(typeof(Grab))]
public class PlayerController : MonoBehaviour {
    [HideInInspector]
	public bool isSlowing = false;

    //Player Stats
    public float health = 100;
    public float stamina = 100;

	//Crouch
    public LayerMask ground;
    bool isGrounded;
    int crouchToggle = 0;
    [HideInInspector]
    public float height = .75f;
    float crouchLerpPercent = 0;

    //flip grav
	GravityShift gravityShift;
    bool hitCeiling = false;

    //movement
    public bool flyMode = false;

    public float speed = 4;
    public float walkSpeed = 4;
    public float runSpeed = 8;
    public float maxWalkSpeed = 4;
    public float maxRunSpeed = 8;

    Vector3 targetWalkAmount;
    Vector3 walkAmount;
    Vector3 smoothDampMoveRef;
    CharacterController controller;

    public float gravity = -12;
    public float velocityY;

    [Header("Look Controls")]
	[Range (-10, 10)]
	public float mouseSensitivityX = 6f;
	[Range (-10, 10)]
	public float mouseSensitivityY = 6f;
	float verticalLookRotation;

    //UI
    Image bodyUI;
    Image headUI;
    float headGbValue = 1;
    float bodyGbValue = 1;
    Image healthUI;


	void Start () {
        controller = GetComponent<CharacterController>();
        gravityShift = GetComponent<GravityShift>();
        
        bodyUI = GameObject.Find("BodyUI").GetComponent<Image>();
        headUI = GameObject.Find("HeadUI").GetComponent<Image>();
        healthUI = GameObject.Find("Health Bar").GetComponent<Image>();

        ResetBlurAmount();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
	}
	
    void Update() {

    if(!flyMode && FindObjectOfType<UI_SubmitPlayerInfo>().submittedPlayerInfo){
        //set movespeed

            if(Input.GetKey(KeyCode.LeftShift)){
        	    speed = runSpeed;
		    } else {
			    speed = walkSpeed;
		    }
        
            if (stamina <= 0) {
                speed = walkSpeed;
            } 

	    //shift gravity
		    if(Input.GetKeyDown(KeyCode.Space)){
			    gravityShift.ShiftGravity();
		    }

	    //slow time
		    if(Input.GetMouseButton(1)){
			    Time.timeScale = .25f;
			    Time.fixedDeltaTime = Time.timeScale * .02f;
			    isSlowing = true;
		    } else if (Input.GetMouseButtonUp(1)){
			    Time.timeScale = 1;
			    Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
			    isSlowing = false;
		    }
        }
    }

	void LateUpdate () {
        if(!flyMode && FindObjectOfType<UI_SubmitPlayerInfo>().submittedPlayerInfo){
	    //crouch
		    if(Input.GetKeyDown(KeyCode.C)){
			    crouchToggle = 1 - crouchToggle;
			    StopCoroutine("CrouchToggle");
			    StartCoroutine("CrouchToggle");
		    }

	    //look rotations
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
	        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
		    Camera.main.transform.localEulerAngles = Vector3.left * verticalLookRotation;

            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            Vector3 newRight = Vector3.Cross(Vector3.up, gameObject.transform.forward);
            Vector3 newForward = Vector3.Cross(newRight, Vector3.up);

        //movement
            if(gravityShift.gravityShifted == 0) {
                Vector3 trueMoveDir = (newRight * Input.GetAxisRaw("Horizontal") + newForward * Input.GetAxisRaw("Vertical"));
                targetWalkAmount = trueMoveDir * speed + Vector3.up * velocityY;
		        walkAmount = Vector3.SmoothDamp(walkAmount, targetWalkAmount, ref smoothDampMoveRef, 0.1f);
                controller.Move(walkAmount * Time.fixedDeltaTime);
        //reverse controls if gravity is shifted
            } else if (gravityShift.gravityShifted == 1) {
                Vector3 trueMoveDir = (-newRight * Input.GetAxisRaw("Horizontal") + newForward * Input.GetAxisRaw("Vertical"));
                targetWalkAmount = trueMoveDir * speed + Vector3.up * velocityY;

		        walkAmount = Vector3.SmoothDamp(walkAmount, targetWalkAmount, ref smoothDampMoveRef, 0.1f);
                controller.Move(walkAmount * Time.fixedDeltaTime);

            }

            velocityY += Time.deltaTime * gravity;

            //detect when grounded or when hitting ceiling
            HitGround();
		    StartCoroutine("HitCeiling");
        }
	}

	IEnumerator HitCeiling(){
		Ray ray = new Ray(transform.position, transform.up);

        if(Physics.SphereCast(ray, .5f, height, ground) && !hitCeiling) {
			hitCeiling = true;
			velocityY = 0;

			yield return new WaitForSeconds(0.5f);
			hitCeiling = false;        
		}  
       
	}

	void HitGround(){
		Ray ray = new Ray(transform.position, -transform.up);

        if(Physics.SphereCast(ray, .5f, height, ground)) {
            velocityY = 0;
        }  
	}


	IEnumerator CrouchToggle(){
		float time = .25f;
		float speed = 1 / time;

		//crouch
		if(crouchToggle == 1){
			height = .25f;
			while(crouchLerpPercent < 1){
				crouchLerpPercent += Time.fixedDeltaTime * speed;
				gameObject.transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, .5f, 1), crouchLerpPercent);
				yield return null;
			}
			crouchLerpPercent = 1;

		//stand up
		} if(crouchToggle == 0){
			
			StartCoroutine("LiftWhenUncrouching");
			height = .75f;
			while(crouchLerpPercent > 0){
				crouchLerpPercent -= Time.fixedDeltaTime * speed;
				gameObject.transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), (1 - crouchLerpPercent));
				yield return null;
			}

			crouchLerpPercent = 0;

		}
	}

	IEnumerator LiftWhenUncrouching(){
		Ray ray = new Ray(transform.position, -transform.up);

		//move character up when uncrouching 
        if(Physics.SphereCast(ray, .5f, height, ground)) {
			float percent = 0;
			float t = .3f;
			float s = 1 / t;
			while(percent < 1){
				percent += Time.fixedDeltaTime * s;
				if(gravityShift.gravityShifted == 0){
					gameObject.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.up * .02f, percent);
				} else if(gravityShift.gravityShifted == 1){
					gameObject.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - Vector3.up * .02f, percent);
				} 
				yield return null;
			}
        }  
	}

	//push rigidbody objects upon colliding with them
	void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;
        
        if (hit.moveDirection.y < -0.3F)
            return;
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * 5;
    }


    public void DamageBody(){
        bodyGbValue -= .05f;
        health -= 2.5f;

        maxWalkSpeed /= 1.05f;
        maxRunSpeed /= 1.05f;

        if(health <= 0){
            Die();
        }
        healthUI.fillAmount = health / 100;

        if(bodyGbValue < 0){
            bodyGbValue = 0;
        }
        bodyUI.color = new Color(1, bodyGbValue, bodyGbValue, 1);
    }

    public void DamageHead(){
        headGbValue -= .2f;
        health -= 10;

        BlurVision();

        if(health <= 0){
            Die();
        }
        healthUI.fillAmount = health / 100;

        if(headGbValue < 0){
            headGbValue = 0;
        }
        headUI.color = new Color(1, headGbValue, headGbValue, 1);
    }

    void Die(){
        SceneManager.LoadScene("Game");
    }

    void BlurVision(){
        PostProcessingProfile profile = Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile;

        //copy current bloom settings from the profile into a temporary variable
        MotionBlurModel.Settings motionBlurSettings = profile.motionBlur.settings;
        ColorGradingModel.Settings colorGradingSettings = profile.colorGrading.settings;
        //change the intensity in the temporary settings variable
        motionBlurSettings.frameBlending += .2f;
        
        colorGradingSettings.basic.saturation -= .15f;
        if(colorGradingSettings.basic.saturation < .2f){
            colorGradingSettings.basic.saturation = .2f;        
        }

        //set the bloom settings in the actual profile to the temp settings with the changed value
        profile.motionBlur.settings = motionBlurSettings;
        profile.colorGrading.settings = colorGradingSettings;
    }

    void ResetBlurAmount(){
        PostProcessingProfile profile = Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile;
        MotionBlurModel.Settings motionBlurSettings = profile.motionBlur.settings;
        motionBlurSettings.frameBlending = 0f;
        profile.motionBlur.settings = motionBlurSettings;

        ColorGradingModel.Settings colorGradingSettings = profile.colorGrading.settings;
        colorGradingSettings.basic.saturation = 1;
        profile.colorGrading.settings = colorGradingSettings;
    }

    
}
