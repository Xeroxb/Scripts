using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MessagesController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[Command]
	public void CmdChatMessage(string message)
	{
		RpcReceiveChatMessage(message);
		if(isLocalPlayer) GameObject.Find("GameController").GetComponent<Chat>().historicoChat.Add(message);
		//CmdChatMessage(message);
	}
	
	[ClientRpc]
	public void RpcReceiveChatMessage(string message)
	{
		GameObject.Find("GameController").GetComponent<Chat>().historicoChat.Add(message);
	}
}
