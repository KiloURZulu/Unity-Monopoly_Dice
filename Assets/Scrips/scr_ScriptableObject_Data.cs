using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Data")]
public class scr_ScriptableObject_Data : ScriptableObject
{

    public GameObject boxPrefab;
    public GameObject playerIconPrefab;

    public int X_AreaLength;
    public int Y_AreaLength;
    public float spacingX_Dir = 0.50f;
    public float spacingY_Dir = 0.50f;

    //------die rolls--

    public bool manualRolls = false;    // deafault AutoRolls
    public int minDieRolls = 1;
    public int maxDieRolls = 6;

    public float Player_move_Spd = 6;

    public ulong Token_DiamondAmount = 0;
    public ulong Token_CoinAmount = 0;
    public ulong Token_TiketAmount = 999;

}
