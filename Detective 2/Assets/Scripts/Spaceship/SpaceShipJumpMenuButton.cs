using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceShipJumpMenuButton : MonoBehaviour
{
    [SerializeField]
    private int targetSceneMonber;

    private Spaceship spaceship;

    private MeshRenderer render;
    private Material baseMaterial;
    [SerializeField]
    private Material emissionMaterial;

    private void Start()
    {
        spaceship = FindObjectOfType<Spaceship>();
        render = GetComponent<MeshRenderer>();
        baseMaterial = render.material;
    }
    private void OnMouseDown()
    {
        spaceship.SetTargetForJump(targetSceneMonber);
    }
    private void OnMouseEnter()
    {

    }
    private void OnMouseExit()
    {

    }
}
