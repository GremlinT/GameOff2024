using System.Collections;
using System.Collections.Generic;
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
        //if (currentItem == null)
        //{
            itemName.GetComponentInChildren<Text>().text = displayedName;
            itemName.SetActive(true);
            itemName.transform.position = Input.mousePosition + itemNameOffset;
        //}
    }
    public void HideItemName()
    {
        itemName.SetActive(false);
    }
    /*
    private UsableItem currentItem;
    public void ShowItemMenu(UsableItem selectetdItem)
    {
        currentItem = selectetdItem;
        itemMenu.transform.position = Input.mousePosition;
        HideItemName();
        itemMenu.SetActive(true);
    }
    public void MakeOpinion()
    {
        Debug.Log(currentItem.MakeOpinion());
        currentItem = null;
        itemMenu.SetActive(false);
    }
    public void UseItem()
    {
        player.UseItem(currentItem);
        currentItem = null;
        itemMenu.SetActive(false);
    }
    */
}
