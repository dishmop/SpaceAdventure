using UnityEngine;
using System.Collections;

public class SaucerController : MonoBehaviour {
    public float saucermass;

    public float rockmass;

    public float Ir;
    public float Pd;
    public float W;

    public float carriedmass
    {
    get {return rockmass + Ir + Pd + W;}
    }
    public float maxcarriedmass = 50;

    public GameController controller;
    public GameObject junk;
    public GameObject arm;
    public GameObject shieldobj;

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
        transform.localScale = new Vector3(Mathf.Sqrt(saucermass), Mathf.Sqrt(saucermass), 1);

        if(!dead)
            shield += maxshield* shieldregen * Time.deltaTime;

        shield = Mathf.Clamp(shield, 0, maxshield);

        shieldobj.transform.localScale = new Vector3(Mathf.Sqrt(shield / maxshield), Mathf.Sqrt(shield / maxshield), 1);

        timesinceshot += Time.deltaTime;

        smokesys.emissionRate = Mathf.Clamp(100 * (1 - health),0,100);

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
                Ir += item.GetComponent<junkscript>().Ir;
                Pd += item.GetComponent<junkscript>().Pd;
                W += item.GetComponent<junkscript>().W;

                //controller.junks.Remove(item.GetComponent<Rigidbody2D>());

                rb.velocity = (rb.velocity * rb.mass + item.GetComponent<Rigidbody2D>().velocity * item.GetComponent<Rigidbody2D>().mass) / (saucermass + carriedmass);

                Destroy(item);

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
            Vector3 offset = shootat - arm.transform.position;
            arm.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(offset.y, offset.x), new Vector3(0, 0, 1));

            rockmass -= shotmass;

            Vector3 shotoffset = new Vector3(transform.localScale.x * 1.8f + 2*Mathf.Sqrt(shotmass), 0, 0);
            Vector3 position = arm.transform.position + arm.transform.rotation * shotoffset;
            GameObject newjunk = (GameObject)Instantiate(junk, position, arm.transform.rotation);

            newjunk.GetComponent<Rigidbody2D>().velocity = rb.velocity;

            newjunk.GetComponent<junkscript>().Init(shotmass, 0, 0, 0);

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

                //partsys.gameObject.SetActive(true);
                beam.SetActive(true);


                //partsys.gameObject.transform.position = position;
                //partsys.gameObject.transform.LookAt(arm.transform);
                //partsys.startLifetime = offset.magnitude / partsys.startSpeed;

                Vector2 direction = new Vector2(offset.x, offset.y);
                direction.Normalize();

                float force = tractorstregth*100;// *(offset.magnitude - tractorlength);
                beam.GetComponent<SpriteRenderer>().color = Color.red;
                //partsys.startColor = Color.red * Mathf.Abs(offset.magnitude - tractorlength) / tractorlength + Color.green * (1 - Mathf.Abs(offset.magnitude - tractorlength) / tractorlength);

                tractedobj.GetComponent<Rigidbody2D>().AddForceAtPosition(-force * direction, new Vector2(position.x, position.y));
                rb.AddForce(force * direction);
            }
            else
                if (mouseoffset.magnitude < tractorrange)
                {
                    //partsys.gameObject.SetActive(true);
                    beam.SetActive(true);

                    SetBeam(arm.transform.position,location);

                    Vector3 pos = location;
                    pos.z += 10;

                    //partsys.gameObject.transform.position = pos;

                    //partsys.gameObject.transform.LookAt(arm.transform);
                    //partsys.startLifetime = mouseoffset.magnitude / partsys.startSpeed;
                    //partsys.startColor = Color.white;

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
