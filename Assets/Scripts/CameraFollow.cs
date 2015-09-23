using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow instance { get; private set; }

    public Rigidbody2D following;
    //public backgroundmover bg;

    public GameObject minimapcube;


    [SerializeField] float linearaccel;

    [SerializeField] float alloweddistance;


    public Vector2 linearvel = new Vector2(0,0);

    public Slider zoombar;

    float minzoom = 20;
    float maxzoom = 500;

    float moverate = 1;

    void Start()
    {
        instance = this;
    }

    public float zoom
    {
        get { return GetComponent<Camera>().orthographicSize / 2.4f; }
    }


	void FixedUpdate () {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        //gallilean relativity!
        Vector2 rellinearvel = linearvel - following.velocity;

        Vector2 lineardisp = following.position - position;


        if (lineardisp.magnitude > alloweddistance)
        {
            //move towards object
            Vector2 desiredlinvel = lineardisp * moverate;

            if (rellinearvel.x < desiredlinvel.x - linearaccel * Time.fixedDeltaTime / 2) rellinearvel.x += linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.x > desiredlinvel.x + linearaccel * Time.fixedDeltaTime / 2) rellinearvel.x -= linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.y < desiredlinvel.y - linearaccel * Time.fixedDeltaTime / 2) rellinearvel.y += linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.y > desiredlinvel.y + linearaccel * Time.fixedDeltaTime / 2) rellinearvel.y -= linearaccel * Time.fixedDeltaTime;
        }
        else
        {
            //accelerate to match object
            if (rellinearvel.x < -linearaccel * Time.fixedDeltaTime / 2) rellinearvel.x += linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.x > linearaccel * Time.fixedDeltaTime / 2) rellinearvel.x -= linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.y < -linearaccel * Time.fixedDeltaTime / 2) rellinearvel.y += linearaccel * Time.fixedDeltaTime;
            if (rellinearvel.y > linearaccel * Time.fixedDeltaTime / 2) rellinearvel.y -= linearaccel * Time.fixedDeltaTime;
        }


        //calculate absolute values again
        linearvel = rellinearvel + following.velocity;

        //set position from speed
        transform.position += new Vector3(linearvel.x,linearvel.y,0) * Time.fixedDeltaTime;
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse Y") * 5f;
            zoombar.value = (GetComponent<Camera>().orthographicSize - minzoom) / (maxzoom - minzoom);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 50f;
            zoombar.value = (GetComponent<Camera>().orthographicSize - minzoom) / (maxzoom - minzoom);
        }
        else
        {
            GetComponent<Camera>().orthographicSize = minzoom + (maxzoom - minzoom) * zoombar.value;
        }

        //GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel")*3f;

        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minzoom, maxzoom);
        //float bgsize = GetComponent<Camera>().orthographicSize / 2.4f;
        //bg.gameObject.transform.localScale = new Vector3(bgsize,bgsize,1);

        Vector3 camerasize1 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        Vector3 camerasize2 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

        transform.localScale = new Vector3(camerasize1.magnitude, camerasize2.magnitude, 1);
    }
}
