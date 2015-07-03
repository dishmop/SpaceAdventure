using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {
    Rigidbody2D rb;

    static float sqrt2;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();

        sqrt2 = Mathf.Sqrt(2);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 displacement = rb.position - GameController.instance.player.position;
        if (displacement.magnitude > GameController.instance.worldradius)
        {
            if (GameController.instance.PeriodicBCs)
            {
                rb.position -= 2 * GameController.instance.worldradius * displacement.normalized;

                float speed = Mathf.Clamp(rb.velocity.magnitude, 0, GameController.instance.maxobjectvelocity);

                rb.velocity = new Vector2(Random.Range(-speed, speed) / sqrt2, Random.Range(-speed, speed) / sqrt2);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
