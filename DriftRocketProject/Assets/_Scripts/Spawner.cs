using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private bool isActive = true;
    [SerializeField]
    private string spawnObject = "Cube";
    [SerializeField]
    private float initialWaitTime = 0;
    [SerializeField]
    private float waitTime = 5;
    [SerializeField]
    private float spawnChance = 0.5f;
    [SerializeField]
    private float spawnRange = 10;
    [SerializeField]
    private float spawnDistanceFromPlayer = 150;
    [SerializeField]
    private int minSpawn = 1;
    [SerializeField]
    private int maxSpawn = 2;
    [SerializeField]
    private float minSize = 1;
    [SerializeField]
    private float maxSize = 1;

    private void Start()
    {
        if (!isActive) return;
        GameController.StartSpawnEvent += StartSpawn;
        GameController.StopSpawnEvent += StopSpawn;
    }
    private void OnApplicationQuit()
    {
        if (!isActive) return;
        GameController.StartSpawnEvent -= StartSpawn;
        GameController.StopSpawnEvent -= StopSpawn;
    }
    public void StartSpawn()
    {
        StartCoroutine("Spawn");
    }
    public void StopSpawn ()
    {
        StopCoroutine("Spawn");
    }
    private void SpawnObjects ()
    {
        if (Random.value > spawnChance) return;
        Vector3 randomSpawn = Random.insideUnitCircle.normalized * spawnDistanceFromPlayer;
        int times = Random.Range(minSpawn, maxSpawn);
        for (int i = 0; i < times; i++)
        {
            GameObject newObject = ObjectPooler.PoolObject(spawnObject);
            if (newObject == null) continue;
            if (minSize != maxSize) newObject.transform.localScale = Vector3.one * Random.Range(minSize, maxSize);
            Vector3 privateSpawn = randomSpawn + (Random.insideUnitSphere * spawnRange);
            if (privateSpawn.x > 0) privateSpawn.x += 25;
            else privateSpawn.x -= 25;
            privateSpawn.z = 0;
            newObject.transform.position = PlayerController.instance.transform.position + privateSpawn;
            RocketController newRocketController = newObject.GetComponent<RocketController>();
            if (newRocketController != null) newRocketController.SetActive(true);
            Entity newEntity = newObject.GetComponent<Entity>();
            if (newEntity != null) newEntity.ResetEntity();
        }
    }
    private float ClampLength (float length, float clampLength)
    {
        if (length > 0)
        {
            if (length < clampLength) return clampLength;
        } else
        {
            if (length > -clampLength) return -clampLength;
        }
        return length;
    }
    private IEnumerator Spawn ()
    {
        yield return new WaitForSeconds(initialWaitTime);
        while (true)
        {
            SpawnObjects();
            yield return new WaitForSeconds(waitTime);
        }
    }
}
