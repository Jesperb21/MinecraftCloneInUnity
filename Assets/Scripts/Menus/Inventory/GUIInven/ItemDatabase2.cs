using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase2 : MonoBehaviour {


    public List<ItemScript2> items = new List<ItemScript2>();
	// Use this for initialization
	void Start () {
        items.Add(new ItemScript2("woodsword", 0, "wood", 2, 2, 0, ItemScript2.ItemType.Weapon));
        items.Add(new ItemScript2("apple", 1, "apple", 0, 0, 0, ItemScript2.ItemType.Consumable));
        items.Add(new ItemScript2("stick", 2, "wood", 1, 1, 0, ItemScript2.ItemType.Head));
	}
	
}
