using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mintextscript : MonoBehaviour {
    public int mineralnumber;
    Text text;

    public Image halo;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = GameController.instance.MineralName[mineralnumber] + " @ $" + GameController.instance.MineralValue[mineralnumber].ToString("F0") + "/kg : " + SaucerPlayer.instance.sc.mineralmass[mineralnumber].ToString("F") + "kg";
        halo.color = GameController.instance.MineralColor[mineralnumber];
    }
}
