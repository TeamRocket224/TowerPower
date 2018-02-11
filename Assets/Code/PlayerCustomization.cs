using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomization : MonoBehaviour {
    public Toggle Music;
    public Toggle SFX;

    public void OnSFXChange() {
        bool sfx_choice = SFX.isOn;
        PlayerPrefs.SetInt("SFX", sfx_choice ? 1 : 0);
    }

    public void OnMusicChange() {
        bool music_choice = Music.isOn;
        PlayerPrefs.SetInt("music", music_choice ? 1 : 0);

        Debug.Log(PlayerPrefs.GetInt("music"));
    }

    public void OnSkinChange() {
        PlayerPrefs.SetInt("skin", GetComponent<Menu>().PlayerSkinChoice);
    }

    public void OnPowerChange() {
        PlayerPrefs.SetInt("power", GetComponent<Menu>().PlayerPowerChoice);
    }

    public void OnGameEnd() {
        //PlayerPrefs.SetInt("height", @todo: Height of the Player here);
    }
}