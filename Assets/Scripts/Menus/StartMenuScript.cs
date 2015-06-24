using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenuScript : MonoBehaviour {

    public Button startText;
    public Button exitText;

	// Use this for initialization
	void Start () {
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
	}
	
	// Update is called once per frame
    public void StartLevel()
    {
        Application.LoadLevel("GameScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
