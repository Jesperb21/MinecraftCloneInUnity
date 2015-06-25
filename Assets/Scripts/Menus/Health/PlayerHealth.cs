using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int maxHealth = 100;
    public int currentHealth = 100;

    public float healthbarLenght;

	// Use this for initialization
	void Start () {

        healthbarLenght = Screen.width / 2;
	}
	
	// Update is called once per frame
	void Update () {
        AddjustCurrentHealth(0);
	}

    void OnGUI()
    {
        GUI.Box(new Rect(10,10,healthbarLenght,20), currentHealth + "/" + maxHealth);
    }

    public void AddjustCurrentHealth(int abj)
    {
        currentHealth += abj;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (maxHealth < 1)
        {
            maxHealth = 1; 
        }
        healthbarLenght = (Screen.width / 2) * (currentHealth / (float)maxHealth);
    }
    
}
