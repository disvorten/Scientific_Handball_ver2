using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupStartScene : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficulty;
    [SerializeField] private Button start_button;
    [SerializeField] private TMP_InputField name_input;

    private void Start()
    {
        int i = 0;
        foreach(var option in difficulty.options)
        {
            if (option.text == PlayerPrefs.GetString("Difficulty", "������"))
            {
                //Debug.Log(option.text);
                //Debug.Log(i);
                //Debug.Log(PlayerPrefs.GetString("Difficulty", "������"));
                break;
            }    
            i++;
        }
        difficulty.value = i;
        name_input.text = PlayerPrefs.GetString("Name", "���");
        start_button.onClick.AddListener(() => OpenScene());
        name_input.onValueChanged.AddListener(ChangeName);

    }
    private void OpenScene()
    {
        //Debug.Log(difficulty.value);
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
        //if(write_data.isOn)
        //    PlayerPrefs.SetInt("Is_write_data", 1);
        //else
        //    PlayerPrefs.SetInt("Is_write_data", 0);
        SceneManager.LoadScene("MainScene");
    }

    private void ChangeName(string name)
    {
        PlayerPrefs.SetString("Name", name_input.text);
        Debug.Log(name_input.text);
        //Debug.Log(PlayerPrefs.GetString("Name", "���"));
    }
}
