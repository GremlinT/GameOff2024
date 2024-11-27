using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private BaseUIBehavoiur baseUIBehavoiur;
    
    [SerializeField]
    private KeyCode pauseKey;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            World.onPause = !World.onPause;
            World.SetPause();
            baseUIBehavoiur.ShowPauseMenu(World.onPause);
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
