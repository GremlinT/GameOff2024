using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
/* Скрипт фиксация ввода от игрока. 
 * 
 * Этот скрипт цепляется на управляемого персонажа - инопланетянина.
 * 
 * Он отсележивает клик левой кнопкой мышки.
 * Если происходит клик мышкой, то проверяется был ли курсор мышки наведен на объект с колладйером.
 * Если такой объект был, то проверяется:
 * - является ли такой объект поверхностью, по которой можно ходить (у него есть компонент WalkableSurface);
 *  В этом случае у управляемого персонажа запускается метод двжиения к конкретной точке, на которую был клик
 * - является ли такой объект используемым объектом (у него есть компонент UsableItem).
 *  В этом случае у управляемого персонажа запускается метод использования объекта, по которому был клик
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
