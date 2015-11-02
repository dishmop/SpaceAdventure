using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class QuitOnEsc : MonoBehaviour {
	public static QuitOnEsc singleton = null;

	public string OnQuitLevelName;
	public string finalQuitURL = "http://google.com";
	
	
	void Start(){
		if (Application.loadedLevelName != "menu"){
//			Debug.Log("levelStart - levelName: " + Application.loadedLevelName);
			GoogleAnalytics.Client.SendEventHit("gameFlow", "levelStart_" + Application.loadedLevelName );
//			Analytics.CustomEvent("levelStart", new Dictionary<string, object>
//			                      {
//				{ "levelName", Application.loadedLevelName },
//			});			
		}
	}
	
	// Update is called once per frame
	void Update () {

		
		// Test for exit
		if (UnityEngine.Input.GetKeyDown (KeyCode.Escape)) {
			TriggerQuit();
		}
	}
	
	public void TriggerQuit(){
		if (OnQuitLevelName != null && OnQuitLevelName != ""){
//			Debug.Log ("quitGame - levelName :" + Application.loadedLevelName + ", levelTime: " + Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "quitGame",  Application.loadedLevelName, Time.timeSinceLevelLoad);
			
//			Analytics.CustomEvent("quitGame", new Dictionary<string, object>
//			{
//				{ "levelName", Application.loadedLevelName },
//				{ "levelTime", Time.timeSinceLevelLoad },
//			});	
			Application.LoadLevel(OnQuitLevelName);
		}
		else{
			Quit();
		}
		
	}
	
	//#if UNITY_WEBPLAYER
	//#endif
	public void Quit()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBPLAYER
		if (finalQuitURL != ""){
			Application.OpenURL(finalQuitURL);
		}
		#else
		if (finalQuitURL != ""){
			Application.OpenURL(finalQuitURL);
		}
		Application.Quit();
		#endif
	}
	
	// Use this for initialization
	void Awake () {
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
		
	}
	
	
	void OnDestroy(){
		
		singleton = null;
	}	
}
