using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cards : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image artImage;

    public Text m_HPText;
    public Text m_AttackText;
    public Text m_ManaText;

    int m_HP;
    int m_Attack;
    int m_Mana;

    int m_UpdateHP;
    int m_UpdateAttack;
    int m_UpdateMana;

    float m_HPTimer;
    float m_AttackTimer;
    float m_ManaTimer;

    public bool isDrag; //Карта перетаскивается
    public bool isOnTable; //Карта на столе?
    GameObject m_Table;
    Vector2 lastMousePosition;

    void Awake()
    {
        m_Table = GameObject.Find("Table");

        m_UpdateHP = m_HP = Random.Range(1, 10);
        m_UpdateAttack = m_Attack = Random.Range(1, 10);
        m_UpdateMana = m_Mana = Random.Range(1, 10);
        TextUpdater();

        StartCoroutine(GetTexture());
    }

    //Загрузка изображения с сайта
    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://picsum.photos/100/100");
        yield return www.SendWebRequest();

        Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
        artImage.sprite = Sprite.Create(myTexture, new Rect(0, 0, 100, 100), Vector2.zero);
    }

    //Анимации счетчиков
    void Update()
    {
        if (m_HPTimer > 0) m_HPTimer -= Time.deltaTime;
        if (m_AttackTimer > 0) m_AttackTimer -= Time.deltaTime;
        if (m_ManaTimer > 0) m_ManaTimer -= Time.deltaTime;

        if (m_HP < m_UpdateHP & m_HPTimer <= 0) { m_HP++; m_HPTimer = 0.5f; }
        if (m_HP > m_UpdateHP & m_HPTimer <= 0) { m_HP--; m_HPTimer = 0.5f; }

        if (m_Attack < m_UpdateAttack & m_AttackTimer <= 0) { m_Attack++; m_AttackTimer = 0.5f; }
        if (m_Attack > m_UpdateAttack & m_AttackTimer <= 0) { m_Attack--; m_AttackTimer = 0.5f; }

        if (m_Mana < m_UpdateMana & m_ManaTimer <= 0) { m_Mana++; m_ManaTimer = 0.5f; }
        if (m_Mana > m_UpdateMana & m_ManaTimer <= 0) { m_Mana--; m_ManaTimer = 0.5f; }

        TextUpdater();
    }

    //Обновление интерфейса
    void TextUpdater()
    {
        if (int.Parse(m_HPText.text) != m_HP)
            m_HPText.text = m_HP.ToString();
        if (int.Parse(m_AttackText.text) != m_Attack)
            m_AttackText.text = m_Attack.ToString();
        if (int.Parse(m_ManaText.text) != m_Mana)
            m_ManaText.text = m_Mana.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_HP > 0)
        {
            isDrag = true;
            lastMousePosition = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_HP > 0)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;
            Vector2 newPosition = GetComponent<RectTransform>().position + new Vector3(diff.x, diff.y);
            GetComponent<RectTransform>().position = newPosition;

            lastMousePosition = currentMousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        if (m_HP > 0 & m_UpdateHP > 0)
        {
            if (GetRaycastTable())
                isOnTable = true;
        }

    }

    //Проверка нахождения на столе
    bool GetRaycastTable()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        for (int i = 0; i < raysastResults.Count; i++)
        {
            if (raysastResults[i].gameObject == m_Table)
                return true;
        }
        return false;
    }

    public void SetCardHP(int value)
    {
        m_UpdateHP = value;
    }

    public void SetCardAttack(int value)
    {
        m_UpdateAttack = value;
    }

    public void SetCardMana(int value)
    {
        m_UpdateMana = value;
    }

    public int GetCardHP()
    {
        return m_HP;
    }
}
