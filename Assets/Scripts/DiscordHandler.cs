using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscordHandler : MonoBehaviour {
	public static DiscordHandler instance;
	public DiscordController d;
	float uploadToDiscordCd = 0;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{		
		uploadToDiscordCd -= Time.deltaTime;
		if(uploadToDiscordCd <= 0)
		{
			uploadToDiscordCd = 3f;
			DiscordRpc.UpdatePresence(d.presence);
			//Debug.Log("update presense!");
		}
	}

	public void SetPresence(string detail="Playing ???", string state = "0 (0%) | 0x")
	{
		d.presence.state = state;
		d.presence.details = detail; //string.Format("{0:N0} ({1:N2}%) | {2:N0}x",  score, combo);
		d.presence.smallImageKey = "thinking";
		d.presence.smallImageText = "master BATE - LV.69";
		d.presence.largeImageKey = "bbe";
		d.presence.largeImageText = "nou! iz tha bast muzic gay evar!!";
	}
}
