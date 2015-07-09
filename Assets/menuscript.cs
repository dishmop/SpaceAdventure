using UnityEngine;
using System.Collections;

public class menuscript : MonoBehaviour {

    public GameObject bg;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.Mouse0))
        {
            Application.LoadLevel(1);
        }

        float bgsize = GetComponent<Camera>().orthographicSize / 2.4f;
        bg.transform.localScale = new Vector3(bgsize, bgsize, 1);

	}
}
