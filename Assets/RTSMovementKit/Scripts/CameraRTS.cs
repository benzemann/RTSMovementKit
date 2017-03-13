using UnityEngine;
using System.Collections;

public class CameraRTS : MonoBehaviour {
    [SerializeField, Tooltip("The camera")]
    private Camera camera;
    [SerializeField, Tooltip("How close the mouse has to be to the screen border to start scrolling")]
    private int screenBorder;
    [SerializeField, Tooltip("How fast you can scroll the camera.")]
    private int ScrollSpeed;
    [SerializeField, Tooltip("How fast you can drag the camera with the middle mouse button")]
    private int DragSpeed;
    [SerializeField, Tooltip("How fast you can zoom in with the camera")]
    private int ZoomSpeed;

    // Update is called once per frame
    void Update()
    {
        var translation = Vector3.zero;

        var zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        if (zoomDelta != 0)
        {
            translation -= Vector3.up * ZoomSpeed * zoomDelta;
        }

        translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetMouseButton(2))
        {
            
            translation -= new Vector3(Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime, 0,
                               Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime);
        }
        else
        {
            if (Input.mousePosition.x < screenBorder)
            {
                translation += Vector3.right * -ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.x >= Screen.width - screenBorder)
            {
                translation += Vector3.right * ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y < screenBorder)
            {
                translation += Vector3.forward * -ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y > Screen.height - screenBorder)
            {
                translation += Vector3.forward * ScrollSpeed * Time.deltaTime;
            }
        }
        camera.transform.position += translation;
    }
}
