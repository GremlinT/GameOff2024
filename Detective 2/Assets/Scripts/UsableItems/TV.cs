using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TV : UsableItem
{
    [SerializeField]
    private Material offLineMaterial, onLineMaterial;

    MeshRenderer meshRenderer;

    private bool lookAtTV;

    private string[] newsDialog = 
        {
        "������ ����. � ����� ������� ���������.",
        "��������� ������� - ������ ���� �� ����� ������� ������� ����������.",
        "������� ����� ��������� ����������, ��� ����� ���������� �������, ���� �� ����� ������� � ��������� ����������.",
        "�� ��������� ����������, �� ����� ������ ������� ��������� ���������� ����������� ������� � ����� ������ ��������� ������.",
        "��� ������ �� ������ ����������, �� ������ ����� �� ������ �� �������������� �������.",
        "��������� �������� ����� ���������, ������ ������� ���� �� ����������. �� ������ �� ��������� �������!"
        };
    private string[] addsDialog =
        {
        "�������� ��� \"� ����������\", ������ ��� � ���������!",
        "��������� ���������� 4000, ��������� ������, ������ ������ � �������!",
        "����� ����� ������ �������� ��� �� �������� �������� ���.",
        };

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void Use(AlienBehavoiur user)
    {
        cameraPoint.position = user.transform.position + Vector3.up * 1.8f;
        base.Use(user);
        //currentUser.LookAt(cameraTargetPoint.position);
        lookAtTV = true;
        StartCoroutine(TVOnn());
    }

    public override void StopUse()
    {
        lookAtTV = false;
        base.StopUse();
    }


    private IEnumerator TVOnn()
    {
        if (!World.knowNews)
        {
            meshRenderer.material = onLineMaterial;
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < newsDialog.Length; i++)
            {
                baseUIBehavoiur.ShowDialog(newsDialog[i]);
                yield return new WaitForSeconds(4f);
                yield return new WaitUntil(() => !World.onPause);
            }
            baseUIBehavoiur.HideDialog();
            meshRenderer.material = offLineMaterial;
            World.knowNews = true;
            if (lookAtTV)
            {
                Debug.Log("1");
                currentUser.StopUseItem();
            }
        }
        else
        {
            int addsNomber = Random.Range(0, 3);
            meshRenderer.material = onLineMaterial;
            yield return new WaitForSeconds(2f);
            baseUIBehavoiur.ShowDialog(addsDialog[addsNomber]);
            yield return new WaitForSeconds(5f);
            yield return new WaitUntil(() => !World.onPause);
            baseUIBehavoiur.HideDialog();
            meshRenderer.material = offLineMaterial;
            if (lookAtTV)
            {
                currentUser.StopUseItem();
            }
        }
    }
}
