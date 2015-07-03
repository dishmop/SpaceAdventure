using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SaucerController))]
public class SaucerPlayer : MonoBehaviour {
    public static SaucerPlayer instance { get; private set; }

    public SaucerController sc;

    Rigidbody2D rb;

    public Slider massslider;
    public Slider healthslider;
    public Slider shotsize;

    public float maxshotmass = 10f;

	// Use this for initialization
	void Start () {
        instance = this;
        sc = GetComponent<SaucerController>();
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        //rotate arm towards mouse
        Vector3 cursor = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        cursor.z = 0;
        //Vector3 mouseoffset = cursor - arm.transform.position;


        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //shoot when mouse clicked
            if (Input.GetKey(KeyCode.Mouse0))
            {
                float shotmass = shotsize.value * (maxshotmass - GameController.instance.minAsteroidMass) + GameController.instance.minAsteroidMass;
                sc.Shoot(shotmass, cursor);
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            sc.Tractor(true, new Vector3(cursor.x, cursor.y));
        }
        else
        {
            sc.Tractor(false, new Vector3(cursor.x, cursor.y));
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.angularDrag = 5;
        }
        else
        {
            rb.angularDrag = 0;
        }

        massslider.value = sc.carriedmass / sc.maxcarriedmass;
        healthslider.value = sc.health;

	}

    public void Respawn()
    {
        sc.health = 1;
        sc.dead = false;
        sc.shield = sc.maxshield;
        rb.velocity = new Vector2();
        CameraFollow.instance.linearvel = new Vector2(0, 0);
    }
}
