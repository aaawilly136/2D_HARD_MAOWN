using UnityEngine;
using UnityEngine.UI;

public class skillitem : MonoBehaviour
{
    [Header("技能冷卻時間")]
    public float coldTime = 2;//技能冷卻時間
    private float timer = 0; //計時器初始直
    private Image filledImage;
    private bool isStartTimer;

    private Inventory inventory;
    [SerializeField] private UI_Inventory uiInventory;

    public KeyCode keyCode;

    private void Start()
    {
        filledImage = transform.Find("技能冷卻").GetComponent<Image>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            isStartTimer = true;
        }
        if(isStartTimer)
        {
            timer += Time.deltaTime;
            filledImage.fillAmount = (coldTime - timer) / coldTime;
        }
        if(timer >= coldTime)
        {
            filledImage.fillAmount = 0;
            timer = 0;
            isStartTimer = false;
        }
    }
    public void OnClick()
    {
        isStartTimer = true;
    }
}
