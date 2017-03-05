using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {
    [SerializeField, Tooltip("The object that should be spawned")]
    private GameObject agentPrefab;
    [SerializeField, Tooltip("How often an agent will spawn")]
    private float spawnRate;

    private float timeAtLastSpawn;

    private void Update()
    {
        if(Time.time - timeAtLastSpawn >= spawnRate)
        {
            timeAtLastSpawn = Time.time;
            Instantiate(agentPrefab, transform.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)), Quaternion.identity);
        }
    }
}
