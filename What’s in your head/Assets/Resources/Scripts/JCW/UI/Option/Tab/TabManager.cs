using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabManager : MonoBehaviour, IPointerEnterHandler
{
    //private readonly List<GameObject> tabs = new();
    private readonly Dictionary<GameObject, Button> tabs = new();
    public static TabManager Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else if (Instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        for (int i = 0 ; i < transform.childCount ; ++i)
        {
            GameObject obj = this.transform.GetChild(i).gameObject;
            tabs.Add(obj, obj.GetComponent<Button>());
        }
    }
    

    public void ClickTab(Button _button)
    {
        foreach (GameObject obj in tabs.Keys)
        {
            // 눌려진 탭을 꺼주기
            if (tabs[obj] == _button)
            {
                _button.interactable = false;
                continue;
            }
            // 그 외는 켜주고, 내용물도 숨기기
            tabs[obj].interactable = true;
            tabs[obj].gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
    }
}
