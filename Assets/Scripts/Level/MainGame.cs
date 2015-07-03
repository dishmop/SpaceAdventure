using UnityEngine;
using System.Collections;

public class MainGame : LevelAbstract
 {

	// Use this for initialization
	void Start () {
        HideText();
	}

	// Update is called once per frame
	void Update () {
        Fade();
        if(SaucerPlayer.instance.tractor == upgradelevel.none)
        {
            ShowText("Click the 'Shop' button and buy a tractor beam",1);
        } else
        {
            HideText();
        }
	}
}
