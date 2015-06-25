using UnityEngine;
using System.Collections;

//The item is serialized
//because we want to be able to se all item variabels and interact through the inspector
[System.Serializable]
//The class does not inherit from monodevelop, because we want it to inherit from Object
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


    //a constuctor overloaded
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

    //a constructor
   public ItemScript()
   {
       itemID = -1;
   }
}
