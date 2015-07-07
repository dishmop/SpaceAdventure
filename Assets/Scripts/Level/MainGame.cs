using UnityEngine;
using System.Collections;

public class MainGame : LevelAbstract
 {
    public static MainGame instance { get; private set; }

    public GameObject mintext;

    public GameObject canvas;

	// Use this for initialization
	void Start () {
        instance = this;
        HideText();
	}

    string currentmessagetext = "Click the 'Shop' button and buy a tractor beam and cargo bay.";

	// Update is called once per frame
	void Update () {
        Fade();

        if(SaucerPlayer.instance.sc.dead)
        {
            if (ShowText("GAME OVER! Press ok to start again."))
                Application.LoadLevel(Application.loadedLevel);
        }
        else {
            if (currentmessagetext != "")
            {
                if (ShowText(currentmessagetext, 3) && SaucerPlayer.instance.tractor != upgradelevel.none) currentmessagetext = "";
            }
            else
            {
                HideText();
            }
        }

	}

    int numtexts = 0;

    public void DiscoverMineral(int mineralnum)
    {
        GameController.instance.MineralDiscovered[mineralnum] = true;
        currentmessagetext = GameController.instance.MineralName[mineralnum] + " discovered!";




        GameObject newobj = (GameObject)Instantiate(mintext);

        newobj.GetComponent<mintextscript>().mineralnumber = mineralnum;
        
        newobj.transform.SetParent(canvas.transform, false);

        Vector3 pos = newobj.transform.position;
        pos.y += numtexts * 20;

        newobj.transform.position = pos;

        numtexts++;
    }
}