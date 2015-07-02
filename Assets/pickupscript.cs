using UnityEngine;
using System.Collections;

public class pickupscript : MonoBehaviour {

    Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.mass < GameController.instance.player.GetComponent<SaucerController>().maxcarriedmass - GameController.instance.player.GetComponent<SaucerController>().carriedmass)
            GetComponentInChildren<MeshRenderer>().material.color = Color.green + Color.grey;
        else
            GetComponentInChildren<MeshRenderer>().material.color = Color.red + Color.grey;

    }
}
