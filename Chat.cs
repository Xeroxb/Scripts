using System;
using UnityEngine;
using System.Collections.Generic;

public class Chat : MonoBehaviour {
	
	//Historico do chat
	public List<string> historicoChat = new List<string>();
	
	//Segue a mensagem atual, que esta sendo digitada
	private string mensagemAtual = String.Empty;
	
	//Enviar mensagem
	void EnviarMensagem() {
		
		if (string.IsNullOrEmpty(mensagemAtual.Trim())) return; //se nao tiver mensagem, a string recebe a mensagem que for digitada
		{
			GameObject.Find("myPlayer").GetComponent<MessagesController>().CmdChatMessage(mensagemAtual);
			mensagemAtual = "";
		}
	}
	
	//(Chat embaixo)
	private void ChatBaixo() {
		mensagemAtual = GUI.TextField(new Rect(0, Screen.height*0.8f, Screen.width*0.2f, 20), mensagemAtual); // aqui a barra do chat recebe a mensagem digitada
		if (GUI.Button (new Rect (Screen.width * 0.2f, Screen.height * 0.8f, 75, 20), "Enviar")) { //aqui manda a mensagem digitada atraves do botao "enviar"
			EnviarMensagem ();
		} if (Input.GetKey(KeyCode.Return))
		{
			EnviarMensagem ();
		}
		
		GUILayout.Space(15); //aqui um label recebe tudo o q foi digitado (HistoricoChat)
		for (int i = historicoChat.Count - 1; i >= 0; i--)
			GUILayout.Label(historicoChat[i]);
	}
	
	private void TopChat()//funçao responsavel pelo layout do box do chat
	{
		GUILayout.Space(15);
		GUILayout.BeginHorizontal(GUILayout.Width(250));
		mensagemAtual = GUILayout.TextField(mensagemAtual);
		if (GUILayout.Button("Enviar"))
		{
			EnviarMensagem();
		}
		else if ((Event.current.type == EventType.KeyDown)||(Event.current.keyCode == KeyCode.Return))
		//if (Event.current.Equals (Event.KeyboardEvent("return")))
		//if (Input.GetKey(KeyCode.Return))
		{
			EnviarMensagem ();
		}
		GUILayout.EndHorizontal();
		foreach (string c in historicoChat)
			GUILayout.Label(c);
	}
	
	
	//O layout da caixa de mensagem
	void OnGUI() {
		ChatBaixo();
	}
}