using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class StaminaSystem : MonoBehaviour {

    PlayerController playerController;
    public Image staminaBar;

	// Use this for initialization
	void Start () {
        playerController = GetComponent<PlayerController>();
		
	}
	
	// Update is called once per frame
	void Update () {

        if(!playerController.flyMode){
		    if(Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0 || Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") != 0){
                playerController.stamina -= Time.deltaTime * 25;
            } else {
                playerController.stamina += Time.deltaTime * 15;
            }

            if(playerController.stamina > 100){
                playerController.stamina = 100;
            } else if (playerController.stamina < 0){
                playerController.stamina = 0;            
            }

            staminaBar.fillAmount = playerController.stamina/100;
        }
	}
}
