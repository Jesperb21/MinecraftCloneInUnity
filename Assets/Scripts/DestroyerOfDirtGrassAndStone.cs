using UnityEngine;
using System.Collections;

public class DestroyerOfDirtGrassAndStone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.X))
        {
            for(int i = 0; i < transform.childCount; i++){
                Transform chunk = transform.GetChild(i);
                for (int e = 0; e < chunk.transform.childCount; e++)
                {
                    if (chunk.GetChild(e).tag == "FillerBlock")
                    {
                        Destroy(chunk.GetChild(e).gameObject);
                    }
                }
            }
        }
	}
}
