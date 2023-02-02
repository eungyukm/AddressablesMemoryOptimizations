using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InventorySystemAddressable : MonoBehaviour
{
    public AssetReferenceGameObject[] inventoryItems;
    public Transform[] spawnPositions;
    private readonly Dictionary<int, List<GameObject>> _spawnedObjects = new Dictionary<int, List<GameObject>>();
    public void SpawnItem(int itemNumber)
    {
        if (!_spawnedObjects.ContainsKey(itemNumber))
        {
            _spawnedObjects.Add(itemNumber, new List<GameObject>());
        }

        if (_spawnedObjects[itemNumber].Count > 0)
        {
            Vector3 randomPos = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-1.5f, 1.5f), 0);
            StartCoroutine(WaitForSpawnComplete(InstantiateAsync(itemNumber, randomPos), itemNumber));
        }
        else
        {
            StartCoroutine(WaitForSpawnComplete(Addressables.InstantiateAsync(inventoryItems[itemNumber],
                spawnPositions[itemNumber].position, spawnPositions[itemNumber].rotation), itemNumber));
        }
    }
    
    // Addressables InstantiateAsync 메서드
    private AsyncOperationHandle<GameObject> InstantiateAsync(int itemNumber, Vector3 randomPos)
    {
        return Addressables.InstantiateAsync(inventoryItems[itemNumber],
            spawnPositions[itemNumber].position + randomPos, spawnPositions[itemNumber].rotation);
    }
    
    // Spawn이 완료 되었을 때, 코루틴
    IEnumerator WaitForSpawnComplete(AsyncOperationHandle<GameObject> op, int itemNumber)
    {
        while(op.IsDone == false)
        {
            yield return op;
        }

        OnSpawnComplete(op, itemNumber);
    }
    
    // 모든 Item을 Addressables.ReleaseInstance 처리
    public void DespawnItem(int itemNumber)
    {
        if (_spawnedObjects.TryGetValue(itemNumber, out var value))
        {
            foreach(var entry in value)
            {
                Addressables.ReleaseInstance(entry);
            }
            value.Clear();
        }
    }

    void OnSpawnComplete(AsyncOperationHandle<GameObject> handle, int itemNumber)
    {
        if (_spawnedObjects.TryGetValue(itemNumber, out var value))
        {
            value.Add(handle.Result);
        }
        else
        {
            _spawnedObjects.Add(itemNumber, new List<GameObject>() { handle.Result });
        }
    }
    
    // 모든 inventory Item Spawn
    public void SpawnAll(int amount)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                SpawnItem(i);
            }
        }
    }
    
    // 모든 메모리 Unload
    public void MemoryUnloadAll()
    {
        Resources.UnloadUnusedAssets();
    }
}
