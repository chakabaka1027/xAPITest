using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinCan;
using TinCan.LRSResponses;

//network clearance
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class LRS : MonoBehaviour {

    public string playerName = "";
    public string email = "";

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
		bool isOk = true;
		// If there are errors in the certificate chain, look at each error to determine the cause.
		if (sslPolicyErrors != SslPolicyErrors.None) {
			for (int i=0; i<chain.ChainStatus.Length; i++) {
				if (chain.ChainStatus [i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					bool chainIsValid = chain.Build ((X509Certificate2)certificate);
					if (!chainIsValid) {
						isOk = false;
					}
				}
			}
		}
		return isOk;
	}

    private void OnTriggerEnter(Collider other) {
        //string name = "Alex";
        //string email = "alex.cha@mtsi-va.com";

        SendMessage(playerName, email);
    }

    void SendMessage(string playerName, string email){
        float health = FindObjectOfType<PlayerController>().health;
        float stamina = FindObjectOfType<PlayerController>().stamina;        
        
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

		var lrs = new RemoteLRS(
            "https://cloud.scorm.com/tc/B0MLOLLYBS/sandbox/",
            "alex.cha@mtsi-va.com",
            "Abcd1234"
        );
        
        var actor = new Agent();
        //actor.mbox = "mailto:info@tincanapi.com";
        actor.mbox = "mailto:" + email;
        actor.name = playerName;

        var verb = new Verb();
        verb.id = new Uri ("http://activitystrea.ms/schema/1.0/submit");
        verb.display = new LanguageMap();
        verb.display.Add("en-US", "submitted");

        var activity = new Activity();
        //id
        activity.id = "http://adlnet.gov/expapi/activities/performance";
        
        //definition
        activity.definition = new ActivityDefinition();
            
            //definition name
        activity.definition.name = new LanguageMap();
        activity.definition.name.Add("en-US", "Stamina");

            //definition description
        activity.definition.description = new LanguageMap();
        activity.definition.description.Add("en-US", "The Player's Stamina Score");

            //definition type
        activity.definition.type = new Uri("http://adlnet.gov/expapi/activities/performance");
        
        //result
        var result = new Result();
        result.completion = true;
        result.success = true;
        result.score = new Score();
        result.score.raw = stamina;
        
        //declaring the full statement
        var statement = new Statement();
        statement.actor = actor;
        statement.verb = verb;
        statement.target = activity;
        statement.result = result;


        StatementLRSResponse lrsResponse = lrs.SaveStatement(statement);
        if (lrsResponse.success){
            // Updated 'statement' here, now with id
            Debug.Log("Save statement: " + lrsResponse.content.id);
        }
        else {
            // Do something with failure
            Debug.Log("YOU SUCK!");
        }    
    }
}
