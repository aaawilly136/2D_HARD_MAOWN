using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public enum ItemType 
    {
        fire,
        health,
        poison,
    }
    public ItemType itemType;
    public int amount;

    public Sprite GetSprite() 
    {
        switch (itemType)
        {
            default:
            case ItemType.fire:     return ItemAssets.instance.fireSpirite;
            case ItemType.health:   return ItemAssets.instance.healthSpirite;
            case ItemType.poison:   return ItemAssets.instance.poisonSpirite;
        }
    }
}
