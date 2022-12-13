using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public Cards cardGameObject; //Префаб карты
    public List<Cards> cardsHandList; //Карты в руке
    public List<Cards> cardsTableList; //Карты на столе
    public Transform cardsHandParent; //Родительские объекты
    public Transform cardsTableParent;

    //Создание карт в начале игры
    void Awake()
    {
        int r = Random.Range(4, 7);
        cardsHandList = new List<Cards>();
        cardsTableList = new List<Cards>();
        for (int i = 0; i < r; i++)
        {
            Cards obj = Instantiate(cardGameObject, cardsHandParent);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            cardsHandList.Add(obj);
        }
    }

    void Update()
    {
        CardsPositionUpdate();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RandomButton()
    {
        foreach (var obj in cardsHandList) {
            int r = Random.Range(0, 3);
            if (r == 0)
                obj.SetCardHP(Random.Range(-2, 10));
            if (r == 1)
                obj.SetCardAttack(Random.Range(-2, 10));
            if (r == 2)
                obj.SetCardMana(Random.Range(-2, 10));
        }
    }

    //Анимация падения карты при HP <= 0
    IEnumerator CardBreak(Cards card)
    {
        while (card.GetComponent<RectTransform>().anchoredPosition.y > -240)
        {
            card.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 15);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(card.gameObject);
    }

    //Анимирование карт на столе/в руке
    void CardsPositionUpdate()
    {
        float angle = 10f * (cardsHandList.Count - 1);
        float increment = 20f * (cardsHandList.Count - 1) / (cardsHandList.Count - 1);

        for (int i = 0; i < cardsHandList.Count; i++)
        {
            if (cardsHandList[i].GetCardHP() <= 0)
            {
                StartCoroutine(CardBreak(cardsHandList[i]));
                cardsHandList.RemoveAt(i);
                continue;
            }

            if (cardsHandList[i].isDrag)
            {
                angle -= increment;
                continue;
            }

            if (cardsHandList[i].isOnTable)
            {
                cardsHandList[i].transform.SetParent(cardsTableParent);
                cardsTableList.Add(cardsHandList[i]);
                cardsHandList.RemoveAt(i);
                continue;
            }

            Vector2 pos = new Vector2((i - ((float)cardsHandList.Count - 1) / 2) * 60, 0);
            cardsHandList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(cardsHandList[i].GetComponent<RectTransform>().anchoredPosition, pos, Time.deltaTime * 3f);

            Quaternion target = Quaternion.Euler(0, 0, angle);
            cardsHandList[i].GetComponent<RectTransform>().rotation = Quaternion.Slerp(cardsHandList[i].GetComponent<RectTransform>().rotation, target, Time.deltaTime * 3f);
            angle -= increment;
        }

        for (int i = 0; i < cardsTableList.Count; i++)
        {
            if (cardsTableList[i].isDrag)
                continue;

            Vector2 pos = new Vector2((i - ((float)cardsTableList.Count - 1) / 2) * 60, 0);
            cardsTableList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(cardsTableList[i].GetComponent<RectTransform>().anchoredPosition, pos, Time.deltaTime * 3f);

            Quaternion target = Quaternion.Euler(0, 0, 0);
            cardsTableList[i].GetComponent<RectTransform>().rotation = Quaternion.Slerp(cardsTableList[i].GetComponent<RectTransform>().rotation, target, Time.deltaTime * 3f);
        }
    }
}
