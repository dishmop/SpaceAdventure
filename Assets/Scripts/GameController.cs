using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class areaproperties
{
    public Color fogcolour = Color.white;
    public float[] mineralprobability = {0.3f,0.1f,0.1f,0,0,0,0,0,0};
    public Vector3 localshoppos = new Vector3(800, 850,0);

    public float maxmass = 100f;
    public float weight = 6f;
    public float maxspeed = 40f;
}

public class GameController : MonoBehaviour {
    public static GameController instance { get; private set; }

    public Text cashtext;

    public float numberdensity = 0.0001f;

    public float minAsteroidMass =  1f;

    public Rigidbody2D player;
    public GameObject arm;
    public GameObject junk;
    public GameObject cam;
    public GameObject enemy;
    public GameObject healthcrate;

    public static int numminerals = 9;

    [System.NonSerialized]
    public string[] MineralName = {"Gold", "Iridium", "Osmium", "Palladium", "Platinum", "Rhenium", "Rhodium", "Ruthenium", "Tungsten"};

    [System.NonSerialized]
    public float[] MineralValue = {3,4,5,10,13,17,50,70,100};

    [System.NonSerialized]
    public Color[] MineralColor = {Color.yellow, Color.green, Color.blue, Color.cyan, Color.red, Color.magenta, Color.gray, Color.red * Color.yellow,Color.green * Color.blue  };
 
    [System.NonSerialized]
    public bool[] MineralDiscovered = new bool[numminerals];

    public static int xsize = 5;
    public static int ysize = 5;

    public static float areasize = 1800f;

    public int xcurr;
    public int ycurr;

    public areaproperties[,] props = new areaproperties[xsize,ysize];

    public areaproperties currentproperties
    {
        get { return props[xcurr, ycurr]; }
    }

    public Vector3 areapos
    {
        get { return new Vector3(areasize*Mathf.Floor((Camera.main.gameObject.transform.position.x+areasize/2) / areasize)-areasize/2, areasize*Mathf.Floor((Camera.main.gameObject.transform.position.y+areasize/2) / areasize)-areasize/2,0); }
    }


    public bool PeriodicBCs = false;

    public float worldradius = 1000;

    float WeightedRandom(float min, float max, float power)
    {
        return Mathf.Pow(Random.value, power) * (max - min) + min;
    }


	// Use this for initialization
	void Start () {
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < ysize; j++)
            {
                props[i, j] = new areaproperties();
            }
        }

        props[2, 3].fogcolour = Color.red;
        props[2, 3].maxmass = 500f;

        props[2, 1].fogcolour = Color.yellow;
        props[2, 3].maxspeed = 100f;
        props[2, 3].maxmass = 50f;

        props[1, 2].fogcolour = Color.green;
        props[1, 2].maxmass = 200f;
        props[1, 2].mineralprobability[3] = 0.3f;
        props[1, 2].mineralprobability[4] = 0.2f;
        props[1, 2].mineralprobability[5] = 0.1f;

        props[3, 2].fogcolour = Color.blue;
        props[3, 2].maxmass = 400f;
        props[3, 2].maxspeed = 50f;

        instance = this;

        SpawnRocks();
    }

    public void SpawnRocks()
    {
        int NumRandomAsteroids = (int)(worldradius * worldradius * Mathf.PI * numberdensity);
        for (int n = 0; n < NumRandomAsteroids; n++)
        {
            float radius = worldradius  * Mathf.Sqrt(Random.value);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = player.gameObject.transform.position + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newjunk = (GameObject)Instantiate(junk, position, rotation);

            newjunk.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-currentproperties.maxspeed, currentproperties.maxspeed) / Mathf.Sqrt(2), Random.Range(-currentproperties.maxspeed, currentproperties.maxspeed) / Mathf.Sqrt(2)); newjunk.GetComponent<junkscript>().Init(WeightedRandom(minAsteroidMass, currentproperties.maxmass, currentproperties.weight), currentproperties.mineralprobability);
        }
    }

    int getGridX(float worldX)
    {
        return Mathf.FloorToInt((worldX + areasize / 2) / areasize + xsize / 2) % xsize;
    }

    int getGridY(float worldY)
    {
        return Mathf.FloorToInt((worldY + areasize / 2) / areasize + ysize / 2) % ysize;
    }
	
	void Update () {
        if (cashtext != null)
        {
            cashtext.text = "Cash: $" + SaucerPlayer.instance.Cash;
        }

        xcurr = getGridX(Camera.main.gameObject.transform.position.x);
        ycurr = getGridY(Camera.main.gameObject.transform.position.y);

        // spawn rocks to fill empty space we've moved into
        int numNewRocks = (int)(Mathf.PI * worldradius * player.velocity.magnitude * Time.deltaTime * numberdensity);

        float probabilityofextra = Mathf.PI * worldradius * player.velocity.magnitude * Time.deltaTime * numberdensity - numNewRocks;

        if (Random.value < probabilityofextra)
            numNewRocks++;

        for(int i=0; i<numNewRocks; i++)
        {
            float radius = worldradius;

            // random weighted angle between -pi/2 and pi/2
            float randomangle = Mathf.Asin(Random.value) * (Random.value > 0.5 ? -1 : 1);

            float angle = randomangle + Mathf.Atan2(player.velocity.y, player.velocity.x);
            
            Vector3 position = player.gameObject.transform.position + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            int x = getGridX(position.x);
            int y = getGridY(position.y);

            areaproperties localprops = props[x,y];

            GameObject newjunk = (GameObject)Instantiate(junk, position, rotation);

            newjunk.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-currentproperties.maxspeed, currentproperties.maxspeed) / Mathf.Sqrt(2), Random.Range(-currentproperties.maxspeed, currentproperties.maxspeed) / Mathf.Sqrt(2));
            newjunk.GetComponent<junkscript>().Init(WeightedRandom(minAsteroidMass, localprops.maxmass, localprops.weight), localprops.mineralprobability);
        }
    }
}
