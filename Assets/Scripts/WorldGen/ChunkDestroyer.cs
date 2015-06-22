using UnityEngine;
using System.Collections;

public class ChunkDestroyer : MonoBehaviour {
    public float timeframe = 10f;

    void OnEnable()
    {
        //prevent it from destroying if it was enabled within the timeframe allowed
        CancelInvoke();
    }

    void OnDisable()
    {
        //destroy after the timeframe allowed unless enabled again
        Invoke("Destroy", timeframe);
    }


    void Destroy()
    {
        CancelInvoke(); //safe guard to make sure it only destroys once
        Destroy(gameObject);
    }
}
