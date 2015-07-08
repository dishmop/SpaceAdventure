using UnityEngine;
using System.Collections;

public class SaucerController : MonoBehaviour {
    public float saucermass;

    public float rockmass;

    public float[] mineralmass = new float[GameController.numminerals];

    public float totalmineralmass
    {
        get 
        {
            float tmm = 0;
            foreach(float minmass in mineralmass)
            {
                tmm += minmass;
            }
            return tmm;
        }
    }

    public float carriedmass
    {
    get {return rockmass + totalmineralmass;}
    }

    public float maxcarriedmass = 50;

    public GameController controller;
    public GameObject junk;
    public GameObject arm;
    public GameObject shieldobj;

    public AudioSource burningsound;
    public AudioSource pickupsound;
    public AudioSource shootsound;

    public ParticleSystem smokesys;

    public GameObject beam;

    Rigidbody2D rb;

    public float tractorrange = 3f;
    public float tractorstregth = 1f;
    public float tractorlength = 1f;

    Vector3 localtractorpos;
    public GameObject tractedobj = null;

    public float health = 1.0f;

    public float shield;
    public float maxshield = 1000;
    float shieldregen = 0.1f;

    float shotspeed = 100;

    public bool dead = false;

    float timesinceshot = 100;
    float reloadtimepermass = 0.1f;

    public float maxMass
    {
        get { return saucermass + maxcarriedmass; }
    }

    public float mass
    {
        get { return saucermass + carriedmass; }
    }

    public bool tractorstabilizer = false;


	// Use this for initialization
	void Start () {
        saucermass = GetComponent<Rigidbody2D>().mass;

        shield = maxshield;

        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (dead)
        {
            beam.SetActive(false);
            GetComponentInChildren<MeshRenderer>().material.color = Color.black;
        } else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }

        rb.mass = mass;
        transform.localScale = new Vector3(Mathf.Sqrt(saucermass), Mathf.Sqrt(saucermass), Mathf.Sqrt(saucermass));

        if(!dead)
            shield += maxshield* shieldregen * Time.deltaTime;

        shield = Mathf.Clamp(shield, 0, maxshield);

        shieldobj.transform.localScale = new Vector3(Mathf.Sqrt(shield / maxshield), Mathf.Sqrt(shield / maxshield), 1);

        timesinceshot += Time.deltaTime;

        smokesys.emissionRate = Mathf.Clamp(100 * (1 - health),0,100);
        burningsound.volume = Mathf.Clamp((1 - health), 0, 1);

        health = Mathf.Clamp(health, 0, 1);

        if (health <= 0)
        {
            dead = true;
        }
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        //cheeky use of short-circuiting
        if (coll.collider.gameObject == tractedobj && Collect(coll.collider.gameObject))
        {
        }
        else
        {
            Vector2 relvel = -coll.relativeVelocity;
            float impulsemag = Vector2.Dot(relvel, coll.contacts[0].normal) * coll.collider.attachedRigidbody.mass;

            shield -= impulsemag;

            if (shield < 0)
                health += shield / 10000;
        }
    }

    public bool Collect(GameObject item)
    {
        if (item.tag == "junk")
        {
            if (!dead && carriedmass + item.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
            {
                if (item.GetComponent<Rigidbody2D>() == null) return false;

                rockmass += item.GetComponent<junkscript>().rockmass;

                for(int i=0; i<GameController.numminerals; i++)
                {
                    mineralmass[i] += item.GetComponent<junkscript>().mineralmass[i];
                }

                //controller.junks.Remove(item.GetComponent<Rigidbody2D>());

                rb.velocity = (rb.velocity * rb.mass + item.GetComponent<Rigidbody2D>().velocity * item.GetComponent<Rigidbody2D>().mass) / (saucermass + carriedmass);

                Destroy(item);

                pickupsound.Play();


                if (item.GetComponent<junkscript>().mineralnum >= 0)
                {
                    if (!GameController.instance.MineralDiscovered[item.GetComponent<junkscript>().mineralnum] && MainGame.instance != null)
                    {
                        MainGame.instance.DiscoverMineral(item.GetComponent<junkscript>().mineralnum);
                    }
                }

                return true;
            }
        }
        else if(item.tag == "pickup")
        {
            if (!dead && carriedmass + item.GetComponent<Rigidbody2D>().mass <= maxcarriedmass)
            {
                rockmass += item.GetComponent<Rigidbody2D>().mass;
                //controller.pickups.Remove(item.GetComponent<Rigidbody2D>());

                rb.velocity = (rb.velocity * rb.mass + item.GetComponent<Rigidbody2D>().velocity * item.GetComponent<Rigidbody2D>().mass) / (saucermass + carriedmass);

                Destroy(item);

                health += 0.5f;
                return true;
            }
        }
        return false;
    }

    public void Shoot(float shotmass, Vector3 shootat)
    {
        if (!dead && rockmass >= shotmass && timesinceshot > reloadtimepermass * shotmass)
        {
            shootsound.Play();

            Vector3 offset = shootat - arm.transform.position;
            arm.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(offset.y, offset.x), new Vector3(0, 0, 1));

            rockmass -= shotmass;

            Vector3 shotoffset = new Vector3(transform.localScale.x * 1.8f + 2*Mathf.Sqrt(shotmass), 0, 0);
            Vector3 position = arm.transform.position + arm.transform.rotation * shotoffset;
            GameObject newjunk = (GameObject)Instantiate(junk, position, arm.transform.rotation);

            newjunk.GetComponent<Rigidbody2D>().velocity = rb.velocity;

            float[] minprob = new float[GameController.numminerals];
            newjunk.GetComponent<junkscript>().Init(shotmass, minprob);

            Vector3 impulse = arm.transform.rotation * new Vector3(shotspeed, 0, 0) * shotmass;
            newjunk.GetComponent<Rigidbody2D>().AddForce(new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);
            rb.AddForce(-new Vector2(impulse.x, impulse.y), ForceMode2D.Impulse);

            newjunk.GetComponent<junkscript>().shotby = gameObject;

            timesinceshot = 0;
        }
    }
    
    public void Tractor(bool on, Vector3 location)
    {
        if (dead) return;
        Vector3 mouseoffset = location - arm.transform.position;
        if (on)
        {
            if (tractedobj != null)
            {
                if (!tractedobj.activeSelf)
                {
                    tractedobj = null;
                    return;
                }
                Vector3 position = tractedobj.transform.TransformPoint(localtractorpos);
                Vector3 offset = position - arm.transform.position;

                if(offset.magnitude > tractorrange)
                {
                    tractedobj = null;
                    return;
                }

                SetBeam(arm.transform.position, position);

                position.z += 10;

                beam.SetActive(true);

                Vector2 direction = new Vector2(offset.x, offset.y);
                direction.Normalize();

                float force = tractorstregth*100;// *(offset.magnitude - tractorlength);
                beam.GetComponent<SpriteRenderer>().color = Color.red;

                tractedobj.GetComponent<Rigidbody2D>().AddForceAtPosition(-force * direction, new Vector2(position.x, position.y));
                rb.AddForce(force * direction);

                // do force to counter orbiting
                if (tractorstabilizer)
                {
                    Vector2 relvel = tractedobj.GetComponent<Rigidbody2D>().velocity - rb.velocity;
                    relvel.Normalize();
                    tractedobj.GetComponent<Rigidbody2D>().AddForce(-relvel * force / 2);

                    rb.AddForce(relvel * force / 2);
                }
            }
            else
                if (mouseoffset.magnitude < tractorrange)
                {
                    beam.SetActive(true);

                    SetBeam(arm.transform.position,location);

                    Vector3 pos = location;
                    pos.z += 10;


                    beam.GetComponent<SpriteRenderer>().color = Color.white;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hitinfo = Physics2D.GetRayIntersection(ray);

                    if (hitinfo.rigidbody != null && hitinfo.rigidbody != rb && !hitinfo.collider.isTrigger)
                    {
                        localtractorpos = hitinfo.transform.InverseTransformPoint(location);
                        tractedobj = hitinfo.rigidbody.gameObject;
                    }

                }
                else
                {
                    tractedobj = null;
                    //partsys.gameObject.SetActive(false);
                    beam.SetActive(false);
                }
        }else
        {
            tractedobj = null;
            //partsys.gameObject.SetActive(false);
            beam.SetActive(false);
        }
    }

    void SetBeam(Vector3 from, Vector3 to)
    {
        beam.transform.position = (from + to) / 2;
        Quaternion rotation = Quaternion.LookRotation
            (to - transform.position, beam.transform.TransformDirection(Vector3.up));
        beam.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        float distance = (to - from).magnitude;
        beam.transform.localScale = new Vector3(distance/(5*transform.localScale.x),1,1);
    }
}
