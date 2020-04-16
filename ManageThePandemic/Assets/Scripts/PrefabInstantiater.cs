using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/*
 * Initiates an instance of desired prefab.
 */
public class PrefabInstantiater : MonoBehaviour
{
    // Main canvas of the game.
    public Canvas canvas;
    
    public List<GameObject> prefabs;

    // [Name of the prefab, Index of that prefab in the prefabs list.]
    public Dictionary<string, int> indexTable = new Dictionary<string, int>();

    public void Start()
    {
        indexTable.Add("ActionAsker", 0);
    }


    /*
     * prefabName: Object.name
     */
    public GameObject GetPrefab(string prefabName)
    {
        int index = indexTable[prefabName];
        return prefabs[index];
    }


    /*
     * 
     */
    public  void InstantiatePrefab(string prefabName)
    {
        GameObject prefab= GetPrefab(prefabName);
        GameObject instanceOfPrefab = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        instanceOfPrefab.transform.SetParent(canvas.transform, false);
    }
}
