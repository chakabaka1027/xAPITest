using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour {

    void OnControllerColliderHit(ControllerColliderHit hit ){
        if (hit.collider.tag == "ClingOn"){
            hit.collider.SendMessageUpwards("Damage"); 
        }
    }
}
