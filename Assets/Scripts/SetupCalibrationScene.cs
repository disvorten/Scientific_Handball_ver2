using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupCalibrationScene : MonoBehaviour
{
    private DateTime start_time;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject right_arm;
    [SerializeField] private GameObject head;
    [SerializeField] private UnityEngine.UI.Button back_button;
    private List<float> positions_x = new();
    private List<float> positions_y = new();
    private AudioSource sound;
    private bool first_flag = false;
    private bool second_flag = false;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        start_time = DateTime.Now;
        text.text = "Поднимите обе руки горизонтально в стороны на уровень плеч";
        back_button.onClick.AddListener(() => StartScene());
        back_button.gameObject.SetActive(false);
        sound.Play();
    }

    void FixedUpdate()
    {
        if((DateTime.Now - start_time).TotalSeconds >= 5.5f && !first_flag)
        {
            positions_y.Add(right_arm.transform.position.y);
            positions_x.Add(right_arm.transform.position.x - head.transform.position.x);
            if ((DateTime.Now - start_time).TotalSeconds >= 7 && !first_flag)
            {
                sound.Play();
                PlayerPrefs.SetFloat("Height", positions_y.Average());
                PlayerPrefs.SetFloat("Radius", positions_x.Average());
                //Debug.Log("Height: " + positions_x.Average());
                text.text = "Калибровка проведена! Нажмите кнопку 'Назад'";
                first_flag = true;
                back_button.gameObject.SetActive(true);
            }
        }
        //if ((DateTime.Now - start_time).TotalSeconds >= 9 && !second_flag)
        //{
        //    positions.Add(right_arm.transform.position.x - head.transform.position.x);
        //    if ((DateTime.Now - start_time).TotalSeconds >= 10 && !second_flag)
        //    {
        //        sound.Play();
        //        PlayerPrefs.SetFloat("Radius", positions.Average());
        //        text.text = "Калибровка проведена! Нажмите кнопку 'Назад'";
        //        back_button.gameObject.SetActive(true);
        //        second_flag = true;
        //    }
        //}
    }

    private void StartScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
