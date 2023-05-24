using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 newPosition = player.position + new Vector3(0f, 0f, -10);
        transform.position = newPosition;
        if (SceneManager.GetActiveScene().name.Equals("LevelOne"))
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -3f, 223.5f),
                                 Mathf.Clamp(transform.position.y, 1.08f, 2.6f), transform.position.z);
        }
        else if (SceneManager.GetActiveScene().name.Equals("LevelTwo"))
        {

        }
        else if (SceneManager.GetActiveScene().name.Equals("LevelTree"))
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, 4.8f, 351.2f),
                                 Mathf.Clamp(transform.position.y, -1.4f, 23.5f), transform.position.z);
        }
        else if (SceneManager.GetActiveScene().name.Equals("LevelFour"))
        {

        }

    }
}
