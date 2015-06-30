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
    public ParticleSystem partsys;

    public Slider massslider;
    public Slider healthslider;

    List<Rigidbody2D> ships = new List<Rigidbody2D>();
    List<Rigidbody2D> junks = new List<Rigidbody2D>();


    public float carriedmass = 10;

    public float maxcarriedmass = 10;

    public float tractorrange = 3f;
    public float tractorstregth = 1f;
    public float tractorlength = 1f;

    float playermass;

    Vector3 localtractorpos;
    GameObject tractedobj = null;

    float health = 1.0f;


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

            //newship.GetComponent<ShipController>().force = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            newship.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-3f, 3f);
            newship.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
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
        cursor.z = 0;
        Vector3 mouseoffset = cursor - arm.transform.position;

        arm.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(mouseoffset.y, mouseoffset.x), new Vector3(0, 0, 1));

        //shoot when mouse clicked
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (carriedmass >= junk.GetComponent<Rigidbody2D>().mass)
            {
                carriedmass -= junk.GetComponent<Rigidbody2D>().mass;

                Vector3 offset = new Vector3(junkcollector.GetComponent<CircleCollider2D>().radius+0.2f, 0, 0);
                Vector3 position = arm.transform.position + arm.transform.rotation * offset;
                GameObject newjunk = (GameObject)Instantiate(junk, position, arm.transform.rotation);

                newjunk.GetComponent<Rigidbody2D>().velocity = player.velocity;

                Vector3 impulse = arm.transform.rotation * new Vector3(5, 0, 0);
                newjunk.GetComponent<Rigidbody2D>().AddForce(new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);
                player.AddForce(-new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);

                junks.Add(newjunk.GetComponent<Rigidbody2D>());
            }
        }

        if(tractedobj != null)
        {
            Vector3 position = tractedobj.transform.TransformPoint(localtractorpos);
            Vector3 offset = position - arm.transform.position;

            partsys.gameObject.transform.position = position;
            partsys.gameObject.transform.LookAt(arm.transform);
            partsys.startLifetime = offset.magnitude / partsys.startSpeed;

            Vector2 direction = new Vector2(offset.x, offset.y);
            direction.Normalize();

            float force = tractorstregth * (offset.magnitude - tractorlength);

            partsys.startColor = Color.red * Mathf.Abs(offset.magnitude - tractorlength) / tractorlength + Color.green * (1 - Mathf.Abs(offset.magnitude - tractorlength) / tractorlength);

            tractedobj.GetComponent<Rigidbody2D>().AddForceAtPosition(-force * direction, new Vector2(position.x, position.y));
            player.AddForce(force * direction);


            if (!Input.GetKey(KeyCode.Mouse1)) tractedobj = null;
        }else
        if(Input.GetKey(KeyCode.Mouse1) && mouseoffset.magnitude < tractorrange)
        {
            partsys.gameObject.SetActive(true);
            partsys.gameObject.transform.position = cursor;
            partsys.gameObject.transform.LookAt(arm.transform);
            partsys.startLifetime = mouseoffset.magnitude / partsys.startSpeed;
            partsys.startColor = Color.white;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitinfo = Physics2D.GetRayIntersection(ray);

            if(hitinfo.rigidbody != null)
            {
                localtractorpos = hitinfo.transform.InverseTransformPoint(cursor);
                tractedobj = hitinfo.rigidbody.gameObject;
            }

        }
        else
        {
            partsys.gameObject.SetActive(false);
        }

        //set position for trigger
        transform.position = player.position;

        junkcollector.transform.position = player.position;

        player.mass = playermass + carriedmass;
        player.gameObject.transform.localScale = new Vector3(Mathf.Sqrt(player.mass / playermass)*0.5f, Mathf.Sqrt(player.mass / playermass)*0.5f, 1);
        junkcollector.GetComponent<CircleCollider2D>().radius = Mathf.Sqrt(player.mass / playermass) * 0.4f;

        massslider.value = player.mass / (playermass + maxcarriedmass);
        healthslider.value = health;


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

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "junk")
        {
            if (other.gameObject.GetComponent<Rigidbody2D>().mass > playermass + carriedmass)
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

    public void Collect(GameObject junk)
    {
        if (carriedmass + junk.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
        {
            carriedmass += junk.GetComponent<Rigidbody2D>().mass;
            junks.Remove(junk.GetComponent<Rigidbody2D>());

            player.velocity = (player.velocity * player.mass + junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass)/(playermass+carriedmass);

            Destroy(junk);
        }
        else
        {
            //health -= junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass/(playermass+carriedmass);
        }
    }
}
