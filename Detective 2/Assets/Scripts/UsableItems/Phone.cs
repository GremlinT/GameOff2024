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
        "Добрый день. Детектив зеленый человечек?",
        "Да, это я. Слушаю Вас.",
        "Вы возможно слышали, исчез один очень важный человечек. Я хочу Вас нанять для его поиска.",
        "Вы не про Дедушку?",
        "Да, про него. Вам уже что-то известно?",
        "Только то, что говорили в новостях.",
        "Хорошо. Вы возьметесь?",
        "Конечно.",
        "Замечательно, прилетайте к нам на станцию, я расскажу Вам детали.",
        "Принято. Вылетаю к Вам"
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
