using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameController : MonoBehaviour {
    public Rigidbody2D player;
    public GameObject arm;
    public GameObject junk;
    public GameObject cam;
    public GameObject ship;
    public GameObject junkcollector;

    public Slider slider;

    List<Rigidbody2D> ships = new List<Rigidbody2D>();
    List<Rigidbody2D> junks = new List<Rigidbody2D>();


    public float carriedmass = 10;

    public float maxcarriedmass = 10;

    float playermass;


	// Use this for initialization
	void Start () {
        playermass = player.mass;
    
        //create random ships
        for(int n=0; n<30; n++)
        {
            float radius = Random.Range(0f, 100f);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newship = (GameObject)Instantiate(ship, position, rotation);

            ships.Add(newship.GetComponent<Rigidbody2D>());

            newship.GetComponent<ShipController>().force = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            newship.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newship.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-0, 0), Random.Range(-0, 0));
        }

        for(int n=0; n<100; n++)
        {
            float radius = Random.Range(0f, 100f);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newjunk = (GameObject)Instantiate(junk, position, rotation);

            junks.Add(newjunk.GetComponent<Rigidbody2D>());

            newjunk.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));

        }
    }
	
	void Update () {
	    //rotate arm towards mouse
        Vector3 cursor = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        Vector3 mouseoffset = cursor - arm.transform.position;

        arm.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(mouseoffset.y, mouseoffset.x), new Vector3(0, 0, 1));

        //shoot when mouse clicked
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (carriedmass >= junk.GetComponent<Rigidbody2D>().mass)
            {
                carriedmass -= junk.GetComponent<Rigidbody2D>().mass;

                Vector3 offset = new Vector3(0.8f, 0, 0);
                Vector3 position = arm.transform.position + arm.transform.rotation * offset;
                GameObject newjunk = (GameObject)Instantiate(junk, position, arm.transform.rotation);

                newjunk.GetComponent<Rigidbody2D>().velocity = player.velocity;

                Vector3 impulse = arm.transform.rotation * new Vector3(2, 0, 0);
                newjunk.GetComponent<Rigidbody2D>().AddForce(new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);
                player.AddForce(-new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);

                junks.Add(newjunk.GetComponent<Rigidbody2D>());
            }
        }

        //set position for trigger
        transform.position = player.position;

        junkcollector.transform.position = player.position;

        player.mass = playermass + carriedmass;

        slider.value = player.mass / (playermass + maxcarriedmass);


        //stabilize if space is pressed
        if(Input.GetKey(KeyCode.Space))
        {
            player.angularDrag = 5;
        }
        else
        {
            player.angularDrag = 0;
        }

        //periodic BCs for junk
        foreach(Rigidbody2D ship in ships)
        {
            Vector2 displacement = ship.position - player.position;
            if(displacement.magnitude > 105)
            {
                ship.position -= 2 * displacement;
            }
            
            if(displacement.magnitude<20)
            {
                ship.gameObject.GetComponent<ShipController>().particlesenabled = true;
            }
            else
            {
                ship.gameObject.GetComponent<ShipController>().particlesenabled = false;
            }
        }

        foreach(Rigidbody2D junk in junks)
        {
            Vector2 displacement = junk.position - player.position;
            if (displacement.magnitude > 105)
            {
                junk.position -= 2 * displacement;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "ship")
        {
            cam.GetComponent<CameraFollow>().following = other.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "ship")
        {
            cam.GetComponent<CameraFollow>().following = player;
        }
    }

    public void Collect(GameObject junk)
    {
        if (carriedmass + junk.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
        {
            carriedmass += junk.GetComponent<Rigidbody2D>().mass;
            junks.Remove(junk.GetComponent<Rigidbody2D>());

            player.velocity = (player.velocity * player.mass + junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass)/(playermass+carriedmass);

            Destroy(junk);
        }
    }
}
