using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public class DieImage
{
    public int DieNumber;
    public Sprite DiePicture;

}

[CreateAssetMenu(fileName = "Images", menuName = "ScriptableObject/Images")]
public class scr_ScriptableObject_Images : ScriptableObject
{
    public DieImage[] ImageDie;

    public Sprite tokenAdd1Image;

    public Sprite button_RollPress;
    public Sprite button_RollDePress;

    public Sprite button_RollPress_Auto;
    public Sprite button_RollDePress_Auto;

    public float Timer_for_Roll = 5.0f;

}
