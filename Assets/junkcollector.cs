using UnityEngine;
using System.Collections;

public class junkcollector : MonoBehaviour {

    public GameController controller;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="junk")
        {
            controller.Collect(other.gameObject);
        }
    }
}
