using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
    public static GameController instance { get; private set; }

    public int NumRandomAsteroids;
    public int NumRandomEnemies;
    public int NumRandomPickups;

    public float maxAsteroidMass = 100f;
    public float minAsteroidMass =  1f;

    public Rigidbody2D player;
    public GameObject arm;
    public GameObject junk;
    public GameObject cam;
    public GameObject enemy;
    public GameObject healthcrate;

    public bool PeriodicBCs = false;

    public float worldradius = 1000;

    float WeightedRandom(float min, float max, float power)
    {
        return Mathf.Pow(Random.value, power) * (max - min) + min;
    }

    public float maxobjectvelocity = 50;

	// Use this for initialization
	void Start () {
        instance = this;

        for(int n=0; n<NumRandomAsteroids; n++)
        {
            float radius = (worldradius-5)*Mathf.Sqrt(Random.value);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newjunk = (GameObject)Instantiate(junk, position, rotation);

            newjunk.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-5f, 5f);
            newjunk.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2), Random.Range(-maxobjectvelocity, maxobjectvelocity) / Mathf.Sqrt(2));
            newjunk.GetComponent<Rigidbody2D>().mass = WeightedRandom(minAsteroidMass, maxAsteroidMass, 6);
        }

        for (int n = 0; n < NumRandomEnemies; n++)
        {
            float radius = (worldradius - 5) * Mathf.Sqrt(Random.value);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newenemy = (GameObject)Instantiate(enemy, position, rotation);

            newenemy.gameObject.GetComponent<SaucerController>().controller = this;
        }

        for (int n = 0; n < NumRandomPickups; n++)
        {
            float radius = (worldradius - 5) * Mathf.Sqrt(Random.value);
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-180, 180), new Vector3(0, 0, 1));

            GameObject newcrate = (GameObject)Instantiate(healthcrate, position, rotation);

            newcrate.GetComponent<Rigidbody2D>().angularVelocity = Random.value;
        }
    }
	
	void Update () {

	}
}
