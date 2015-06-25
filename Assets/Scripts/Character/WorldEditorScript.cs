using UnityEngine;
using System.Collections;

public class WorldEditorScript : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                ChunkGenerator CG = null;
                CG = hit.collider.GetComponent<ChunkGenerator> ();
                Vector3 pos = hit.point;
                if (CG != null)
                {
                    CG.setBlock((int)pos.x, (int)pos.y, (int)pos.z);
                }
            }
        }
	}
}
