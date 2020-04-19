using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


/*
 * Holds the list of all prefabs and
 * instantiate the requested prefab to the canvas.
 */
public class Prefabs : MonoBehaviour
{
    // Main canvas of the game.
    public Canvas canvas;
    
    public List<GameObject> prefabs;

    // [Name of the prefab, Index of that prefab in the prefabs list.]
    public Dictionary<Name, int> indexTable = new Dictionary<Name, int>();

    public enum Name
    {
        ActionAsker
    }


    public void Start()
    {
        CreateIndexTable();
    }


    /*
     * Creates an index table to enable easy access to prefabs
     * by using Name enum.
     * TODO: Generalize this method.
     */
    public void CreateIndexTable()
    {
        indexTable.Add(Name.ActionAsker, 0);
    }


    public GameObject GetPrefab(Name prefabName)
    {
        int index = indexTable[prefabName];
        return prefabs[index];
    }
}
