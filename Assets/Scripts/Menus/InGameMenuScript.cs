using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuScript : MonoBehaviour {

    //public variables
    //here we set the scene names in the inspector insted og hardcoding it.
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
	
	void Update () {

        player = GameObject.FindWithTag("Player");

        if (isPaused)
        {
            //We want to be able to se and move our mouse in the menu.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //show ingamemenu
            pauseMenuCanvas.SetActive(true);
            //disable Player scripts for better menu interaction
            player.transform.GetComponent<MovementController>().enabled = false;
            player.transform.GetComponent<CameraController>().enabled = false;

        }
        else
        {
            //diable the ingamemenu
            pauseMenuCanvas.SetActive(false);
            //inable Player scripts
            player.transform.GetComponent<MovementController>().enabled = true;
            player.transform.GetComponent<CameraController>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //toggle paused and not paused 
            isPaused = !isPaused;
        }
	}
}
