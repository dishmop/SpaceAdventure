using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

    public Rigidbody2D following;
    public backgroundmover bg;

    public GameObject minimapcube;


    [SerializeField] float linearaccel;
    [SerializeField] float angularaccel;

    [SerializeField]
    float alloweddistance;
    [SerializeField]
    float allowedangle;

    public Vector2 linearvel = new Vector2(0,0);
    float angularvel = 0;

    float rotation = 0;

    float rate = 1;

    float zmovetime = 0.5f;

    float movetime = 0.0f;
    float targetsize = 100f;

    public bool rotate = false;

    float rotatetime = 0.6f;
    float time = 0;

    public Toggle autozoom;


	void FixedUpdate () {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        //gallilean relativity!
        Vector2 rellinearvel = linearvel - following.velocity;
        float relangularvel = angularvel - following.angularVelocity;

        Vector2 lineardisp = following.position - position;
        float angulardisp = following.rotation - rotation;


        //constrain angles to [-180,180]
        while (angulardisp > 180) angulardisp -= 360;
        while (angulardisp < -180) angulardisp += 360;


        if (lineardisp.magnitude > alloweddistance)
        {
            //move towards object
            Vector2 desiredlinvel = lineardisp * rate;

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

        if (Mathf.Abs(angulardisp) > allowedangle)
        {
            //rotate towards
            float desiredangvel = angulardisp * rate;

            if (relangularvel < desiredangvel - angularaccel * Time.fixedDeltaTime / 2) relangularvel += angularaccel * Time.fixedDeltaTime;
            if (relangularvel > desiredangvel + angularaccel * Time.fixedDeltaTime / 2) relangularvel -= angularaccel * Time.fixedDeltaTime;

        }
        else
        {
            if (relangularvel < -angularaccel * Time.fixedDeltaTime / 2) relangularvel += angularaccel * Time.fixedDeltaTime;
            if (relangularvel > angularaccel * Time.fixedDeltaTime / 2) relangularvel -= angularaccel * Time.fixedDeltaTime;
        }

        //calculate absolute values again
        linearvel = rellinearvel + following.velocity;
        angularvel = relangularvel + following.angularVelocity;

        angularvel = Mathf.Clamp(angularvel, -180, 180);

        //set position from speed
        transform.position += new Vector3(linearvel.x,linearvel.y,0) * Time.fixedDeltaTime;

        if (!rotate)
        {
            angularvel = 0;
            if (rotation != 0)
            {
                time += Time.fixedDeltaTime;
                rotation = Mathf.Lerp(rotation, 0, time / rotatetime);
            }
            else
            {
                time = 0;
            }

        }
        else
            rotation += angularvel * Time.fixedDeltaTime;

        while (rotation > 180)  rotation -= 360;
        while (rotation < -180) rotation += 360;

        transform.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,0,1));

        //transform.LookAt(following.transform);

	}

    bool moving = false;
    float oldtargetsize;

    void Update()
    {
        if (autozoom.isOn)
        {
            if (!moving)
            {
                oldtargetsize = targetsize;
                targetsize = Mathf.Sqrt(following.mass)*3;
                if (targetsize != oldtargetsize)
                {
                    moving = true;
                    movetime = 0.0f;
                    //zmovetime = Mathf.Abs((targetsize - oldtargetsize) / 2);
                }


            }
            else
            {
                movetime += Time.deltaTime;

                GetComponent<Camera>().orthographicSize = Mathf.Lerp(oldtargetsize, targetsize, movetime / zmovetime);

                if (GetComponent<Camera>().orthographicSize == targetsize)
                    moving = false;

            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse2))
                GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse Y");

            GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel")*3f;
            moving = false;
        }

        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, 1, 500);

        bg.size = GetComponent<Camera>().orthographicSize / 2.4f;

        Vector3 camerasize1 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        Vector3 camerasize2 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

        transform.localScale = new Vector3(camerasize1.magnitude, camerasize2.magnitude, 1);

    }
}
