using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class scr_LapReward : MonoBehaviour
{
    public RawImage rawImage; // Reference to the RawImage component
    private Color currentColor;

    [SerializeField] private scr_ScriptableObject_prizeBox prizeBox;
    [SerializeField] private scr_rollDice rollDiceUI;

    [SerializeField] private string LapPrizeName;
    [SerializeField] private string LapPrizeCode;
    [SerializeField] private ulong LapPrizeAmount;
    [SerializeField] private int LapPin;

    public string code;
    public string rarity;
    public ulong amount;

    private void Awake()
    {
        if (transform.parent != null)
        {
            if (transform.parent.parent.name.Contains("Lap_pin_1"))
            {
                LapPin = 1;
                Debug.Log("ID set to Pin 1");
            }
            else if (transform.parent.parent.name.Contains("Lap_pin_2"))
            {
                LapPin = 2;
                Debug.Log("ID set to Pin 2");
            }
            else
            {
                LapPin = 3;
                Debug.Log("ID set to Pin 3");
            }
        }

        if (prizeBox != null)
        {
            // Iterate through the LapPrizes list inside prizeBox
            foreach (LapPrize LapPrizes in prizeBox.LapPrizes)
            {
                if (LapPrizes.LapNumber == LapPin)
                {
                    name = LapPrizes.reward_Name;
                    code = LapPrizes.reward_Code;
                    rarity = LapPrizes.reward_Rating;
                    amount = LapPrizes.reward_Amount;
                    rawImage.texture = LapPrizes.reward_Texture;

                    Image parentImage = transform.parent.GetComponentInParent<Image>();
                    if (parentImage != null)
                    {
                        parentImage.sprite = LapPrizes.reward_border;
                        Debug.Log($"Parent image border set for LapPin {LapPin}");
                    }
                    else
                    {
                        Debug.LogWarning("Parent Image component not found.");
                    }
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentColor = rawImage.color;    // Get the current color of the RawImage

        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentColor == Color.white)
        {
            Debug.Log("The RawImage color is white.");
        }
        else
        {
            Debug.Log("The RawImage color is not white.");

            if (name.ToLower().Contains("coin"))
            {
                if (rollDiceUI.HoldingCoin != null)
                {
                    Debug.Log("BB-inputCoin");
                    rollDiceUI.HoldingCoin += amount;
                }

            }
            else if (name.ToLower().Contains("diamond"))
            {
                if (rollDiceUI.HoldingDiamond != null)
                {
                    Debug.Log("BB-inputDiamond");
                    rollDiceUI.HoldingDiamond += amount;
                }
            }

        }
    }
}
