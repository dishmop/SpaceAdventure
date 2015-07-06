using UnityEngine;
using System.Collections;

public class shopscript : MonoBehaviour {
    public GameObject beam;
    public GameObject shopbutton;

    void SetBeam(Vector3 from, Vector3 to)
    {
        beam.transform.position = (from + to) / 2;
        Quaternion rotation = Quaternion.LookRotation
            (to - transform.position, beam.transform.TransformDirection(Vector3.up));
        beam.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        float distance = (to - from).magnitude;
        beam.transform.localScale = new Vector3(distance / (5 * transform.localScale.x), 1, 1);
    }

	// Use this for initialization
	void Start () {
	
	}

    float radius = 69;
    float tractorrange = 200;
    float tractornaturallength = 150f;
    float tractorstrength = 100f;

    float A = 10;

    public float forcestrength;

    public float offsetlength;

	// Update is called once per frame
	void Update () {
        transform.position = GameController.instance.areapos +  GameController.instance.currentproperties.localshoppos;

        Vector3 offset = SaucerPlayer.instance.transform.position - transform.position;
        Vector2 offsetnorm = new Vector2(offset.x, offset.y);
        offsetnorm.Normalize();

        offsetlength = offset.magnitude;

	    if(offset.magnitude<tractorrange)
        {
            shopbutton.SetActive(true);
            beam.SetActive(true);
            SetBeam(transform.position, transform.position + offset);

            float B = A / ((tractornaturallength - radius) * (tractornaturallength - radius));


            if(offset.magnitude < tractornaturallength)
            {
                forcestrength = A / ((offsetlength - radius) * (offsetlength - radius)) - B;

                GetComponent<Rigidbody2D>().AddForce(-offsetnorm * forcestrength);
                SaucerPlayer.instance.GetComponent<Rigidbody2D>().AddForce(offsetnorm * forcestrength);
            }
            //else if(offset.magnitude > tractornaturallength)
            //{
            //    //GetComponent<Rigidbody2D>().AddForce(offsetnorm * tractorstrength);
            //    //SaucerPlayer.instance.GetComponent<Rigidbody2D>().AddForce(-offsetnorm * tractorstrength);
            //}
        }
        else
        {
            beam.SetActive(false);
            shopbutton.SetActive(false);
        }
	}
}
