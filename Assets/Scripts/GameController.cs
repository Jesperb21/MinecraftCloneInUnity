using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject cube;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Instantiate(this, new Vector3(0, 1, 0), Quaternion.identity);

            Instantiate(cube, new Vector3((Random.Range(-5, 5)), (Random.Range(-5, 5)), (Random.Range(-5, 5))), Quaternion.identity);
            
            
            //GameObject[] cubeArray = GameObject.FindGameObjectsWithTag("cube");

            /* foreach (GameObject cube in cubeArray)
             {
                 Transform top = cube.GetComponentInChildren<Transform>().Find("Top");
                 Transform bottom = cube.GetComponentInChildren<Transform>().Find("Bottom");
                 Transform front = cube.GetComponentInChildren<Transform>().Find("Front");
                 Transform back = cube.GetComponentInChildren<Transform>().Find("Back");
                 Transform right = cube.GetComponentInChildren<Transform>().Find("Right");
                 Transform left = cube.GetComponentInChildren<Transform>().Find("Left");
             }*/
        }
	}
}
