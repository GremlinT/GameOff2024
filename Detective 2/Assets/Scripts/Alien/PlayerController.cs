using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
/* ������ �������� ����� �� ������. 
 * 
 * ���� ������ ��������� �� ������������ ��������� - ��������������.
 * 
 * �� ������������ ���� ����� ������� �����.
 * ���� ���������� ���� ������, �� ����������� ��� �� ������ ����� ������� �� ������ � �����������.
 * ���� ����� ������ ���, �� �����������:
 * - �������� �� ����� ������ ������������, �� ������� ����� ������ (� ���� ���� ��������� WalkableSurface);
 *  � ���� ������ � ������������ ��������� ����������� ����� �������� � ���������� �����, �� ������� ��� ����
 * - �������� �� ����� ������ ������������ �������� (� ���� ���� ��������� UsableItem).
 *  � ���� ������ � ������������ ��������� ����������� ����� ������������� �������, �� �������� ��� ����
 * 
 */
public class PlayerController : MonoBehaviour
{
    private AlienBehavoiur player;

    private void Start()
    {
        player = GetComponent<AlienBehavoiur>();
    }

    public void MouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedGO = hit.collider.gameObject;
            if (clickedGO != null)
            {
                if (clickedGO.GetComponent<WalkableSurface>())
                {
                    player.MoveTo(hit.point);
                }
                if (clickedGO.GetComponent<UsableItem>())
                {
                    player.UseItem(clickedGO.GetComponent<UsableItem>());
                }
            }
        }
        
        
    }

}
