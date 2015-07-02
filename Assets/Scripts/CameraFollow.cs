using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

    public Rigidbody2D following;
    public backgroundmover bg;

    public GameObject minimapcube;


    [SerializeField] float linearaccel;

    [SerializeField] float alloweddistance;


    public Vector2 linearvel = new Vector2(0,0);

    float moverate = 1;


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
            GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse Y");

        GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel")*3f;

        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, 1, 500);

        bg.size = GetComponent<Camera>().orthographicSize / 2.4f;

        Vector3 camerasize1 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        Vector3 camerasize2 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

        transform.localScale = new Vector3(camerasize1.magnitude, camerasize2.magnitude, 1);
    }
}
