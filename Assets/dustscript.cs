using UnityEngine;
using System.Collections;

public class dustscript : MonoBehaviour {
    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(1688,1200,1);//Camera.main.transform.localScale*1.2f;
        ps.emissionRate = transform.localScale.x/10;
        transform.position = Camera.main.transform.position;

        ps.startColor = GameController.instance.currentproperties.fogcolour;
	}
}
