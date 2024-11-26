using UnityEngine;
using UnityEngine.UI;

public class BaseUIBehavoiur : MonoBehaviour
{
    [SerializeField]
    private Vector3 itemNameOffset;
    [SerializeField]
    private GameObject itemName;

    [SerializeField]
    private GameObject itemMenu;

    [SerializeField]
    private GameObject dialogPanel;

    [SerializeField]
    private GameObject pauseMenu;

    private AlienBehavoiur player;

    private void Awake()
    {
        UsableItem[] items = FindObjectsByType<UsableItem>(FindObjectsSortMode.None);
        foreach (UsableItem item in items)
        {
            item.SetUIBehavoiur(this);
        }
        player = FindObjectOfType<AlienBehavoiur>();
    }

    public void ShowItemName(string displayedName)
    {
        itemName.GetComponentInChildren<Text>().text = displayedName;
        itemName.SetActive(true);
        itemName.transform.position = Input.mousePosition + itemNameOffset;
    }
    public void HideItemName()
    {
        itemName.SetActive(false);
    }
    
    public void ShowDialog(string dialog)
    {
        dialogPanel.SetActive(true);
        dialogPanel.GetComponentInChildren<Text>().text = dialog;
    }
    public void HideDialog()
    {
        dialogPanel.SetActive(false);
    }

    public void ShowPauseMenu(bool isShown)
    {
        pauseMenu.SetActive(isShown);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void ResumeGame()
    {
        World.onPause = !World.onPause;
        World.SetPause();
        ShowPauseMenu(World.onPause);
    }
}
