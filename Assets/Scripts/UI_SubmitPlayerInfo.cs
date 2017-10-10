using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubmitPlayerInfo : MonoBehaviour {

    [HideInInspector]
    public bool submittedPlayerInfo = false;
    public GameObject playerIcon;

    LRS lrsScript;

    InputField playerName;
    InputField playerEmail;
    Button submitButton;

	// Use this for initialization
	void Start () {
		playerName = GameObject.Find("PlayerName").GetComponent<InputField>();
		playerEmail = GameObject.Find("PlayerEmail").GetComponent<InputField>();
        submitButton = GameObject.Find("SubmitButton").GetComponent<Button>();

        lrsScript = FindObjectOfType<LRS>();

        if(!submittedPlayerInfo){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
	}
	
    void Update(){
        if(playerName.text.Length > 0 && playerEmail.text.Length > 0){
            submitButton.interactable = true;
        } else {
            submitButton.interactable = false;
        }
    }

	public void SubmitPlayerInformation(){
        lrsScript.playerName = playerName.text;
        lrsScript.email = playerEmail.text;
        submittedPlayerInfo = true;
        GameObject.Find("PlayerInformationUI").SetActive(false);
        playerIcon.GetComponent<TextMesh>().text = playerName.text;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
}
