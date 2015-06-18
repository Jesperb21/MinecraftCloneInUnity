using UnityEngine;
using System.Collections;

public class PerlinNoiseGenerator : MonoBehaviour {
    GameObject world;
    public GameObject block;

	// Use this for initialization
	void Start () {
        for (float x = 0; x < 100; x++)
        {
            for(float z = 0; z < 100; z++){
                float y = Mathf.PerlinNoise(x/10, z/10) *20;
                
                GameObject c = (GameObject)Instantiate(block, new Vector3(x,y, z), Quaternion.identity);
                c.transform.parent = transform;
                float r = Mathf.PerlinNoise(x / 10, z / 10);
                float g = Mathf.PerlinNoise(x / 20, z / 20);
                float b = Mathf.PerlinNoise(x / 30, z / 30);
                c.GetComponent<Renderer>().material.color = new Color(r, g, b);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
    }
}
