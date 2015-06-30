using UnityEngine;
using System.Collections;

public class minimapcam : MonoBehaviour {
    public GameObject player;
    public GameObject bigarrow;

    float z;

	// Use this for initialization
	void Start () {
        z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, z);
        bigarrow.transform.position = player.transform.position;
        bigarrow.transform.rotation = player.transform.rotation;
	}
}
