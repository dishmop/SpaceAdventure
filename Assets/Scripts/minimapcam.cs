using UnityEngine;
using System.Collections;

public class minimapcam : MonoBehaviour {
    public GameObject player;
    public GameObject bigarrow;
    public Camera maincam;

    float z;
    float size;
    float mainsize;

    //public gridOverlay go;

	// Use this for initialization
	void Start () {
        z = transform.position.z;
        size = GetComponent<Camera>().orthographicSize;
        mainsize = maincam.orthographicSize;
        //go = GetComponent<gridOverlay>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, z);
        bigarrow.transform.position = player.transform.position;
        bigarrow.transform.rotation = player.transform.rotation;

        GetComponent<Camera>().orthographicSize = size * maincam.orthographicSize / mainsize;

        //if (maincam.orthographicSize < 50)
        //{
        //    go.largeStep = 50;
        //}
        //else if(maincam.orthographicSize < 100)
        //{
        //    go.largeStep = 100;
        //}
        //else if(maincam.orthographicSize <200)
        //{
        //    go.largeStep = 200;
        //}
        //else if(maincam.orthographicSize<500)
        //{
        //    go.largeStep = 400;
        //}
	}
}
