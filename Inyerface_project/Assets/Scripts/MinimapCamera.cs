using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player;
    public bool followRotation;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        this.transform.rotation = Quaternion.Euler(90, 180, 0);
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        if(followRotation)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
