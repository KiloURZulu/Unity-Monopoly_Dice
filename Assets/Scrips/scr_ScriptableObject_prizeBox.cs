using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public class Prize
{
    public string prizeName;
    public int prizeNumber;
    public string prizeActualCode;
    public string prizeRarityLevel;
    public string prizeRarityCode;
}

[System.Serializable] public class ItemRating
{
    public string Rating;
    public string RatingCode;
    public int Number;
    public Sprite RatingFrame;
}

[System.Serializable] public class PrizeItem
{
    public string Name;
    public string Code;
    public Sprite picture;
    public ulong AmountPrize;
}

[System.Serializable] public class PrizeforLocation
{
    public int startBendsCode;
    public string PrizeCode;
    public string BgCode;
}

[System.Serializable] public class LapPrize
{
    public int LapNumber;
    public int LapTarget;
    public string reward_Rating;
    public string reward_Name;
    public string reward_Code;
    public ulong reward_Amount;
    public Sprite reward_Image;
    public Texture reward_Texture;
    public Sprite reward_border;
    
}


[CreateAssetMenu(fileName = "prizes", menuName = "ScriptableObject/Prizes")]
public class scr_ScriptableObject_prizeBox : ScriptableObject
{
    public Prize[] prizes;
    public ItemRating[] ItemRating;
    public PrizeItem[] PrizeItem;
    public PrizeforLocation[] prizeforLocations;
    public LapPrize[] LapPrizes;

    public Sprite ItemGet_BG;

    public Sprite button_press;
    public Sprite button_UnPress;

    public float Delay_PressButton;

    public string codeStartInit;
    public string codeStartAfter;

}
