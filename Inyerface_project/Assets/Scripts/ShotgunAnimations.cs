using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I'm so sorry if you're reading in the future, but this would be better named as gun animations because I'll use it for both but I'm too lazy to change it. Honestly this class is useless and the guns shouldn't be 
//implemented in the same "Gun" class as they are but oof game jam
public class ShotgunAnimations : MonoBehaviour
{
    public Animator anim;
    private AudioManager audio;

    public bool shooting;
    public bool reloading;

    private void Start()
    {
        audio = FindObjectOfType<AudioManager>();        
    }
    public void SetShooting(int isShooting)
    {
        shooting = isShooting > 0;
    }
    public void SetReloading(int isReloading)
    {
        reloading = isReloading > 0;
    }
    
    public void PlaySound(string soundName)
    {
        audio.Play(soundName);
    }

}
