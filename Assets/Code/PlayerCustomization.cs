using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomization : MonoBehaviour {
    public AudioSource MainMusic;
    public GameObject MainSFX;
    public GameObject RightControls;
    public GameObject LeftyControls;

    public Toggle Music;
    public Toggle SFX;
    public Toggle Lefty;

    public void OnSFXChange() {
        bool sfx_choice = SFX.isOn;
        PlayerPrefs.SetInt("SFX", sfx_choice ? 1 : 0);
        MainSFX.SetActive(sfx_choice);
    }

    public void OnMusicChange() {
        bool music_choice = Music.isOn;
        PlayerPrefs.SetInt("music", music_choice ? 1 : 0);
        MainMusic.enabled = Music.isOn;
    }

    public void OnLeftyChange() {
        bool lefty = Lefty.isOn;
        PlayerPrefs.SetInt("lefty", lefty ? 1 : 0);
        LeftyControls.SetActive(Lefty.isOn);
        RightControls.SetActive(!Lefty.isOn);
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