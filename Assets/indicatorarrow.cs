using UnityEngine;
using System.Collections;

public class indicatorarrow : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = Camera.main.gameObject.transform.position;
        pos.z = 0;
        Vector3 offset = target.transform.position - pos;

        pos += offset.normalized * Camera.main.orthographicSize*0.7f;
        transform.position = pos;

        transform.localScale = new Vector3(Camera.main.orthographicSize * 0.3f, Camera.main.orthographicSize*0.3f, 1);

        Quaternion rotation = Quaternion.LookRotation
            (offset, transform.TransformDirection(Vector3.up));
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        Vector3 screenpos = 2*(Camera.main.WorldToViewportPoint(target.transform.position)-new Vector3(0.5f,0.5f,0));
        if (Mathf.Abs(screenpos.x) < 1 && Mathf.Abs(screenpos.y) < 1)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;

        }

        //transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 90)) * transform.rotation;


        //transform.LookAt(target.transform);
	}
}
