using UnityEngine;
using System.Collections;

public class junkscript : MonoBehaviour {
    public GameObject shotby = null;

    float density = 0.1f;

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(Mathf.Pow(rb.mass/density,1f/3f), Mathf.Pow(rb.mass/density,1f/3f), 1);


        if (rb.mass < GameController.instance.player.GetComponent<SaucerController>().maxcarriedmass - GameController.instance.player.GetComponent<SaucerController>().carriedmass)
            GetComponentInChildren<SpriteRenderer>().color = Color.green + Color.grey;
        else
            GetComponentInChildren<SpriteRenderer>().color = Color.red + Color.grey;

	}
}
