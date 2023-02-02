using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public GameObject[] inventoryItems;
    public Transform[] spawnPositions;
    private GameObject[] _spawnedObjects;

    void Start()
    {
        _spawnedObjects = new GameObject[inventoryItems.Length];
    }

    public void SpawnItem(int itemNumber)
    {
        if (_spawnedObjects[itemNumber] != null)
        {
            return;
        }

        _spawnedObjects[itemNumber] = Instantiate(inventoryItems[itemNumber],
            spawnPositions[itemNumber].position, spawnPositions[itemNumber].rotation);
    }

    public void DespawnItem(int itemNumber)
    {
        if (_spawnedObjects[itemNumber] == null)
            return;

        Destroy(_spawnedObjects[itemNumber]);
    }
}
