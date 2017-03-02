using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Collider) )]
public class Selectable : MonoBehaviour {

    #region private variables
    [Header("Selection plane variables")]
    [SerializeField, Tooltip("This gameobject will be shown below the gameobject when selected")]
    private GameObject selectionProjectorPrefab;
    [SerializeField, Tooltip("The selection plane will be instatiated at local (0,0,0) + this offset")]
    private Vector3 offset;
    [SerializeField, Tooltip("Rotation of the selectionplane")]
    private Vector3 selectionProjectorOffset;
    [SerializeField, Tooltip("The size of the selection plane is calculated based on the size of the model + this scale")]
    private Vector3 scaling;
    [SerializeField, Tooltip("Defines whether the selection plane should be a square or allowed to be rectangular")]
    private bool keepSquare;
    private GameObject selectionPlane;
    private bool isSelected;
    #endregion

    #region Properties
    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }
    #endregion

    // Use this for initialization
    void Start () {
        SelectableManager.Instance.AddSelectable(this);
    }

    private void Update()
    {
        if(isSelected && selectionProjectorPrefab != null)
        {
            // If there is no selectionPlane, instantiate one.
            if(selectionPlane == null)
            {
                selectionPlane = Instantiate(selectionProjectorPrefab, this.transform.position + selectionProjectorOffset, Quaternion.Euler(new Vector3(90.0f,0.0f,0.0f)), this.transform);
            }
            selectionPlane.SetActive(true);

        } else if (!isSelected && selectionPlane != null)
        {
            selectionPlane.SetActive(false);
        }
    }

    /// <summary>
    /// Is called when the mouse is released untop of this object.
    /// </summary>
    private void OnMouseUp()
    {
        SelectableManager.Instance.SelectObject(this);
    }

    private void OnDestroy()
    {
        SelectableManager.Instance.RemoveSelectable(this);
    }

}
