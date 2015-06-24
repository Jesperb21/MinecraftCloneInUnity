using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryListScript : MonoBehaviour {

    //her tilføjer vi tingende
    public int slotsX, slotsY;
    public List<ItemScript> inventory = new List<ItemScript>();
    public List<ItemScript> slots = new List<ItemScript>();
    private ItemDatabaseScript database;
    public bool showinventory;
    public GUISkin skin;


	// Use this for initialization
	void Start () {
        for (int i = 0; i < (slotsX *slotsY); i++)
        {
            slots.Add(new ItemScript());
            inventory.Add(new ItemScript());
        }

        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabaseScript>();
        inventory[0] = database.items[0];
        //inventory.Add(database.items[0]);
        //inventory.Add(database.items[1]);

	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            showinventory = !showinventory;
        }
    }

    void OnGUI()
    {
        if (showinventory)
        {
            DrawInventory();
        }
       /* for (int i = 0; i < inventory.Count; i++)
        {
            GUI.Label(new Rect(10, i*20, 200, 50),inventory[i].itemName);
        }*/
    }

    void DrawInventory()
    {
        int i = 0;
        for (int y = 0; y < slotsY; y++)
        {
            for (int x = 0; x < slotsX; x++)
            {
                Rect slotRect = new Rect(x*60, y * 60,50,50);
                GUI.Box(new Rect(slotRect), "", skin.GetStyle("Slots"));
                slots[i] = inventory[i];
                if (slots[i].itemName != null)
                {
                    GUI.DrawTexture(slotRect, slots[i].itemIcon);
                }

                i++;
            }
        }
    }
}
