using UnityEngine;
using System.Collections;

public class junkscript : MonoBehaviour {
    public GameObject shotby = null;

    public SpriteRenderer halo;

    public float rockmass;
    public float[] mineralmass = new float[GameController.numminerals];

    public float totalmineralmass
    {
        get
        {
            float tmm = 0;
            foreach (float minmass in mineralmass)
            {
                tmm += minmass;
            }
            return tmm;
        }
    }

    public float mass
    {
        get { return rockmass + totalmineralmass; }
    }

    public int mineralnum = -1;


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


    public void Init(float mass, float[] MineralProbability)
    {
        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < GameController.numminerals; i++ )
        {
            if(Random.value<MineralProbability[i])
            {
                mineralnum = i;
                mineralmass[i] = WeightedRandom(0,mass,3);
                mass -= mineralmass[i];
                break;
            }
        }

        rockmass = mass;

        rb.mass = this.mass;
    }
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(Mathf.Pow(this.mass / density, 1f / 3f), Mathf.Pow(this.mass / density, 1f / 3f), 1);

        rb.mass = this.mass;

        Color halocolor = Color.black;
        for (int i = 0; i < GameController.numminerals; i++)
        {
            halocolor += GameController.instance.MineralColor[i] * mineralmass[i] / this.mass;

        }
        halocolor.a = Mathf.Pow(1 - rockmass / this.mass, 0.3f);
        halo.color = halocolor;



        if (rb.mass < GameController.instance.player.GetComponent<SaucerController>().maxcarriedmass - GameController.instance.player.GetComponent<SaucerController>().carriedmass)
        {

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.sprite.name == "junk")
                {
                    sr.color = Color.green + Color.grey;
                }
            }
        }
        else
        {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.sprite.name == "junk")
                {
                    sr.color = Color.red + Color.grey;
                }
            }
        }

	}
}
