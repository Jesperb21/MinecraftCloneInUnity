using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotController : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler, IPointerExitHandler, IDragHandler {

    public ItemScript2 item;
    Image itemImage;
    public int slotnumber;
    InventoryController inventory;



    
	// Use this for initialization
	void Start () {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryController>();
        itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
      
	}
	
	// Update is called once per frame
	void Update () {
        if (inventory.Items[slotnumber].itemName != null)
        {
            itemImage.enabled = true;
            itemImage.sprite = inventory.Items[slotnumber].itemIcon;
        }
        else
        {
            itemImage.enabled = false;
        }
	}

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("slot clicked");
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (inventory.Items[slotnumber].itemName != null)
        {
            inventory.showTooltip(inventory.Slots[slotnumber].GetComponent<RectTransform>().localPosition, inventory.Items[slotnumber]);
            //Debug.Log(inventory.Items[slotnumber].itemDesc);

        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (inventory.Items[slotnumber].itemName != null)
        {
           inventory.closeTooltip(); 
        }
        
    }
    public void OnDrag(PointerEventData data)
    {
        if (inventory.Items[slotnumber].itemName != null)
        {
            inventory.showDraggedItem(inventory.Items[slotnumber]);
        }
    }
}
