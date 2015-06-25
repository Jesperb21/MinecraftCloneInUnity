using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenuScript : MonoBehaviour {

    //Get public variables
    public Button startText;
    public Button exitText;


	void Start () {
        //Get the Button component of the Text elements
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
	}
	

    public void StartLevel()
    {
        //Load the GameScene
        Application.LoadLevel("GameScene");
    }
    public void ExitGame()
    {
        //Quit game, only workes ind a Build Game
        Application.Quit();
    }
}
