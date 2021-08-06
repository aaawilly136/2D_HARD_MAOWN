using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
   public static ItemAssets instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    public Transform pfItemWorld;

    public Sprite fireSpirite;
    public Sprite healthSpirite;
    public Sprite poisonSpirite;
}
