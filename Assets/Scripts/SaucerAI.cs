using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SaucerController))]
public class SaucerAI : MonoBehaviour {

    SaucerController sc;

    bool tracting = false;

	// Use this for initialization
	void Start () {
        sc = GetComponent<SaucerController>();
	}
	
	// Update is called once per frame
	void Update () {
        sc.Tractor(tracting, new Vector3(0,0,0));
        tracting = false;
	}


    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "junk")
        {
            if ((other.gameObject.transform.position - transform.position).magnitude < sc.tractorrange)
            {
                if(other.attachedRigidbody!=null)
                    if(other.GetComponent<junkscript>().shotby != gameObject)
                        if (other.attachedRigidbody.mass < sc.maxcarriedmass - sc.carriedmass)
                        {
                            tracting = true;
                            sc.tractedobj = other.gameObject;
                        }
            }
        }

        if(other.gameObject.tag == "Player")
        {
            sc.Shoot(5, other.gameObject.transform.position);
        }
    }
}
