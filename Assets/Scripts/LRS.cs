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

    float health = 100;

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
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

		var lrs = new RemoteLRS(
            "https://cloud.scorm.com/tc/B0MLOLLYBS/sandbox/",
            "alex.cha@mtsi-va.com",
            "Abcd1234"
        );
        
        var actor = new Agent();
        actor.mbox = "mailto:info@tincanapi.com";

        var verb = new Verb();
        verb.id = new Uri ("http://adlnet.gov/expapi/verbs/experienced");
        verb.display = new LanguageMap();
        verb.display.Add("en-US", "experienced");

        var activity = new Activity();
        activity.id = new Uri ("http://rusticisoftware.github.io/TinCan.NET").ToString();



        var statement = new Statement();
        statement.actor = actor;
        statement.verb = verb;
        statement.target = activity;


        StatementLRSResponse lrsResponse = lrs.SaveStatement(statement);
        if (lrsResponse.success){
            // Updated 'statement' here, now with id
            Debug.Log("Save statement: " + lrsResponse.content.id);
        }
        else {
            // Do something with failure
        }    
    }
}
