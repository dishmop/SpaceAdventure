using UnityEngine;
using System.Collections;

public class backgrounds : MonoBehaviour {
    public GameObject LT;
    public GameObject RT;
    public GameObject LB;
    public GameObject RB;

    float bgw = 1920f;
    float bgh = 1080f;

	void Update () {
        Vector3 lb = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f));
        Vector3 rt = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));

        float camleft = lb.x;
        float cambottom = lb.y;
        float camtop = rt.y;
        float camright = rt.x;

        LT.transform.position = new Vector3(Mathf.Round(camleft / bgw) * bgw,  Mathf.Round(camtop / bgh) * bgh);
        RT.transform.position = new Vector3(Mathf.Round(camright / bgw) * bgw, Mathf.Round(camtop / bgh) * bgh);
        LB.transform.position = new Vector3(Mathf.Round(camleft / bgw) * bgw,  Mathf.Round(cambottom / bgh) * bgh);
        RB.transform.position = new Vector3(Mathf.Round(camright / bgw) * bgw, Mathf.Round(cambottom / bgh) * bgh);
	}
}
