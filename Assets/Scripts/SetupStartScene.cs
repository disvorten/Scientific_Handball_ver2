using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SetupStartScene : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficulty;
    [SerializeField] private UnityEngine.UI.Button start_button;
    [SerializeField] private UnityEngine.UI.Button calibration_button;
    [SerializeField] private UnityEngine.UI.Button calibration_reset_button;
    [SerializeField] private TMP_InputField name_input;
    private SetupConfig setup_config = new();
    public ConfigReader config;
    private AudioSource sound;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        int i = 0;
        foreach(var option in difficulty.options)
        {
            if (option.text == PlayerPrefs.GetString("Difficulty", "Легкая"))
            {
                break;
            }    
            i++;
        }
        difficulty.value = i;
        name_input.text = PlayerPrefs.GetString("Name", "Имя");
        start_button.onClick.AddListener(() => MainScene());
        calibration_button.onClick.AddListener(() => CalibrationScene());
        calibration_reset_button.onClick.AddListener(() => CalibrationReset());
        name_input.onValueChanged.AddListener(ChangeName);
        difficulty.onValueChanged.AddListener(ChangeDiff);
        StartCoroutine(DelayedStart());

    }
    private void MainScene()
    {
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
        SceneManager.LoadScene("MainScene");
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.01f);
        config.ReadConfig(setup_config.Get_config_path());
        if (config.person_name != null)
        {
            name_input.text = config.person_name;
            PlayerPrefs.SetString("Name", name_input.text);
        }
        else
            name_input.text = PlayerPrefs.GetString("Name", "Имя");
    }

    private void ChangeDiff(int i)
    {
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
        config.ReadConfig(setup_config.Get_config_path());
        if (config.person_name != null)
        {
            name_input.text = config.person_name;
            PlayerPrefs.SetString("Name", name_input.text);
        }
        else
            name_input.text = PlayerPrefs.GetString("Name", "Имя");
    }

    private void CalibrationScene()
    {
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
        SceneManager.LoadScene("CalibrationScene");
    }

    private void ChangeName(string name)
    {
        PlayerPrefs.SetString("Name", name_input.text);
    }

    private void CalibrationReset()
    {
        PlayerPrefs.SetFloat("Height", 0f);
        PlayerPrefs.SetFloat("Radius", 0f);
        sound.Play();
    }
}
