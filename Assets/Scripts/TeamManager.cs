using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Teams
{
    PlayerTeam = 0,
    EnemyTeam = 1
}
public class TeamManager : Singleton<TeamManager> {
    
    List<GameObject> playerTeam;
    List<GameObject> enemyTeam;

    public GameObject[] AllPlayerObjects { get { return playerTeam.ToArray(); } }
    public GameObject[] AllEnemyObjects { get { return enemyTeam.ToArray(); } }

	// Use this for initialization
	void Awake () {
        playerTeam = new List<GameObject>();
        enemyTeam = new List<GameObject>();
	}

    public GameObject[] GetAllObjectsInTeam(Teams team)
    {
        switch (team)
        {
            case Teams.PlayerTeam:
                return AllPlayerObjects;
            case Teams.EnemyTeam:
                return AllEnemyObjects;
        }
        return null;
    }
	
    public void AddTeamObject(GameObject obj, Teams team)
    {
        switch (team)
        {
            case Teams.PlayerTeam:
                playerTeam.Add(obj);
                break;
            case Teams.EnemyTeam:
                enemyTeam.Add(obj);
                break;
            default:
                Debug.LogWarning("Trying to add an object to an unknown team");
                break;
        }
    }

    public void RemoveTeamObject(GameObject obj, Teams team)
    {
        switch (team)
        {
            case Teams.PlayerTeam:
                if(playerTeam.Contains(obj))
                    playerTeam.Remove(obj);
                break;
            case Teams.EnemyTeam:
                if(enemyTeam.Contains(obj))
                    enemyTeam.Remove(obj);
                break;
            default:
                Debug.LogWarning("Trying to remove an object to an unknown team");
                break;
        }

    }


}
