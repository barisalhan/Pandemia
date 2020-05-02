using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSprites : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] regionSprites;

    // [Name of the spriteRenderer, Index of that spriteRenderer in the sprites list.]
    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();

    public enum Name
    {
        MidWest,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,  
        West
    }

    public void Awake()
    {
        CreateIndexTable();
    }

    /*
    * Creates an index table to enable easy access to sprites
    * by using Name enum.
    * WARNING:
    *   It is currently static. TODO: Change this!
    */
    public void CreateIndexTable()
    {
        indexTable.Add(Name.MidWest, 0);
        indexTable.Add(Name.NorthEast, 1);
        indexTable.Add(Name.NorthWest, 2);
        indexTable.Add(Name.SouthEast, 3);
        indexTable.Add(Name.SouthWest, 4);
        indexTable.Add(Name.West, 5);
    }

    public SpriteRenderer GetRegionSprite(String regionName)
    {
        Name currentRegion;
        Enum.TryParse<Name>(regionName, out currentRegion);
        int index = indexTable[currentRegion];
        return regionSprites[index];
    }
}
