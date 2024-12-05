using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class CircleMinigame : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;

    public static UnityAction<Item, GameObject> OnItemReceived;
    public Item item;

    [SerializeField] private Image fixedCircle;
    [SerializeField] private Image shrinkingCircle;

    [SerializeField] private bool isShrinkingActive;

    [SerializeField] private float shrinkingSpeed = 1f;

    private Vector3 initSize;

    private float shrinkingRadius;
    private float innerRadius;

    private const float borderProportion = 0.27f;
    private const float borderFixedProportion = 0.27f;
    private const float minRadius = 0.1f;

    [Header("Game State")]
    [SerializeField] private int gameCount = 0;         // Number of minigames played
    private int itemsMined = 0;
    private GameObject corespondingGO;

    private void OnEnable() { OnItemReceived += ReceiveItem; }
    private void OnDisable() { OnItemReceived -= ReceiveItem; }

    void ReceiveItem(Item _item, GameObject go)
    {
        item = _item;
        corespondingGO = go;
        StartMinigame();
    }

    private void Start()
    {
        gameUI.SetActive(false);
        initSize = shrinkingCircle.rectTransform.sizeDelta;
    }

    void Update()
    {
        if (!isShrinkingActive) return;

        Shrink();
        ShrinkingCircleCalc();
        CheckMinigameState();
    }

    void StartMinigame()
    {
        isShrinkingActive = true;
        gameCount = 0;
        itemsMined = 0;
        fixedCircle.color = Color.white;
        gameUI.SetActive(isShrinkingActive);
    }

    private void CheckMinigameState()
    {
        if (gameCount >= 3) EndGame();

        float fixedRadius = fixedCircle.rectTransform.sizeDelta.x * fixedCircle.rectTransform.localScale.x / 2;
        float fixedInnerRadius = FixedCircleCalc();
        if (shrinkingRadius < fixedInnerRadius || innerRadius > fixedRadius)
        {
            fixedCircle.color = Color.yellow;
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Debug.Log("fail");
                GameRestart();
            }
        }
        else if (innerRadius <= fixedRadius)
        {
            fixedCircle.color = Color.green;
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Debug.Log("success");

                itemsMined++;
                GameRestart();
                return;
            }
        }

    }

    void ShrinkingCircleCalc()
    {
        float width = shrinkingCircle.rectTransform.sizeDelta.x * shrinkingCircle.rectTransform.localScale.x;
        float height = shrinkingCircle.rectTransform.sizeDelta.y * shrinkingCircle.rectTransform.localScale.y;

        shrinkingRadius = width / 2;

        float borderWidth = shrinkingRadius * borderProportion;
        innerRadius = shrinkingRadius - borderWidth;
    }

    float FixedCircleCalc()
    {
        float fixedWidth = fixedCircle.rectTransform.sizeDelta.x * fixedCircle.rectTransform.localScale.x;
        float fixedHeight = fixedCircle.rectTransform.sizeDelta.y * fixedCircle.rectTransform.localScale.y;

        float fixedOuterRadius = fixedWidth / 2;

        float fixedBorderWidth = fixedOuterRadius * borderFixedProportion;

        float fixedInnerRadius = fixedOuterRadius - fixedBorderWidth;
        return fixedInnerRadius;
    }

    void Shrink()
    {
        if (!isShrinkingActive) return;

        if (shrinkingCircle.rectTransform.sizeDelta.x > minRadius && shrinkingCircle.rectTransform.sizeDelta.y > minRadius)
        {
            shrinkingCircle.rectTransform.sizeDelta = new Vector2(
                shrinkingCircle.rectTransform.sizeDelta.x - shrinkingSpeed * Time.deltaTime,
                shrinkingCircle.rectTransform.sizeDelta.y - shrinkingSpeed * Time.deltaTime
            );
        }
        else
        {
            GameRestart();
        }
    }

    void GameRestart()
    {
        shrinkingCircle.rectTransform.sizeDelta = initSize;
        gameCount++;
    }

    void EndGame()
    {
        isShrinkingActive = false;
        gameUI.SetActive(isShrinkingActive);
        CircleMinigameHandler.OnMinigameInteracted?.Invoke(itemsMined, item, corespondingGO.transform);
        Destroy(corespondingGO);
        itemsMined = 0;
        item = null;
    }
}
