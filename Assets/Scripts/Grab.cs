using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {
    
    public LayerMask grabable;
    //public LayerMask checkCollisions;
    bool hasCollided;

    bool isGrabbing = false;

    GameObject grabbedObject;
    float positionLerpSpeed = 30;
    float rotationLerpSpeed = 25;
	Material grabbedObjColor;
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.F)) {
            PickUp();
        }

        if(Input.GetMouseButtonDown(0) && isGrabbing) {
           Throw();
        }

        if(FindObjectOfType<PlayerController>().isSlowing){
        	//positionLerpSpeed = 30 * 4;
        	//rotationLerpSpeed = 25 * 4;
		} else if (FindObjectOfType<PlayerController>().isSlowing){
			positionLerpSpeed = 30;
    		rotationLerpSpeed = 25;		
    	}
	}

	void LateUpdate(){
		if(isGrabbing){
        	Carry(grabbedObject);
        	CheckCollisions();
        }
	}

    private void PickUp() {
        Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.SphereCast(ray, .4f, Camera.main.transform.forward, out hit, 2.5f, grabable) && !isGrabbing) {
            grabbedObject = hit.collider.gameObject;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

			grabbedObjColor = grabbedObject.GetComponent<Renderer>().material;
			grabbedObjColor.color = new Color(grabbedObjColor.color.r, grabbedObjColor.color.g, grabbedObjColor.color.b, .8f);

            if(grabbedObject.GetComponent<BoxCollider>() != null) {
                grabbedObject.GetComponent<BoxCollider>().enabled = false;
            } else if(grabbedObject.GetComponent<SphereCollider>() != null) {
                grabbedObject.GetComponent<SphereCollider>().enabled = false;
            }

            isGrabbing = true;
        } else if(isGrabbing) {
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;

			grabbedObjColor = grabbedObject.GetComponent<Renderer>().material;
			grabbedObjColor.color = new Color(grabbedObjColor.color.r, grabbedObjColor.color.g, grabbedObjColor.color.b, 1);

            if(grabbedObject.GetComponent<BoxCollider>() != null) {
                grabbedObject.GetComponent<BoxCollider>().enabled = true;
            } else if(grabbedObject.GetComponent<SphereCollider>() != null) {
                grabbedObject.GetComponent<SphereCollider>().enabled = true;
            }            
            isGrabbing = false;
        }
    }
    
    void Throw() {
		grabbedObjColor = grabbedObject.GetComponent<Renderer>().material;
		grabbedObjColor.color = new Color(grabbedObjColor.color.r, grabbedObjColor.color.g, grabbedObjColor.color.b, 1);

		isGrabbing = false;        
        grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        if(grabbedObject.GetComponent<BoxCollider>() != null) {
            grabbedObject.GetComponent<BoxCollider>().enabled = true;
        } else if(grabbedObject.GetComponent<SphereCollider>() != null) {
            grabbedObject.GetComponent<SphereCollider>().enabled = true;
        }               
        grabbedObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 40, ForceMode.Impulse);        

    }

    void Carry(GameObject obj){
    	if(!hasCollided){
    		obj.transform.position = Vector3.Lerp(obj.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2, Time.fixedUnscaledDeltaTime * positionLerpSpeed);
			obj.transform.eulerAngles = Vector3.Lerp(obj.transform.eulerAngles, new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z), Time.fixedUnscaledDeltaTime * rotationLerpSpeed);
		}
    }

    void CheckCollisions(){
		Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.SphereCast(ray, .5f, Camera.main.transform.forward, out hit, 2f)) {
        	hasCollided = true;
        	//grabbedObject.transform.position = hit.point + hit.normal * .5f;

			grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, hit.point + hit.normal * .5f, Time.fixedUnscaledDeltaTime * positionLerpSpeed);
			grabbedObject.transform.eulerAngles = Vector3.Lerp(grabbedObject.transform.eulerAngles, new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z), Time.fixedUnscaledDeltaTime * rotationLerpSpeed);
        } else {
        	hasCollided = false;
        }
    }
}
//new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized