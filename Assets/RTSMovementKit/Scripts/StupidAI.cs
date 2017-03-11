using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidAI : MonoBehaviour {
    [SerializeField, Tooltip("The stupid AI will every 30 seconds tell all idle units to attack this object")]
    private GameObject objectiveTarget;
    private float timeAtLastCommand = -20.0f;
	
	// Update is called once per frame
	void Update () {
		
        if(Time.time - timeAtLastCommand >= 20.0f)
        {
            timeAtLastCommand = Time.time;
            GameObject[] allRobotUnits = TeamManager.Instance.AllEnemyObjects;

            for(int i = 0; i < allRobotUnits.Length; i++)
            {
                if(allRobotUnits[i].GetComponent<AgentController>() != null && allRobotUnits[i].GetComponent<TargetFinder>() != null)
                {
                    if(allRobotUnits[i].GetComponent<TargetFinder>().Target == null && allRobotUnits[i].GetComponent<AgentController>().IsReady)
                    {
                        allRobotUnits[i].GetComponent<AgentController>().GoToPos(objectiveTarget.transform.position);
                        allRobotUnits[i].GetComponent<TargetFinder>().GoToAndAttack = true;
                    }
                }
            }
        }

	}
}
