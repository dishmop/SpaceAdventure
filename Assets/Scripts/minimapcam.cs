using UnityEngine;
using System.Collections;

public class minimapcam : MonoBehaviour {
    public GameObject player;
    public GameObject bigarrow;
    public Camera maincam;

    float z;
    float size;
    float mainsize;

	// Use this for initialization
	void Start () {
        z = transform.position.z;
        size = GetComponent<Camera>().orthographicSize;
        mainsize = maincam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, z);
        bigarrow.transform.position = player.transform.position;
        bigarrow.transform.rotation = player.transform.rotation;

        GetComponent<Camera>().orthographicSize = size * maincam.orthographicSize / mainsize;
	}
}
