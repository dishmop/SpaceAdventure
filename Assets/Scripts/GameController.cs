using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class areaproperties
{
    public Color fogcolour = Color.white;
    public float[] mineralprobability = {0.3f,0.1f,0.1f,0.05f,0.3f,0.4f,0.3f,0.2f,0.1f};
    public Vector3 localshoppos = new Vector3(800, 850,0);

    public float maxmass = 100f;
    public float weight = 6f;
}

public class GameController : MonoBehaviour {
    public static GameController instance { get; private set; }

    public Text cashtext;
    public Text Wtext;
    public Text Pdtext;
    public Text Irtext;

    public float numberdensity = 0.0001f;


    //public int NumRandomAsteroids;
    //public int NumRandomEnemies;
    //public int NumRandomPickups;

    public float maxAsteroidMass = 100f;
    public float minAsteroidMass =  1f;

    public Rigidbody2D player;
    public GameObject arm;
    public GameObject junk;
    public GameObject cam;
    public GameObject enemy;
    public GameObject healthcrate;

    public static int numminerals = 9; 

    public string[] MineralName = {"Gold", "Iridium", "Osmium", "Palladium", "Platinum", "Rhenium", "Rhodium", "Ruthenium", "Tungsten"};
    public float[] MineralValue = {10,50,70,200,20,10,20,35,70};
    public Color[] MineralColor = {Color.yellow, Color.green, Color.blue, Color.cyan, Color.red, Color.magenta, Color.gray, Color.red * Color.yellow,Color.green * Color.blue  };
    public bool[] MineralDiscovered = new bool[numminerals];

    static int xsize = 5;
    static int ysize = 5;

    static float areasize = 1800f;

    public int xcurr;
    public int ycurr;

    areaproperties[,] props = new areaproperties[xsize,ysize];

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

    public float maxobjectvelocity = 50;

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

        instance = this;

        //SpawnEnemies();
        //SpawnPickups();
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
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2), Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2));
            newjunk.GetComponent<junkscript>().Init(WeightedRandom(minAsteroidMass, currentproperties.maxmass, currentproperties.weight),currentproperties.mineralprobability);
        }
    }

    //public void SpawnEnemies()
    //{
    //    for (int n = 0; n < NumRandomEnemies; n++)
    //    {
    //        float radius = (worldradius - 5) * Mathf.Sqrt(Random.value);
    //        float angle = Random.Range(0, 2 * Mathf.PI);
    //        Vector3 position = player.gameObject.transform.position + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    //        Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

    //        GameObject newenemy = (GameObject)Instantiate(enemy, position, rotation);

    //        newenemy.gameObject.GetComponent<SaucerController>().controller = this;
    //    }

    //}

    //public void SpawnPickups()
    //{
    //    for (int n = 0; n < NumRandomPickups; n++)
    //    {
    //        float radius = (worldradius - 5) * Mathf.Sqrt(Random.value);
    //        float angle = Random.Range(0, 2 * Mathf.PI);
    //        Vector3 position = player.gameObject.transform.position + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    //        Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

    //        GameObject newcrate = (GameObject)Instantiate(healthcrate, position, rotation);

    //        newcrate.GetComponent<Rigidbody2D>().angularVelocity = Random.value;
    //    }
    //}

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
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2), Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2));
            newjunk.GetComponent<junkscript>().Init(WeightedRandom(minAsteroidMass, localprops.maxmass, localprops.weight), localprops.mineralprobability);
        }
    }
}
