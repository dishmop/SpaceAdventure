using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mintextscript : MonoBehaviour {
    public int mineralnumber;
    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	    text.text = GameController.instance.MineralName[mineralnumber] + ": " + SaucerPlayer.instance.sc.mineralmass[mineralnumber].ToString("F")+"kg";
	}
}
