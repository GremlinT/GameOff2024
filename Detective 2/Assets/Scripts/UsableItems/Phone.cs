using System.Collections;
using UnityEngine;

public class Phone : UsableItem
{
    private bool firstCall;

    private void Start()
    {
        firstCall = true;
    }

    private string[] firstCallDialog =
        {
        "������ ����. �������� ������� ���������?",
        "��, ��� �. ������ ���.",
        "�� �������� �������, ����� ���� ����� ������ ���������. � ���� ��� ������ ��� ��� ������.",
        "�� �� ��� �������?",
        "��, ��� ����. ��� ��� ���-�� ��������?",
        "������ ��, ��� �������� � ��������.",
        "������. �� ����������?",
        "�������.",
        "������������, ���������� � ��� �� �������, � �������� ��� ������.",
        "�������. ������� � ���"
        };

    public override void UseIndividual()
    {
        StartCoroutine(PhoneTalk());
    }

    public override void StopUse()
    {
        currentUser = null;
        parentItem.StopUseChild();
    }

    private IEnumerator PhoneTalk()
    {
        if (firstCall)
        {
            //meshRenderer.material = onLineMaterial;
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < firstCallDialog.Length; i++)
            {
                baseUIBehavoiur.ShowDialog(firstCallDialog[i]);
                yield return new WaitForSeconds(4f);
            }
            baseUIBehavoiur.HideDialog();
            //meshRenderer.material = offLineMaterial;
            firstCall = true;
            StopUse();
        }
        else
        {
            Debug.Log("no more calls");
            StopUse();
        }
    }
}
