using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

    public Vector2 force;
    public float torque;
    //public GameObject partsys;

    Rigidbody2D rb;

    public bool particlesenabled;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.AddRelativeForce(force);

        //float ang = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg -rb.rotation;

        //this is wrong!
        //partsys.transform.rotation = Quaternion.Euler(ang, 90, 0);

        //rb.AddTorque(torque);

        //partsys.SetActive(particlesenabled);
    }
}
