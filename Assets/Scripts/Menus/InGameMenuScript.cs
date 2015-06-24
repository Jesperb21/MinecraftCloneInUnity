using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuScript : MonoBehaviour {

    public string mainMenu;
    public string restart;

    public bool isPaused;
    public GameObject pauseMenuCanvas;
    GameObject player;
    

    public void Restart()
    {
        Application.LoadLevel(restart);
    }

    public void Quit()
    {
        Application.LoadLevel(mainMenu);
    }

    public void Resume()
    {
        isPaused = false;
    }
	
	// Update is called once per frame
	void Update () {

        player = GameObject.FindWithTag("Player");

        if (isPaused)
        {
            Cursor.visible = true;
            pauseMenuCanvas.SetActive(true);
            player.transform.GetComponent<MovementController>().enabled = false;
            player.transform.GetComponent<CameraController>().enabled = false;

        }
        else
        {
            pauseMenuCanvas.SetActive(false);
            player.transform.GetComponent<MovementController>().enabled = true;
            player.transform.GetComponent<CameraController>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
	}
}
