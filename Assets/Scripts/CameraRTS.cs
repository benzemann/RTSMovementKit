﻿using UnityEngine;
using System.Collections;

public class CameraRTS : MonoBehaviour {

    public Camera camera;

    public const int LevelArea = 100;

    public const int ScrollArea = 25;
    public const int ScrollSpeed = 25;
    public const int DragSpeed = 100;

    public const int ZoomSpeed = 25;
    public const int ZoomMin = 5;
    public const int ZoomMax = 100;

    public const int PanSpeed = 50;
    public const int PanAngleMin = 50;
    public const int PanAngleMax = 80;

    // Update is called once per frame
    void Update()
    {
        // Init camera translation for this frame.
        var translation = Vector3.zero;

        // Zoom in or out
        var zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        if (zoomDelta != 0)
        {
            translation -= Vector3.up * ZoomSpeed * zoomDelta;
        }
        
        // Move camera with arrow keys
        translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move camera with mouse
        if (Input.GetMouseButton(2)) // MMB
        {
            // Hold button and drag camera around
            translation -= new Vector3(Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime, 0,
                               Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime);
        }
        else
        {
            // Move camera if mouse pointer reaches screen borders
            if (Input.mousePosition.x < ScrollArea)
            {
                translation += Vector3.right * -ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.x >= Screen.width - ScrollArea)
            {
                translation += Vector3.right * ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y < ScrollArea)
            {
                translation += Vector3.forward * -ScrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y > Screen.height - ScrollArea)
            {
                translation += Vector3.forward * ScrollSpeed * Time.deltaTime;
            }
        }

        // Keep camera within level and zoom area
        var desiredPosition = camera.transform.position + translation;
        if (desiredPosition.x < -LevelArea || LevelArea < desiredPosition.x)
        {
            translation.x = 0;
        }
        if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y)
        {
            translation.y = 0;
        }
        if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z)
        {
            translation.z = 0;
        }

        // Finally move camera parallel to world axis
        camera.transform.position += translation;
    }
}
