using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemScript {

    public string itemName;
    public int itemID;
    public string ItemDesc;
    public Texture2D itemIcon;
    public int itemPower;
    public int itemSpeed;
    public ItemType itemType;

   public enum ItemType {
        Weapon,
        Consumable,
        Quest
   }


    //dette er en cunstructor
   public ItemScript(string name, int id, string desc, int power, int speed, ItemType type)
   {
       itemName = name;
       itemID = id;
       itemIcon = Resources.Load<Texture2D>("Items/" + name);
       ItemDesc = desc;
       itemPower = power;
       itemSpeed = speed;
       itemType = type;
   }

   public ItemScript()
   {
       itemID = -1;
   }
}
