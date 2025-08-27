using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class scr_rollDice : MonoBehaviour
{
    [SerializeField] private scr_ScriptableObject_Images SO_Image;
    [SerializeField] private scr_ScriptableObject_Data SO_Data;
    [SerializeField] private scr_ScriptableObject_Audio SO_Audio;
    [SerializeField] private scr_ScriptableObject_prizeBox SO_Prize;
    [SerializeField] private Scr_createArea CreateAreas;
    [SerializeField] private scr_playerMove playerMoves;

    [SerializeField] private TMP_Text Text_DiceRoll1;
    [SerializeField] private TMP_Text Text_DiceRoll2;
    [SerializeField] private TMP_Text text_laps;
    [SerializeField] private TMP_Text Text_token;
    [SerializeField] private TMP_Text Text_tokenAdd1;
    [SerializeField] private TMP_Text Text_oneMoreTime;

    [SerializeField] private TMP_Text Text_Diamond;
    [SerializeField] private TMP_Text Text_coin;

    [SerializeField] private Button buttonRoll;

    [SerializeField] private Image FG_ProgressBar;
    [SerializeField] private Image Die1_IMG;
    [SerializeField] private Image Die2_IMG;
    [SerializeField] private Image buttonRollIMG;
    [SerializeField] private Image TimerIMG;

    [SerializeField] private bool ManualRoll = true; // True: Manual ; Auto: false 

    public bool button_RollPress = false;

    [SerializeField] private bool trigger1x_001 = false;    // for rolling dice
    [SerializeField] private bool trigger1x_002 = false;    // for AutoRolls  
    [SerializeField] private bool trigger1x_003 = false;    // for timerEnable
    [SerializeField] private bool timerEnable = false;

    [SerializeField] private bool oneMoreTime = false;      // bonus token toggle if #die1 = #die2 (1 1 , 2 2 , 3 3 , 4 4 , 5 5 , 6 6)
    [SerializeField] private bool testDieNumber;

    [SerializeField] public int dieRolls1 = 0;
    [SerializeField] public int dieRolls2 = 0;
    [SerializeField] public int diceRolls = 0;

    
    public bool dieRollsEnd1 = false;
    public bool dieRollsEnd2 = false;

    public float dieRolls_Delay = 0.1f;

    public int minDie = 1;
    public int maxDie = 6;

    private Coroutine rollCoroutine1; // To store the reference to the coroutine
    private Coroutine rollCoroutine2; // To store the reference to the coroutine
    private Coroutine timerCoroutine = null;

    public float initialDelay = 0.01f;   // Initial delay in seconds
    public float maxDelay = 0.50f;         // Maximum delay in seconds
    public float delayIncrease = 0.01f;  // Amount to increase the delay by

    private int[] numbers = { 1, 2, 3, 4, 5, 6 };
    private int currentIndex = 0;
    private float currentDelay=0.05f;
    private Coroutine RollAutoCoroutine;

    public GameObject LapPrize1;
    public GameObject LapPrize2;
    public GameObject LapPrize3;

    public ulong HoldingCoin = 0;
    public ulong HoldingDiamond = 0;
    public ulong HoldingTiketToken = 0;

    [SerializeField] private int i = 0;

    void ReSetState()
    {
        button_RollPress = false;
        trigger1x_001 = false;

       //button_RollStop = false;
        trigger1x_002 = false;
        

        dieRolls1 = 0;
        dieRolls2 = 0;
        diceRolls = 0;

        dieRollsEnd1 = false;
        dieRollsEnd2 = false;

        timerEnable = false;
        trigger1x_003 = false;


    }

    private void Awake()
    {
        Text_DiceRoll1 = GetTMP_TextComponent("die_Text1"); Debug.Log(Text_DiceRoll1 != null ? "Text_DiceRoll1 found" : "Text_DiceRoll1 not found");
        Text_DiceRoll2 = GetTMP_TextComponent("die_Text2"); Debug.Log(Text_DiceRoll2 != null ? "Text_DiceRoll2 found" : "Text_DiceRoll2 not found");

        text_laps = GetTMP_TextComponent("Laps_Text");      Debug.Log(text_laps != null ? "text_laps found" : "text_laps not found");
        Text_token = GetTMP_TextComponent("Token_Text");    Debug.Log(Text_token != null ? "Text_token found" : "Text_token not found");
        Text_Diamond = GetTMP_TextComponent("Diamond_Text");
        Text_coin = GetTMP_TextComponent("Coin_Text");

        Text_tokenAdd1 = GetTMP_TextComponent("Token+1");   Debug.Log(Text_tokenAdd1 != null ? "Text_tokenAdd1 found" : "Text_tokenAdd1 not found");
     
        Text_oneMoreTime = GetTMP_TextComponent("OneMoreTime_Text");

      
        buttonRoll = FindDeepChild<Button>(GameObject.Find("Button"), "Button_Roll");
        if (buttonRoll != null)
        {
            Debug.Log("Button_Roll found");

            //buttonRollIMG = buttonRoll.GetComponent<Image>();

            //buttonRollIMG = GetImageComponent("Button_Roll");
            //Debug.Log(buttonRollIMG != null ? "buttonRollIMG found" : "buttonRollIMG not found");
        }
        else
        {
            Debug.Log("Button_Roll not found");
        }

        //buttonRollIMG = GetImageComponent("Button_Roll");

        CreateAreas = GameObject.Find("Main Camera").GetComponent<Scr_createArea>();

        ReSetState();
    }

    private T GetComponentFromGameObjectByName<T>(string gameObjectName) where T : Component
    {
        GameObject foundObject = GameObject.Find(gameObjectName);
        if (foundObject != null)
        {
            T component = foundObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            else
            {
                Debug.LogError($"Component of type {typeof(T)} not found on GameObject '{gameObjectName}'.");
                return null;
            }
        }
        else
        {
            Debug.LogError($"GameObject with name '{gameObjectName}' not found in Awake.");
            return null;
        }
    }

    private TMP_Text GetTMP_TextComponent(string name)
    {
        TMP_Text[] textComponents = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text textComponent in textComponents)
        {
            if (textComponent.gameObject.name == name)
            {
                return textComponent;
            }
        }
        Debug.LogError($"TMP_Text component with name '{name}' not found in Awake.");
        return null;
    }

    private Image GetImageComponent(string name)
    {
        Transform imageTransform = transform.Find(name);
        if (imageTransform != null)
        {
            return imageTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogError($"Image component with name '{name}' not found in Awake.");
            return null;
        }
    }

    public T FindDeepChild<T>(GameObject parent, string name) where T : Component
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == name)
            {
                T component = child.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            T found = FindDeepChild<T>(child.gameObject, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void SetRawImageToGrayscale(GameObject lapPrize)
    {
        RawImage rawImage = lapPrize.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            // Set the color to grayscale
            rawImage.color = Color.gray;
            Debug.Log($"{lapPrize.name} RawImage set to grayscale");
        }
        else
        {
            Debug.LogError($"{lapPrize.name} does not have a RawImage component as a child");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        timerEnable = false;
        trigger1x_003 = false;

        buttonRoll.interactable = true;
        trigger1x_001 = false;

        
        trigger1x_002 = false;

        currentDelay = initialDelay;
        
        if (CreateAreas.playerCreated)
        {
            playerMoves = GetComponentFromGameObjectByName<scr_playerMove>("playerPivot(Clone)");
            if (playerMoves != null)
            {
                Debug.Log("PLR-playerMoves found and assigned in Start.");
            }

        }

        HoldingDiamond = SO_Data.Token_DiamondAmount;
        HoldingCoin = SO_Data.Token_CoinAmount;
        HoldingTiketToken = SO_Data.Token_TiketAmount;

        Text_Diamond.text = ConvertingNumbers(HoldingDiamond).ToString();
        Text_coin.text = ConvertingNumbers(HoldingCoin).ToString();
        Text_token.text = ConvertingNumbers(HoldingTiketToken).ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (ManualRoll)
        {
            if (button_RollPress)
            {
                buttonRollIMG.sprite = SO_Image.button_RollPress;
            }
            else
            {
                buttonRollIMG.sprite = SO_Image.button_RollDePress;
            }
        }
        else
        {
            if (button_RollPress)
            {
                buttonRollIMG.sprite = SO_Image.button_RollPress_Auto;
            }

            else
            {
                buttonRollIMG.sprite = SO_Image.button_RollDePress;
            }
        }

        if (playerMoves == null)
        {
            playerMoves = GetComponentFromGameObjectByName<scr_playerMove>("playerPivot(Clone)");
            if (playerMoves != null)
            {
                Debug.Log("PLR-playerMoves found and assigned in Update.");
            }

        }

        if (playerMoves != null)
        {
            
            FG_ProgressBar.fillAmount = Mathf.Clamp01(playerMoves.Laps / 20.0f);

            text_laps.text = "LAPS: " + playerMoves.lastPlace + " / " + playerMoves.totalBoxCreate;
            Text_token.text = HoldingTiketToken.ToString();

            // Check if player has completed at least 5 laps
            if (playerMoves.Laps >= 5)
            {
                // Ensure LapPrize1 has an Image component and assign the sprite
                Image img1 = LapPrize1.GetComponent<Image>();
                if (img1 != null)
                {
                    img1.sprite = SO_Prize.ItemGet_BG;
                    Debug.Log("LapPrize1 image updated");
                }
                else
                {
                    Debug.LogError("LapPrize1 does not have an Image component");
                }

                // Set the RawImage child of LapPrize1 to grayscale
                SetRawImageToGrayscale(LapPrize1);
            }

            // Check if player has completed at least 10 laps
            if (playerMoves.Laps >= 10)
            {
                // Ensure LapPrize2 has an Image component and assign the sprite
                Image img2 = LapPrize2.GetComponent<Image>();
                if (img2 != null)
                {
                    img2.sprite = SO_Prize.ItemGet_BG;
                    Debug.Log("LapPrize2 image updated");
                }
                else
                {
                    Debug.LogError("LapPrize2 does not have an Image component");
                }

                // Set the RawImage child of LapPrize1 to grayscale
                SetRawImageToGrayscale(LapPrize2);
            }

            // Check if player has completed at least 15 laps
            if (playerMoves.Laps >= 15)
            {
                // Ensure LapPrize3 has an Image component and assign the sprite
                Image img3 = LapPrize3.GetComponent<Image>();
                if (img3 != null)
                {
                    img3.sprite = SO_Prize.ItemGet_BG;
                    Debug.Log("LapPrize3 image updated");
                }
                else
                {
                    Debug.LogError("LapPrize3 does not have an Image component");


                    // Set the RawImage child of LapPrize1 to grayscale
                    SetRawImageToGrayscale(LapPrize3);
                }

            }
        }

        if (HoldingTiketToken > 0)
        {
            if (!timerEnable && !trigger1x_003) 
            {
                trigger1x_003 = true;
                

                StartCoroutine(activateTimer(SO_Prize.Delay_PressButton));

            }

            if (!ManualRoll)
            {
                if (i == 5 && !trigger1x_002)
                {
                    trigger1x_002 = true;
                    StartCoroutine(AutoRun());
                    
                }
            }


            if (button_RollPress || timerEnable)
            {
                if (!trigger1x_001)
                {
                    //buttonRoll.interactable = false;
                    
                    trigger1x_001 = true;

                    dieRollsEnd1 = false;
                    dieRollsEnd2 = false;

                    rollCoroutine1 = StartCoroutine(rollDice1());
                    rollCoroutine2 = StartCoroutine(rollDice2());

                    oneMoreTime = false;

                    StartCoroutine(RotateNumbers());

                    StopRollCoroutine1();
                    StopRollCoroutine2();
                }
                
            }

            if (i == 4)
            {
                if (dieRolls1 == dieRolls2 && !oneMoreTime)
                {
                    oneMoreTime = true;                 // one more time part

                    StartCoroutine(One_MoreTime());
                    StartCoroutine(OneMOreTime_Text());
                }
                else            // non-one more time part
                {
                    StopCoroutine(RotateNumbers());
                    StopCoroutine(One_MoreTime());
                    StopCoroutine(OneMOreTime_Text());

                    playerMoves.player_dieRun = true;
                    
                    dieRollsEnd1 = true;
                    dieRollsEnd2 = true;
                    
                    trigger1x_001 = false;

                    //buttonRoll.interactable = true;
                    button_RollPress = false;

                    trigger1x_002 = false;

                    trigger1x_003 = false;
                    //timerEnable = false;
                    
                    i++;    //i = 5;

                }
            }

            

        }

        if (playerMoves.lastPlace == playerMoves.count_position)
        {
            if (!oneMoreTime)
            {
                buttonRoll.interactable = true;
                
                //trigger1x_003 = false;
                timerEnable = false;
            }
            
        }
        else
        {
            buttonRoll.interactable = false;
        }

        Text_Diamond.text = ConvertingNumbers(HoldingDiamond).ToString();
        Text_coin.text = ConvertingNumbers(HoldingCoin).ToString();
        Text_token.text = ConvertingNumbers(HoldingTiketToken).ToString();

    }

    IEnumerator AutoRun()
    {
        Debug.Log("tried to roll again");

        yield return new WaitForSecondsRealtime(1.0f);

        trigger1x_001 = false;
        button_RollPress = true;
        HoldingTiketToken--;

        yield return null;
    }

    private IEnumerator One_MoreTime()
    {
        StopCoroutine(RotateNumbers());

        playerMoves.player_dieRun = true;

        dieRollsEnd1 = true;
        dieRollsEnd2 = true;

        trigger1x_001 = false;

        button_RollPress = false;

        i++;

        yield return new WaitForSecondsRealtime(1.0f);

        //buttonRoll.interactable = false;
        button_RollPress = true;

    }

    private void StopRollCoroutine1()
    {
        if (rollCoroutine1 != null)
        {
            StopCoroutine(rollCoroutine1);
            rollCoroutine1 = null;
        }
    }

    private void StopRollCoroutine2()
    {
        if (rollCoroutine2 != null)
        {
            StopCoroutine(rollCoroutine2);
            rollCoroutine2 = null;
        }
    }

    IEnumerator rollDice1()
    {

        dieRolls1 = Random.Range(minDie, maxDie + 1);  // Fixed assignment for random roll
                                                       //dieRolls1 = Random.Range(1, 3);  // TEST Fixed assignment for random roll

        yield return null;
    }

    IEnumerator rollDice2()
    {

        dieRolls2 = Random.Range(minDie, maxDie + 1);  // Fixed assignment for random roll
                                                       //dieRolls2 = Random.Range(1, 3);  // TEST Fixed assignment for random roll

        yield return null;
    }

    public void ChangeDieSprite(Image dieImage, int dieNumber)
    {
        // Store the original scale of the Image component
        Vector3 originalScale = dieImage.rectTransform.localScale;

        foreach (var dieImageItem in SO_Image.ImageDie)
        {
            if (dieImageItem.DieNumber == dieNumber)
            {
                // Change the sprite of the Image component
                dieImage.sprite = dieImageItem.DiePicture;
                // Reapply the original scale
                dieImage.rectTransform.localScale = originalScale;
                return;
            }
        }
        Debug.LogWarning("Die number not found in the list.");
    }

    private IEnumerator AssignPlayerMovesWithDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Try to find the instantiated playerPivot GameObject in the scene
        GameObject playerPivot = GameObject.Find("playerPivot(Clone)"); // Assuming it's named "playerPivot(Clone)" after instantiation
        if (playerPivot != null)
        {
            playerMoves = playerPivot.GetComponent<scr_playerMove>();
            if (playerMoves != null)
            {
                Debug.Log("playerMoves found and assigned after delay.");
            }
            else
            {
                Debug.LogError("scr_playerMove component not found on playerPivot.");
            }
        }
        else
        {
            Debug.LogError("Instantiated GameObject with name 'playerPivot' not found in the scene!");
        }
    }

    IEnumerator RotateNumbers()
    {
        currentDelay = initialDelay;

        while (currentDelay < maxDelay)
        {

            //Text_DiceRoll1.text = numbers[currentIndex].ToString();               // Display the current number die 1
            //ChangeDieSprite(Die1_IMG, numbers[currentIndex]);

            Text_DiceRoll1.text = Random.Range(minDie, maxDie + 1).ToString();      // randon dice show
            ChangeDieSprite(Die1_IMG, int.Parse(Text_DiceRoll1.text));

            //Text_DiceRoll2.text = numbers[currentIndex].ToString();               // Display the current number die 2
            //ChangeDieSprite(Die2_IMG, numbers[currentIndex]);

            Text_DiceRoll2.text = Random.Range(minDie, maxDie + 1).ToString();      //random dice show
            ChangeDieSprite(Die2_IMG, int.Parse(Text_DiceRoll2.text));

            // Wait for the current delay
            yield return new WaitForSeconds(currentDelay - Random.Range(0f, 0.04f));

            // Move to the next number in the cycle
            currentIndex = (currentIndex + 1) % numbers.Length;

            // Increase the delay
            currentDelay += delayIncrease;
        }

        i = 0;
        // Continue with the maximum delay
        while (i < 3 )
        {
            //numberText.text = numbers[currentIndex].ToString();
            Text_DiceRoll1.text = numbers[currentIndex].ToString();
            ChangeDieSprite(Die1_IMG, numbers[currentIndex]);

            yield return new WaitForSeconds(maxDelay);

            Text_DiceRoll2.text = numbers[currentIndex].ToString();
            ChangeDieSprite(Die2_IMG, numbers[currentIndex]);

            currentIndex = (currentIndex + 1) % numbers.Length;

            i++;
        }

        i = 3;

        i++;

        Text_DiceRoll1.text = dieRolls1.ToString();
        ChangeDieSprite(Die1_IMG, dieRolls1);

        Text_DiceRoll2.text = dieRolls2.ToString();
        ChangeDieSprite(Die2_IMG, dieRolls2);

        yield return null;

    }

    public void buttonRoll_pressed()
    {
        if (HoldingTiketToken > 0)
        {
            //buttonRoll.interactable = false;
            button_RollPress = true;

            //buttonRollStop.interactable = true;
            //button_RollStop = false;


            HoldingTiketToken--;
        }
        
    }

   
    IEnumerator activateTimer(float timerDuration)
    {

        //float duration = 5.0f; // Duration in seconds
        float elapsedTime = 0.0f;

        
        yield return new WaitForSecondsRealtime(0.5f);

        while (elapsedTime < timerDuration)
        {
            if (!button_RollPress && !oneMoreTime)
            {
                TimerIMG.fillAmount = Mathf.Lerp(0, 1, elapsedTime / timerDuration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                break;
            }

        }

        // Ensure the fill amount is set to 1 at the end of the duration if button_RollPress is not pressed
        if (!button_RollPress && !oneMoreTime)
        {
            TimerIMG.fillAmount = 1;

            HoldingTiketToken--;
            timerEnable = true;

        }

        StopCoroutine(timerCoroutine);
        timerCoroutine = null;

    }

    IEnumerator OneMOreTime_Text()
    {
        while (oneMoreTime)
        {
            // Fade out
            while (Text_oneMoreTime.color.a > 0.1f)
            {
                Text_oneMoreTime.color = new Color(Text_oneMoreTime.color.r, Text_oneMoreTime.color.g, Text_oneMoreTime.color.b, Mathf.MoveTowards(Text_oneMoreTime.color.a, 0.1f, Time.deltaTime * 0.5f));
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.10f);

            // Fade in
            while (Text_oneMoreTime.color.a < 1.0f)
            {
                Text_oneMoreTime.color = new Color(Text_oneMoreTime.color.r, Text_oneMoreTime.color.g, Text_oneMoreTime.color.b, Mathf.MoveTowards(Text_oneMoreTime.color.a, 1.0f, Time.deltaTime * 0.5f));
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.10f);
        }

        // Set opacity to 0.01f when oneMoreTime is false
        Text_oneMoreTime.color = new Color(Text_oneMoreTime.color.r, Text_oneMoreTime.color.g, Text_oneMoreTime.color.b, 0.01f);

        yield return null;
    }

    // ----------- universal wait time -------------------------
    IEnumerator waitforRT(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
    }

    IEnumerator waitforNotRT(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    //------------------ universal converting numbers ----------------
    private string ConvertingNumbers(float numbers)
    {

        if (numbers >= Mathf.Pow(10, 18))
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 18) * 100) / 100).ToString("F2") + " AB";
        }
        else if (numbers >= Mathf.Pow(10, 15))   
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 15) * 100) / 100).ToString("F2") + " AA";
        }
        else if (numbers >= Mathf.Pow(10, 12))   // Check if the number is greater than or equal to 1 trillion
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 12) * 100) / 100).ToString("F2") + " T";     // Convert number to trillions and format with 2 decimal places
        }
        else if (numbers >= Mathf.Pow(10, 9))
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 9) * 100) / 100).ToString("F2") + " B";
        }
        else if (numbers >= Mathf.Pow(10, 6))
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 6) * 100) / 100).ToString("F2") + " M";
        }
        else if (numbers >= Mathf.Pow(10, 3))
        {
            return (Mathf.Floor(numbers / Mathf.Pow(10, 3) * 100) / 100).ToString("F2") + " K";
        }
        else
        {
            return Mathf.Floor(numbers).ToString();     // Return the number as a string without decimal places
        }
    }


}