using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [HideInInspector]
    public RegionSprites regionSprites;

    public void Awake()
    {
        regionSprites = GetComponent<RegionSprites>();
    }

}
