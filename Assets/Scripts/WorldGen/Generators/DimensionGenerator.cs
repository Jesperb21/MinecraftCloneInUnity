using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DimensionGenerator : MonoBehaviour {
    public List<GameObject> dimensionsToGenerate;
    private bool alreadyHaveActiveDimension = false;

	// Use this for initialization
	void Start () {
	    foreach(GameObject g in dimensionsToGenerate){
            GameObject Dimension = (GameObject)Instantiate(g, new Vector3(0, 0, 0), Quaternion.identity);
            Dimension.transform.parent = transform;
            if(g.tag=="StartDimension" && !alreadyHaveActiveDimension)
            {
                g.SetActive(true);
                alreadyHaveActiveDimension = true;//only have 1 active dimension at a time, this is required to not have 2 worlds at the same position
            }
            else
            {
                g.SetActive(false);
            }
        }
        
	}
}
