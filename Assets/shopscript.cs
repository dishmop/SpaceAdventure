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
    float tractornaturallength = 100f;
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

        Vector2 relvel = SaucerPlayer.instance.GetComponent<Rigidbody2D>().velocity - GetComponent<Rigidbody2D>().velocity;

        float speedtowards = Mathf.Clamp(-Vector2.Dot(relvel, offsetnorm), 0, float.MaxValue);

        if (offset.magnitude < tractorrange)
        {
            shopbutton.SetActive(true);
            beam.SetActive(true);
            SetBeam(transform.position, transform.position + offset);

            float amount = 1- 2*(offsetlength - tractornaturallength)/(tractorrange-tractornaturallength);
            amount = Mathf.Clamp01(amount);

            float playermass = SaucerPlayer.instance.GetComponent<Rigidbody2D>().mass;

            GetComponent<Rigidbody2D>().AddForce(- playermass *amount * speedtowards * offsetnorm,ForceMode2D.Impulse);
            SaucerPlayer.instance.GetComponent<Rigidbody2D>().AddForce(playermass * amount * speedtowards * offsetnorm, ForceMode2D.Impulse);
        }
        else
        {
            beam.SetActive(false);
            shopbutton.SetActive(false);
        }
	}
}
