using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    [SerializeField] private Scr_createArea createArea;
    public scr_resizeBox boxProperties;
    public scr_playerMove playerMove;
    public scr_ScriptableObject_prizeBox ScriptObj_prize;
    public scr_rollDice rollDiceUI;

    private SpriteRenderer spriteRenderer;
    private Vector3 previousScale;
    private Vector2 previousSpriteSize;

    public float colorChangeDuration = 1f;
    public float moveDuration = 1.0f;

    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private bool isReturning = false;
    [SerializeField] private bool hasTriggered = false;

    public string namePrize;
    public string codePrize;
    public ulong amountPrize;
    
    private Coroutine eventFading = null;

    
    public string CodePrize
    {
        get { return codePrize; }
        set
        {
            if (codePrize != value)
            {
                codePrize = value;
                UpdateSprite();
            }
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousScale = transform.localScale;
        if (spriteRenderer.sprite != null)
        {
            previousSpriteSize = spriteRenderer.sprite.bounds.size;
        }
    }

    private void Start()
    {
        StartCoroutine(AssignPlayerMovesWithDelay(0.5f));
        SetInitialCodePrize();
        initialPosition = transform.position;
        SetZPosition(ref initialPosition);
        targetPosition = initialPosition + new Vector3(0f, 3.0f, 0f);
        //SetZPosition(ref targetPosition);
    }

    private void Update()
    {
        if (playerMove == null) return;

        HandleCodePrizeChange();

        if (playerMove.passingStart)
        {
            StartCoroutine(HandlePassingStart());
        }
    }

    private IEnumerator AssignPlayerMovesWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AssignComponent("playerPivot(Clone)", ref playerMove);
        AssignComponent("Main Camera", ref createArea);
        AssignComponent("Canvas_UI", ref rollDiceUI);
    }

    private void AssignComponent<T>(string objectName, ref T component) where T : Component
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{typeof(T).Name} component not found on {objectName}.");
            }
        }
        else
        {
            Debug.LogError($"Instantiated GameObject with name '{objectName}' not found in the scene!");
        }
    }

    private void HandleCodePrizeChange()
    {
        if (boxProperties.start_bends == 1)
        {
            codePrize = playerMove.changeStartIcon ? ScriptObj_prize.codeStartAfter : ScriptObj_prize.codeStartInit;
            Debug.Log("codePrize: " + codePrize);
        }
    }

    private IEnumerator HandlePassingStart()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        StopCoroutine(eventFading);
        spriteRenderer.color = Color.white;
        SetZPosition(ref initialPosition);
        transform.position = initialPosition;
        SetCodePrize();
        isReturning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerMove != null && playerMove.normalizePlayer && collision.CompareTag("Player") && !isReturning)
        {
            if (!hasTriggered)
            {
                hasTriggered = true;
                eventFading = StartCoroutine(ChangeColorAndMove(targetPosition));

                if (namePrize.ToLower().Contains("coin"))
                {
                    if (rollDiceUI.HoldingCoin != null)
                    {
                        Debug.Log("BB-inputCoin");
                        rollDiceUI.HoldingCoin += amountPrize;
                    }

                }
                else if (namePrize.ToLower().Contains("diamond"))
                {
                    if (rollDiceUI.HoldingDiamond != null)
                    {
                        Debug.Log("BB-inputDiamond");
                        rollDiceUI.HoldingDiamond += amountPrize;
                    }
                }
            }
            
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerMove != null && playerMove.normalizePlayer && collision.CompareTag("Player"))
        {
            hasTriggered = false;
            isReturning = false;
        }
    }

    private IEnumerator ChangeColorAndMove(Vector3 target)
    {
        isReturning = true;

        Color startColor = spriteRenderer.color;
        Color endColor = new Color(0.6f, 0.6f, 0.6f, 0.01f);

        Vector3 startPosition = transform.position;

        //SetZPosition(ref startPosition);
        //SetZPosition(ref target);

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor;
        //transform.position = target;
        //transform.position = startPosition;

        //isReturning = false;
    }

    private void SetInitialCodePrize()
    {
        CodePrize = boxProperties.start_bends switch
        {
            //1 => "E1",
            1 => "L" + Random.Range(1, 4).ToString(),
            2 or 3 => "U" + Random.Range(1, 4).ToString(),
            4 => "R" + Random.Range(1, 4).ToString(),
            _ => "C" + Random.Range(1, 4).ToString(),
        };
    }

    private void SetCodePrize()
    {
        CodePrize = boxProperties.start_bends switch
        {
            1 => "L" + Random.Range(1, 4).ToString(),
            2 or 3 => "U" + Random.Range(1, 4).ToString(),
            4 => "R" + Random.Range(1, 4).ToString(),
            _ => "C" + Random.Range(1, 4).ToString(),
        };
    }

    private void UpdateSprite()
    {
        foreach (var prize in ScriptObj_prize.PrizeItem)
        {
            if (prize.Code == CodePrize)
            {
                namePrize = prize.Name;
                amountPrize = prize.AmountPrize;

                spriteRenderer.sprite = prize.picture;
                Vector2 newSpriteSize = prize.picture.bounds.size;

                if (previousSpriteSize != Vector2.zero)
                {
                    Vector3 newScale = new Vector3(
                        previousScale.x * (previousSpriteSize.x / newSpriteSize.x),
                        previousScale.y * (previousSpriteSize.y / newSpriteSize.y),
                        previousScale.z
                    );
                    transform.localScale = newScale;
                }
                spriteRenderer.color = Color.white;
                break;
            }
        }
    }

    private void SetZPosition(ref Vector3 position)
    {
        position.z = -10.0f;
    }


}
