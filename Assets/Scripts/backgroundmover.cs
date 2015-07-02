using UnityEngine;
using System.Collections;

public class backgroundmover : MonoBehaviour {
    //Collider2D trigger;

    public float size = 1;


	// Use this for initialization
	void Start () {
        //trigger = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        //transform.localScale = new Vector3(size,size,1);
	}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if(other.gameObject.tag == "MainCamera")
    //    {
    //        Vector3 campos = other.gameObject.transform.position;
            
    //        Vector2 camvel = other.gameObject.GetComponent<CameraFollow>().linearvel;

    //        if (camvel.x > 0)
    //            campos.x += 3.2f;
    //        else
    //            campos.x -= 3.2f;

    //        if (camvel.y > 0)
    //            campos.y += 2.4f;
    //        else
    //            campos.y -= 2.4f;


    //        float x = Mathf.Round(campos.x / 6.4f) * 6.4f;
    //        float y = Mathf.Round(campos.y / 4.8f) * 4.8f;

    //        Vector3 position = new Vector3(x, y, 0) - new Vector3(9.6f, 7.2f, 0);

    //        leftbottom.transform.position = position;
    //        lefttop.transform.position = position + new Vector3(0, 14.4f, 0);
    //        rightbottom.transform.position = position + new Vector3(19.2f,0, 0);
    //        righttop.transform.position = position + new Vector3(19.2f, 14.4f, 0);
    //    }
    //}
}
