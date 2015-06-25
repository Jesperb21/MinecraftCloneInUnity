using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryListScript : MonoBehaviour {


    public int slotsX, slotsY;
    public GUISkin skin;
    public List<ItemScript> inventory = new List<ItemScript>();
    public List<ItemScript> slots = new List<ItemScript>();
    public bool showinventory;
    private ItemDatabaseScript database;
    private bool showToolTip;
    private string toolTip;

    //drag items
    private bool draggingItem;
    private ItemScript draggedItem;
    private int draggingIndex;

    //player handling
    GameObject player;
    bool isPaused;


	// Use this for initialization
	void Start () {
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabaseScript>();

        for (int i = 0; i < (slotsX *slotsY); i++)
        {
            //add empty slots to list
            slots.Add(new ItemScript());
            //add blank item to every slot on the list
            inventory.Add(new ItemScript());
        }
   
        AddItem(0);
        AddItem(0);
        AddItem(1);
        AddItem(1);
	}

    void Update()
    {

        player = GameObject.FindWithTag("Player");

        if (Input.GetKeyDown(KeyCode.K))
        {
            isPaused = !isPaused;
        }
        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            showinventory = true;
            player.transform.GetComponent<MovementController>().enabled = false;
            player.transform.GetComponent<CameraController>().enabled = false;

        }
        else
        {
            showinventory = false;
            player.transform.GetComponent<MovementController>().enabled = true;
            player.transform.GetComponent<CameraController>().enabled = true;
        }
    }

    void OnGUI()
    {
        toolTip = "";
        GUI.skin = skin;

        if (showinventory)
        {
            DrawInventory();
            //if showing inventory show tooltip
            if (showToolTip)
            {
                GUI.Box(new Rect(Event.current.mousePosition.x+15f, Event.current.mousePosition.y, 200, 200), toolTip, skin.GetStyle("Tooltip"));

            }
            if (draggingItem)
            {

                GUI.DrawTexture(new Rect(Event.current.mousePosition.x -30f, Event.current.mousePosition.y, 50, 50),draggedItem.itemIcon); 
            }

           /* if (GUI.Button(new Rect(40, 250, 100, 40), "Save"))
                SaveInventory();
            if (GUI.Button(new Rect(40, 300, 100, 40), "Load"))
            {
                LoadInventory();
            }*/
        }
        
    }

    void DrawInventory()
    {
        Event e = Event.current;
        int i = 0;
        ItemScript item = slots[i];

        for (int y = 0; y < slotsY; y++)
        {
            for (int x = 0; x < slotsX; x++)
            {
                Rect slotRect = new Rect(x*40, y * 40,30,30);
                GUI.Box(new Rect(slotRect), "", skin.GetStyle("Slots"));
                item = inventory[i];
                if (item.itemName != null)
                {
                    //this is the rectanguel i whant to use
                    GUI.DrawTexture(slotRect, item.itemIcon);
                    //with in this inventory rectangle, the mouse position layes. We can now se if the mouse is howering over this slot
                    if (slotRect.Contains(e.mousePosition))
                    {
                    
                       //toolTip = CreateToolTip(slots[i]);
                        //showToolTip = true;
                        //if left mousebutton pressed pickup item
                        if (e.isMouse && e.button == 0 && e.type == EventType.mouseDrag && !draggingItem)
                        {
                            draggingItem = true;
                            //we set the index number from the index we are dragging
                            draggingIndex = i;
                            //set the item to the item in that slot
                            draggedItem = item;
                            inventory[i] = new ItemScript();
                        }
                        //swap items
                        if (e.isMouse && e.type == EventType.mouseUp && draggingItem)
                        {
                            //swap items, make the item in the slot go to the possition of the dragged item origan.
                            inventory[draggingIndex] = inventory[i];
                            //set the dragged item to be the new item in the new slot
                            inventory[i] = draggedItem;
                            //we are no longer dragging items
                            draggingItem = false;
                            //set draggeditem to null, we are no longer dragging items.
                            draggedItem = null;
                        }
                        //don't show tooltip if we are dragging
                        if (!draggingItem)
                        {
                            toolTip = CreateToolTip(inventory[i]);
                            showToolTip = true;
                        }
                        if (e.isMouse && e.type == EventType.mouseDown && e.button == 1)
                        {
                            if (item.itemType == ItemScript.ItemType.Consumable)
                            {
                               // print("use food");
                                UseFood(slots[i],i,true);
                            }
                        }
                    }

                    if (toolTip == "")
                    {
                        showToolTip = false;
                    }

                }
                else
                {
                    //drop item ind a empty slot
                    if (slotRect.Contains(e.mousePosition))
                    {
                        if (e.isMouse && e.type == EventType.mouseUp && draggingItem)
                        {
                           //set the dragged item to be the new item in the new slot
                            inventory[i] = draggedItem;
                            //we are no longer dragging items
                            draggingItem = false;
                            //set draggeditem to null, we are no longer dragging items.
                            draggedItem = null; 
                        }
                        
                    }
                }
                i++;
            }
        }
    }

    string CreateToolTip( ItemScript item)
    {
        toolTip = item.itemName + "\n\n"+item.ItemDesc;
        return toolTip;
    }

    void AddItem(int id)
    {
        //find all slots
        for (int i = 0; i < inventory.Count; i++)
        {
            //is the slot empty
            if (inventory[i].itemName == null)
            {
                //tjek all items ind database
                for (int j = 0; j < database.items.Count; j++)
                {
                    //find the correct item
                    if (database.items[j].itemID == id)
                    {
                        //set item in inventori = the item ind the database
                        inventory[i] = database.items[j];
                    }
                }
                //when found stop
                break;
            }
        }
    }

    void RemoveItem(int id)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            //if we find the item with that id
            if (inventory[i].itemID == id)
            {
                //when found replace item with empty item, then the slot won't disappear
                inventory[i] = new ItemScript();
                //stop this makes sur that only one item disappears, not alle with that id
                break;
            }
        }
    }

    private void UseFood(ItemScript item, int slot, bool deleteItem)
    {
        switch (item.itemID)
        {
            case 1:
                {
                    //here we kan fx increase health on the Player
                    print("Food eating: "+ item.itemName);
                    break;
                }
        }
        if (deleteItem)
        {
            inventory[slot] = new ItemScript();
        }
    }

    
    bool InventoryContains(int id)
    {
        bool result = false;
        //looper igennem vores hvor mange inventar slots vi har
        for (int i = 0; i < inventory.Count; i++)
        {
            //we whant to se if this item exists
            result = inventory[i].itemID == id;
            //If I find the a item with id
            if (result)
            {
                //stop looping throug our iventory
                break;
            }
        }
        return result;
    }

    void SaveInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            PlayerPrefs.SetInt("Inventory " + i, inventory[i].itemID); 
        }
        
    }

    void LoadInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i] = PlayerPrefs.GetInt("Inventory "+i,-1) >= 0 ? database.items[PlayerPrefs.GetInt("Inventory "+i)] : new ItemScript();
        }
    }
}
