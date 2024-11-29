using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpasechipPanelButton : MonoBehaviour
{
    private Transform TR;
    private Spaceship spaceship;
    private Vector3 basePosition;

    [SerializeField]
    private SpaceshipButtons buttonName;
    [SerializeField]
    private GameObject buttonProjectile;

    [SerializeField]
    private Material[] buttonMaterials;

    private MeshRenderer render;
    private bool isActive;

    private void Start()
    {
        TR = transform;
        spaceship = FindObjectOfType<Spaceship>();
        basePosition = TR.localPosition;
        render = GetComponent<MeshRenderer>();
    }

    private void OnMouseDown()
    {
        TR.localPosition = TR.localPosition + TR.up * -0.02f;
        spaceship.PanelButtonClick(this);
    }
    private void OnMouseUp()
    {
        TR.localPosition = basePosition;
    }
    private void OnMouseExit()
    {
        TR.localPosition = basePosition;
    }

    public SpaceshipButtons GetButtonType()
    {
        return buttonName;
    }

    public bool IsActive()
    {
        return isActive;
    }
    public void TurnButtonOnOff(bool isOn)
    {
        if (isOn)
        {
            render.material = buttonMaterials[1];
            buttonProjectile.SetActive(true);
        }
        else
        {
            render.material = buttonMaterials[0];
            buttonProjectile.SetActive(false);
        }
    }

    public void ActivateButton(bool isActivate)
    {
        if (isActivate)
        {
            render.material = buttonMaterials[3];
            isActive = true;
        }
        else
        {
            render.material = buttonMaterials[2];
            isActive = false;
        }
        
    }
    public void ButtonIsReadyOrNot(bool isReady)
    {
        if (isReady)
        {
            render.material = buttonMaterials[2];
        }
        else
        {
            render.material = buttonMaterials[1];
        }
        
    }
}
