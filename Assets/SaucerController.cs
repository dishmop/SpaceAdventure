using UnityEngine;
using System.Collections;

public class SaucerController : MonoBehaviour {
    public float saucermass;
    public float carriedmass = 20;
    public float maxcarriedmass = 50;

    public GameController controller;
    public GameObject junk;
    public GameObject arm;

    public ParticleSystem partsys;

    Rigidbody2D rb;

    public float tractorrange = 3f;
    public float tractorstregth = 1f;
    public float tractorlength = 1f;

    Vector3 localtractorpos;
    GameObject tractedobj = null;

    public float health = 1.0f;

    float shotspeed = 100;

    public float maxMass
    {
        get { return saucermass + maxcarriedmass; }
    }

    public float mass
    {
        get { return saucermass + carriedmass; }
    }


	// Use this for initialization
	void Start () {
        saucermass = GetComponent<Rigidbody2D>().mass;

        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.mass = mass;
        transform.localScale = new Vector3(Mathf.Sqrt(mass), Mathf.Sqrt(mass), 1);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "junk")
        {
            Collect(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Vector2 relvel = rb.velocity - coll.collider.attachedRigidbody.velocity;
        Vector2 impulse = relvel * coll.collider.attachedRigidbody.mass;

        health -= impulse.magnitude/10000;
    }

    public void Collect(GameObject junk)
    {
        if (carriedmass + junk.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
        {
            carriedmass += junk.GetComponent<Rigidbody2D>().mass;
            controller.junks.Remove(junk.GetComponent<Rigidbody2D>());

            rb.velocity = (rb.velocity * rb.mass + junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass) / (saucermass + carriedmass);

            Destroy(junk);
        }
        else
        {
            //health -= junk.GetComponent<Rigidbody2D>().velocity * junk.GetComponent<Rigidbody2D>().mass/(playermass+carriedmass);
        }
    }

    public void Shoot(float shotmass)
    {
        if (carriedmass >= shotmass)
        {
            carriedmass -= shotmass;

            Vector3 offset = new Vector3(transform.localScale.x * 1.8f + 2*Mathf.Sqrt(shotmass), 0, 0);
            Vector3 position = arm.transform.position + arm.transform.rotation * offset;
            GameObject newjunk = (GameObject)Instantiate(junk, position, arm.transform.rotation);

            newjunk.GetComponent<Rigidbody2D>().velocity = rb.velocity;

            newjunk.GetComponent<Rigidbody2D>().mass = shotmass;

            Vector3 impulse = arm.transform.rotation * new Vector3(shotspeed, 0, 0) * shotmass;
            newjunk.GetComponent<Rigidbody2D>().AddForce(new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);
            rb.AddForce(-new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);

            controller.junks.Add(newjunk.GetComponent<Rigidbody2D>());

            newjunk.transform.localScale = new Vector3(Mathf.Sqrt(shotmass), Mathf.Sqrt(shotmass), 1);
        }
    }
    
    public void Tractor(bool on, Vector3 location)
    {
        Vector3 mouseoffset = location - arm.transform.position;
        if (on)
        {
            if (tractedobj != null)
            {
                Vector3 position = tractedobj.transform.TransformPoint(localtractorpos);
                Vector3 offset = position - arm.transform.position;

                position.z += 10;
                partsys.gameObject.transform.position = position;
                partsys.gameObject.transform.LookAt(arm.transform);
                partsys.startLifetime = offset.magnitude / partsys.startSpeed;

                Vector2 direction = new Vector2(offset.x, offset.y);
                direction.Normalize();

                float force = tractorstregth * (offset.magnitude - tractorlength);

                partsys.startColor = Color.red * Mathf.Abs(offset.magnitude - tractorlength) / tractorlength + Color.green * (1 - Mathf.Abs(offset.magnitude - tractorlength) / tractorlength);

                tractedobj.GetComponent<Rigidbody2D>().AddForceAtPosition(-force * direction, new Vector2(position.x, position.y));
                rb.AddForce(force * direction);
            }
            else
                if (mouseoffset.magnitude < tractorrange)
                {
                    partsys.gameObject.SetActive(true);

                    Vector3 pos = location;
                    pos.z += 10;

                    partsys.gameObject.transform.position = pos;

                    partsys.gameObject.transform.LookAt(arm.transform);
                    partsys.startLifetime = mouseoffset.magnitude / partsys.startSpeed;
                    partsys.startColor = Color.white;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hitinfo = Physics2D.GetRayIntersection(ray);

                    if (hitinfo.rigidbody != null)
                    {
                        localtractorpos = hitinfo.transform.InverseTransformPoint(location);
                        tractedobj = hitinfo.rigidbody.gameObject;
                    }

                }
                else
                {
                    tractedobj = null;
                    partsys.gameObject.SetActive(false);
                }
        }else
        {
            tractedobj = null;
            partsys.gameObject.SetActive(false);
        }
    }
}
