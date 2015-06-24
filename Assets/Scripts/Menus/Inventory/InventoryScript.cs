using UnityEngine;
using System.Collections;

public class InventoryScript : MonoBehaviour {

    public GameObject inventory;
    public bool isPaused;

    GameObject player;


	// Use this for initialization
	void Start () {
        inventory.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {


        player = GameObject.FindWithTag("Player");

        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inventory.SetActive(true);
            player.transform.GetComponent<MovementController>().enabled = false;
            player.transform.GetComponent<CameraController>().enabled = false;

        }
        else
        {
            inventory.SetActive(false);
            player.transform.GetComponent<MovementController>().enabled = true;
            player.transform.GetComponent<CameraController>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isPaused = !isPaused;
        }
	}
}
