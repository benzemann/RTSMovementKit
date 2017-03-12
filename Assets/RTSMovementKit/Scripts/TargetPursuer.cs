using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetFinder)), RequireComponent(typeof(Damager)), RequireComponent(typeof(AgentController))]
public class TargetPursuer : MonoBehaviour {

    private bool goToTarget = false;
    private Vector3 targetOldPos;

	// Update is called once per frame
	void Update () {

        var target = GetComponent<TargetFinder>().Target;

        if(target != null)
        {
            var dis = Vector3.Distance(target.transform.position, transform.position);
            if(dis > GetComponent<Damager>().AttackRange && (GetComponent<AgentController>().IsReady))
            {
                goToTarget = true;
                targetOldPos = target.transform.position;
                GetComponent<AgentController>().GoToPos(target.transform.position);
            }
        }
        if (goToTarget)
        {
            if(target == null)
            {
                //GetComponent<AgentController>().Stop();
                GetComponent<TargetFinder>().GoToAndAttack = true;
                goToTarget = false;
                return;
            }
            var dis = Vector3.Distance(target.transform.position, targetOldPos);
            if(dis > 10f)
            {
                targetOldPos = target.transform.position;
                GetComponent<AgentController>().GoToPos(target.transform.position);
            }
        }
	}
}
