using UnityEngine;
using System.Collections;

public class quitscript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Quit()
    {
        QuitOnEsc.singleton.TriggerQuit();
    }
}
