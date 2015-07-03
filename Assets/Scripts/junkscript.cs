using UnityEngine;
using System.Collections;

public class junkscript : MonoBehaviour {
    public GameObject shotby = null;

    public SpriteRenderer halo;

    public float rockmass;
    public float Ir;
    public float Pd;
    public float W;

    public float mass
    {
        get { return rockmass + Ir + Pd + W; }
    }

    float density = 0.1f;

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();

	}

    float WeightedRandom(float min, float max, float power)
    {
        return Mathf.Pow(Random.value, power) * (max - min) + min;
    }


    public void Init(float mass, float IrProb, float PdProb, float WProb)
    {
        rb = GetComponent<Rigidbody2D>();

        Ir=Random.value<IrProb? WeightedRandom(0,mass, 3) : 0;
        mass -= Ir;

        Pd = Random.value < PdProb ? WeightedRandom(0, mass, 3) : 0;
        mass-=Pd;

        W = Random.value < WProb ? WeightedRandom(0, mass, 3) : 0;
        mass-=W;

        rockmass = mass;

        transform.localScale = new Vector3(Mathf.Pow(mass / density, 1f / 3f), Mathf.Pow(mass / density, 1f / 3f), 1);

        rb.mass = this.mass;

        Color halocolor = Color.red * Ir / this.mass + Color.green * Pd / this.mass + Color.blue * W / this.mass;
        halocolor.a = Mathf.Pow(1 - rockmass / this.mass,0.3f);
        halo.color = halocolor;
    }
	
	// Update is called once per frame
	void Update () {
        if (rb.mass < GameController.instance.player.GetComponent<SaucerController>().maxcarriedmass - GameController.instance.player.GetComponent<SaucerController>().carriedmass)
            GetComponentInChildren<SpriteRenderer>().color = Color.green + Color.grey;
        else
            GetComponentInChildren<SpriteRenderer>().color = Color.red + Color.grey;

	}
}
