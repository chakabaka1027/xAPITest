using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShift : MonoBehaviour {

    public int gravityShifted = 0;

    Transform mainCamera;

    void Start(){
    	mainCamera = Camera.main.transform;
    }


	public void ShiftGravity(){
		gravityShifted = 1 - gravityShifted;
        if(gravityShifted == 1) {
            GetComponent<PlayerController>().gravity *= -1;
            
            //GetComponent<PlayerController>().velocityY = 0;
            StopCoroutine("SmoothRotation");
            StartCoroutine("SmoothRotation");
        } else if(gravityShifted == 0) {
            GetComponent<PlayerController>().gravity *= -1;
            //GetComponent<PlayerController>().velocityY = 0;

            StopCoroutine("SmoothRotation");
            StartCoroutine("SmoothRotation");

        }
	}

    IEnumerator SmoothRotation() {
        float percent = 0;
        float time = .4f;
        float speed = 1 / time;
        
        while(percent < 1) {
            percent += Time.smoothDeltaTime * speed;
            if(gravityShifted == 1) {
				transform.localEulerAngles = Vector3.Lerp(new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z), new Vector3(0, transform.localEulerAngles.y, 180), percent);
				//mainCamera.localEulerAngles = Vector3.Lerp(mainCamera.localEulerAngles, new Vector3(mainCamera.localEulerAngles.x * -1, 0, 0), percent);
            } else if(gravityShifted == 0) {
				transform.localEulerAngles = Vector3.Lerp(new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z), new Vector3(0, transform.localEulerAngles.y, 0), percent);
				//mainCamera.localEulerAngles = Vector3.Lerp(mainCamera.localEulerAngles, new Vector3(mainCamera.localEulerAngles.x * -1, mainCamera.localEulerAngles.y, mainCamera.localEulerAngles.z), percent);

            }
            yield return null;
        }


    }


}
