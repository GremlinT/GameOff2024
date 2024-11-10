using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    
    [SerializeField]
    private KeyCode pauseKey;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            World.onPause = !World.onPause;
            World.SetPause(World.onPause);
        }

        if (!World.onPause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerController.MouseClick();
            }
        }
    }
}
