using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

    public GameObject slots;

    public List<GameObject> Slots = new List<GameObject>();
    public List<ItemScript2> Items = new List<ItemScript2>();
    ItemDatabase2 database;
    //sets start position of først row
    private float x = -90f;
    //sets the hight starting point of first row
    private float y = -12f;

    private bool isPaused;
    GameObject player;
    public Transform selectedItem, selectedSlot, originalSlot;
    public Canvas inventory;
	// Use this for initialization

    public GameObject tooltip;
    public GameObject draggeditem;
    public bool draggingItem = false;




    public void showTooltip(Vector3 toolPosition, ItemScript2 item)
    {
        tooltip.SetActive(true);
        tooltip.GetComponent<RectTransform>().localPosition = new Vector3(toolPosition.x + 80, toolPosition.y -80, toolPosition.z);

        //setting the tooltip texts
        tooltip.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
        tooltip.transform.GetChild(2).GetComponent<Text>().text = item.itemDesc;
    }

    public void showDraggedItem(ItemScript2 item)
    {
        draggeditem.SetActive(true);
        draggingItem = true;
        draggeditem.GetComponent<Image>().sprite = item.itemIcon;
    }

    public void closeTooltip()
    {
        tooltip.SetActive(false);
    }

	void Start () {
        int Slotamount = 0;

        database = GameObject.FindGameObjectWithTag("ItemDatabase2").GetComponent<ItemDatabase2>();

        inventory.enabled = false;

        //Create slots
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject slot = (GameObject)Instantiate(slots);
                slot.GetComponent<SlotController>().slotnumber = Slotamount;
                //Adding empty slot
                Slots.Add(slot);
                //Adding emplty item
                Items.Add(new ItemScript2());
                slot.transform.SetParent(this.gameObject.transform, false);
                slot.name = "Slot"+ i+"."+j;

                slot.GetComponent<RectTransform>().localPosition =  new Vector3(x, y,0);
                x = x + 22.5f;
                //if we have 4 slots in one row go down
                if (j == 8)
                {
                    //Sets start position of secound row
                    x = -90f;
                    //sets the distance between up and down icons
                    y = y -23.9f;
                }
                Slotamount++;
            }
        }

        AddItem(0);
        AddItem(1);
        AddItem(2);

        Debug.Log(Items[0].itemName);
        Debug.Log(Items[1].itemName);
	}
	
	// Update is called once per frame
	void Update () {

        if (draggingItem)
        {
            Vector3 position = (Input.mousePosition - GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>().localPosition);
            Debug.Log("Mous = "+Input.mousePosition + "Var posi = "+position);
           // draggeditem.GetComponent<RectTransform>().localPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            draggeditem.GetComponent<RectTransform>().localPosition = new Vector3(position.x, position.y, position.z);
        }

        player = GameObject.FindWithTag("Player");
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPaused = !isPaused;
            //showinventory = !showinventory;
        }

        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inventory.enabled = true;
            player.transform.GetComponent<MovementController>().enabled = false;
            player.transform.GetComponent<CameraController>().enabled = false;

        }
        else
        {
            inventory.enabled = false;
            player.transform.GetComponent<MovementController>().enabled = true;
            player.transform.GetComponent<CameraController>().enabled = true;
        }
	}

    void AddItem(int id)
    {
        for (int i = 0; i < database.items.Count; i++)
        {
            if (database.items[i].itemID == id)
            {
                //we take the Item from the database and put it ind the "item"
                ItemScript2 item = database.items[i];
                AddItemAddEmptySlot(item);
                break;
            }
        }
    }

    void AddItemAddEmptySlot(ItemScript2 item)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].itemName == null)
            {
                Items[i] = item;
                break;
            }
        }
    }
}
