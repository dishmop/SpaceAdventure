using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
    public Rigidbody2D player;
    public GameObject arm;
    public GameObject junk;
    public GameObject cam;
    public GameObject ship;
    //public GameObject junkcollector;
    //public ParticleSystem partsys;

    public Slider massslider;
    public Slider healthslider;
    public Slider shotsize;

    List<Rigidbody2D> ships = new List<Rigidbody2D>();
    public List<Rigidbody2D> junks = new List<Rigidbody2D>();


 //   public float carriedmass = 10;

 //   public float maxcarriedmass = 10;


  //  float playermass;

    SaucerController playerSaucerController;

    public Toggle snaptoobjects;

    float worldradius = 1000;

    float WeightedRandom(float min, float max, float power)
    {
        return Mathf.Pow(Random.value, power) * (max - min) + min;
    }

	// Use this for initialization
	void Start () {
       // playermass = player.mass;
    
        //create random ships
        //for(int n=0; n<30; n++)
        //{
        //    float radius = Random.Range(0f, 100f);
        //    float angle = Random.Range(0, 2 * Mathf.PI);
        //    Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
        //    Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

        //    GameObject newship = (GameObject)Instantiate(ship, position, rotation);

        //    ships.Add(newship.GetComponent<Rigidbody2D>());

        //    //newship.GetComponent<ShipController>().force = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        //    newship.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-3f, 3f);
        //    newship.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        //}

        playerSaucerController = player.gameObject.GetComponent<SaucerController>();

        for(int n=0; n<100; n++)
        {
            float radius = Random.Range(0f, worldradius-5);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newjunk = (GameObject)Instantiate(junk, position, rotation);

            junks.Add(newjunk.GetComponent<Rigidbody2D>());

            newjunk.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
            newjunk.GetComponent<Rigidbody2D>().mass = WeightedRandom(0, 400, 4);

            newjunk.transform.localScale = new Vector3(Mathf.Sqrt(newjunk.GetComponent<Rigidbody2D>().mass),Mathf.Sqrt(newjunk.GetComponent<Rigidbody2D>().mass),1);
        }
    }
	
	void Update () {
	    //rotate arm towards mouse
        Vector3 cursor = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        cursor.z = 0;
        Vector3 mouseoffset = cursor - arm.transform.position;

        arm.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(mouseoffset.y, mouseoffset.x), new Vector3(0, 0, 1));

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //shoot when mouse clicked
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerSaucerController.Shoot(shotsize.value * 5);
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            playerSaucerController.Tractor(true, new Vector3(cursor.x, cursor.y));
        }
        else
        {
            playerSaucerController.Tractor(false, new Vector3(cursor.x, cursor.y));
        }

        
        

        //set position for trigger


        //junkcollector.transform.position = player.position;

        //player.mass = playermass + carriedmass;
        //player.gameObject.transform.localScale = new Vector3(Mathf.Sqrt(playermass + carriedmass), Mathf.Sqrt(playermass + carriedmass), 1);
        //junkcollector.transform.localScale = new Vector3(Mathf.Sqrt(playermass + carriedmass), Mathf.Sqrt(playermass + carriedmass), 1);

        massslider.value = playerSaucerController.mass / playerSaucerController.maxMass;
        healthslider.value = playerSaucerController.health;

        transform.position = player.position;
        transform.localScale = player.gameObject.transform.localScale;


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
        //foreach(Rigidbody2D ship in ships)
        //{
        //    Vector2 displacement = ship.position - player.position;
        //    if(displacement.magnitude > 105)
        //    {
        //        ship.position -= 2 * displacement;
        //    }
            
        //    if(displacement.magnitude<20)
        //    {
        //        ship.gameObject.GetComponent<ShipController>().particlesenabled = true;
        //    }
        //    else
        //    {
        //        ship.gameObject.GetComponent<ShipController>().particlesenabled = false;
        //    }
        //}

        foreach(Rigidbody2D junk in junks)
        {
            Vector2 displacement = junk.position - player.position;
            if (displacement.magnitude > worldradius)
            {
                junk.position -= 2 * displacement;
            }
        }

        if(!snaptoobjects.isOn)
        {
            cam.GetComponent<CameraFollow>().following = player;
            cam.GetComponent<CameraFollow>().rotate = false;
        }

        if (playerSaucerController.health <= 0)
            Debug.Break();

	}

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "junk" && snaptoobjects.isOn)
        {
            if (other.gameObject.GetComponent<Rigidbody2D>().mass > cam.GetComponent<CameraFollow>().following.mass)
            {
                cam.GetComponent<CameraFollow>().following = other.gameObject.GetComponent<Rigidbody2D>();
                cam.GetComponent<CameraFollow>().rotate = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "junk")
        {
            cam.GetComponent<CameraFollow>().following = player;
            cam.GetComponent<CameraFollow>().rotate = false;
        }
    }

    //public void Collect(GameObject junk)
    //{
    //    if (carriedmass + junk.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
    //    {
    //        carriedmass += junk.GetComponent<Rigidbody2D>().mass;
    //        junks.Remove(junk.GetComponent<Rigidbody2D>());

    //        player.velocity = (player.velocity * player.mass + junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass)/(playermass+carriedmass);

    //        Destroy(junk);
    //    }
    //    else
    //    {
    //        //health -= junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass/(playermass+carriedmass);
    //    }
    //}
}
