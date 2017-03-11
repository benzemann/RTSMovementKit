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
            var newAgent = Instantiate(agentPrefab, transform.position, Quaternion.identity) as GameObject;
            if(newAgent.GetComponent<AgentController>() != null)
            {
                newAgent.GetComponent<AgentController>().GoToPos(transform.position + (transform.up * -10f));
            }
        }
    }
}
