using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public class AudioSFX
{
    public string SFX_Name;
    public string SFX_Code;
    public AudioClip SFX_Audio;
    
    public Sprite SFX_Image_Before;
    public Sprite SFX_Image_After;

}

[System.Serializable] public class AudioBGM
{
    public string BGM_Name;
    public string BGM_Code;
    public AudioClip BGM_Audio;
   
}

[CreateAssetMenu(fileName = "Audio", menuName = "ScriptableObject/Audio")]
public class scr_ScriptableObject_Audio : ScriptableObject
{
    public AudioSFX[] AudioSFX;
    public AudioBGM[] AudioBGM;
   

}
