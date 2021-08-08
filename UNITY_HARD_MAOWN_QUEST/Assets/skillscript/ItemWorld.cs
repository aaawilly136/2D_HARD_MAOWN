using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;



public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position,Item item)
    {
        Transform transform = Instantiate(ItemAssets.instance.pfItemWorld,position,Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }
    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();
        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomDir * 5f, item);
        itemWorld.GetComponent<Rigidbody2D>().AddForce(randomDir * 5, ForceMode2D.Impulse);
        return itemWorld;
    }
    private Item item;
    private SpriteRenderer spriteRenderer;
 
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        

    }
    private void Start()
    {
        
    }
    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
      
    }
    public Item GetItem()
    {
        return item;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
