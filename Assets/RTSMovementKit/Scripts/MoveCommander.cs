using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable)), RequireComponent(typeof(AgentController))]
public class MoveCommander : MonoBehaviour {
    [SerializeField, Tooltip("You can either use the Teams to define enemies or tag." 
        +"Leave tag empty to use teams. If you use teams, "
        +"all with the team component and set to this team will be considered an enemy")]
    private Teams enemyTeam;
    [SerializeField, Tooltip("You can either use the Teams to define enemies or tag."
    + "Leave tag empty to use teams. If you use teams, "
    + "all with the team component and set to this team will be considered an enemy")]
    private string enemyTag;

    // Update is called once per frame
    void Update () {
        // Check for mouse down and if the gameobject is selected
        if (GetComponent<Selectable>().IsSelected && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    // Tell the agent to move to the position
                    GetComponent<AgentController>().GoToPos(hit.point);
                    // Clear target
                    if (GetComponent<TargetFinder>() != null)
                    {
                        GetComponent<TargetFinder>().Target = null;
                        GetComponent<TargetFinder>().GoToAndAttack = false;
                    }
                        


                }
                else if (GetComponent<TargetFinder>() != null & 
                    ((enemyTag != "" && hit.transform.gameObject.tag == enemyTag) ||
                    (hit.transform.gameObject.GetComponent<Team>() != null &&
                    hit.transform.gameObject.GetComponent<Team>().GetTeam == enemyTeam)))
                {
                    GetComponent<AgentController>().Stop();
                    // Set new target
                    if (GetComponent<TargetFinder>() != null)
                    {
                        GetComponent<TargetFinder>().Target = hit.transform.gameObject;
                        GetComponent<TargetFinder>().GoToAndAttack = true;
                    }
                }
            }
        }
	}

}
