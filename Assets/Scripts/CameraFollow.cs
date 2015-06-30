using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Rigidbody2D following;
    public backgroundmover bg;


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
    float targetsize = 0f;


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

        //set position from speed
        transform.position += new Vector3(linearvel.x,linearvel.y,0) * Time.fixedDeltaTime;
        rotation += angularvel * Time.fixedDeltaTime;

        transform.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,0,1));


	}

    bool moving = false;
    float oldtargetsize;

    void Update()
    {

        if (!moving)
        {
            oldtargetsize = targetsize;
            targetsize = Mathf.Log(following.mass+1);
            if (targetsize != oldtargetsize)
            {
                moving = true;
                movetime = 0.0f;
            }


        }
        else
        {
            movetime += Time.deltaTime;

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(oldtargetsize, targetsize, movetime / zmovetime);

            if (GetComponent<Camera>().orthographicSize == targetsize)
                moving = false;

        }

        bg.size = GetComponent<Camera>().orthographicSize/2.4f;
    }
}
