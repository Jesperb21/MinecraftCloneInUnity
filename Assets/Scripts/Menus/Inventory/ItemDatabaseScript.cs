using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabaseScript : MonoBehaviour {

    // A List of Items
    public List<ItemScript> items = new List<ItemScript>();

    void Start()
    {
        //Add items to the Itemsdatabase
        items.Add(new ItemScript("woodsword",0,"wood",2,2,ItemScript.ItemType.Weapon));
        items.Add(new ItemScript("apple", 1, "apple", 2, 2, ItemScript.ItemType.Consumable));
        items.Add(new ItemScript("stick", 2, "stick", 2, 2, ItemScript.ItemType.Consumable));
    }

}
