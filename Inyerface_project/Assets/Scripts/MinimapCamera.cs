using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player;
    public bool followRotation;
    public CameraUIType cameraUIType;

    private Transform playerCam;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        playerCam = player.gameObject.GetComponentInChildren<Camera>().transform;
        this.transform.rotation = Quaternion.Euler(90, 180, 0);
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;

        switch (cameraUIType)
        {
            case CameraUIType.RotatingMinimap:
                newPosition.y = transform.position.y;
                transform.position = newPosition;
                transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
                break;
            case CameraUIType.StaticMinimap:
                newPosition.y = transform.position.y;
                transform.position = newPosition;
                break;
            case CameraUIType.RearViewMirror:
                newPosition.y += .629f; //height offset to place camera at head height
                transform.position = newPosition;
                transform.rotation = Quaternion.Euler(0f, player.eulerAngles.y + 180f, 0f);
                break;
            default:
                break;
        }


    }
}

public enum CameraUIType
{
    RotatingMinimap,
    StaticMinimap,
    RearViewMirror
}
