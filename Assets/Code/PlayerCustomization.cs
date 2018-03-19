using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomization : MonoBehaviour {
    public AudioSource MainMusic;
    public AudioSource MainSFX;

    public Toggle Music;
    public Toggle SFX;

    public void OnSFXChange() {
        bool sfx_choice = SFX.isOn;
        PlayerPrefs.SetInt("SFX", sfx_choice ? 1 : 0);
    }

    public void OnMusicChange() {
        bool music_choice = Music.isOn;
        PlayerPrefs.SetInt("music", music_choice ? 1 : 0);
        MainMusic.enabled = Music.isOn;

        Debug.Log(PlayerPrefs.GetInt("music"));
    }

    public void OnSkinChange() {
        if (PlayerPrefs.GetInt("skin_unlock_" + (GetComponent<Menu>().PlayerSkinChoice + 1)) == 1) {
            PlayerPrefs.SetInt("skin", GetComponent<Menu>().PlayerSkinChoice);
        }
    }

    public void OnSkillChange() {
        if (PlayerPrefs.GetInt("skill_unlock_" + (GetComponent<Menu>().PlayerSkillChoice + 1)) == 1) {
            PlayerPrefs.SetInt("skill", GetComponent<Menu>().PlayerSkillChoice);
        }
    }
}