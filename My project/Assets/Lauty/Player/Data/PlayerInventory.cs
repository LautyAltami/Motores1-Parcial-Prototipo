using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // La lista invisible donde guardamos los IDs
    public List<string> keys = new List<string>();

    public void AddKey(string id)
    {
        if (!keys.Contains(id))
        {
            keys.Add(id);
            Debug.Log("Llave guardada en el bolsillo: " + id);
        }
    }

    public bool HasKey(string id)
    {
        return keys.Contains(id);
    }
}