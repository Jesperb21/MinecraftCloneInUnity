using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemScript2 {

    public string itemName;
    public int itemID;
    public string itemDesc;
    public Sprite itemIcon;
    public GameObject itemModel;
    public int itemPower;
    public int itemSpeed;
    public int itemValue;
    public ItemType itemType;

    public enum ItemType
    {
        Weapon,
        Consumable,
        Quest,
        Head,
        Shoes,
        Chest,
        Trousters,
        Errings,
        Necklace,
        Rings,
        Hands
    }

    public ItemScript2(string name, int id, string desc, int power, int speed, int value, ItemType type)
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemPower = power;
        itemSpeed = speed;
        itemValue = value;
        itemType = type;
        itemIcon = Resources.Load<Sprite>("Items/Sprits/" + name);
    }

    public ItemScript2()
    {
    }
}
