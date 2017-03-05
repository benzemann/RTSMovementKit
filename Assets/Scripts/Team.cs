using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

    [SerializeField, Tooltip("The team this object is part of")]
    private Teams team;

	// Use this for initialization
	void Start () {
        TeamManager.Instance.AddTeamObject(this.gameObject, team);
	}

    private void OnDestroy()
    {
        TeamManager.Instance.RemoveTeamObject(this.gameObject, team);
    }
}
