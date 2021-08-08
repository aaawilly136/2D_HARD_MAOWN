using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    public bool IsStackabke()
    {
        switch(itemType)
        {
            default:
            case ItemType.health:
            case ItemType.poison:
            case ItemType.fire:
                return true;
            
                //return false;
            
        }
    }
    
}
