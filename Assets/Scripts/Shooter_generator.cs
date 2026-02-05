using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class Shooter_generator : MonoBehaviour
{
    private int _miss = 0;
    private int _success = 0;
    private int stimuls_number = 0;
    private System.Random rand = new();
    [HideInInspector] public UnityEvent<int, int> points_counter;
    [SerializeField] private TMP_Text points;
    [SerializeField] private TMP_Text balls;
    [SerializeField] private TMP_Text start_text;
    [SerializeField] private SetupConfig setup_config;
    [SerializeField] private GameObject shooter;
    [SerializeField] private DataPathCreator creator;
    [SerializeField] private GameObject statistic;
    [SerializeField] private GameObject cam;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource start;
    [SerializeField] private AudioSource end;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject throw_surface;

    private float velocity;
    private float delta_before_shoot = -1f;
    private float delta_t;
    private Vector3 direction;
    private int repeating_counts = 0;
    private string index_path;
    public float arm_sphere_diameter;
    private List<List<int>> second_exp_list = new() {new(){1,2}, new() { 0, 3 }, new() { 5, 4 }};
    private List<List<int>> third_exp_list = new() {new(){0,1}, new() { 0, 2 }, new() { 3, 1 }, new() { 3, 2 } };
    private float Height;
    private float Radius;
    private void Start()
    {
        points_counter = new();
        Height = PlayerPrefs.GetFloat("Height", 0f);
        Radius = PlayerPrefs.GetFloat("Radius", 0f);
        Debug.Log("Height: " + Height);
        Debug.Log("Radius: " + Radius);
        arm_sphere_diameter = setup_config.config.diameter_of_arm;
        points_counter.AddListener(ChangePoints);
        repeating_counts = (int)setup_config.config.number_of_stimuls;
        if(setup_config.config.experiment_number == 1)
        {
            stimuls_number = (int)(6f * repeating_counts);
            ChangeTextOfStimuls(stimuls_number);
        }
        if (setup_config.config.experiment_number == 2)
        {
            stimuls_number = (int)(3f * repeating_counts);
            ChangeTextOfStimuls(stimuls_number * 2);
        }
        if (setup_config.config.experiment_number == 3)
        {
            stimuls_number = (int)(4f * repeating_counts);
            ChangeTextOfStimuls(stimuls_number * 2);
        }
        if (Height == 0f)
        {
            Height = setup_config.config.throw_area_for_experiments[1];
            Radius = setup_config.config.target_area_R[0];
        }
        Debug.Log("Height: " + Height);
        Debug.Log("Radius: " + Radius);
        throw_surface.transform.position = new Vector3(0, Height, setup_config.config.throw_area_for_experiments[0]);
        StartCoroutine(StartExperiment());
    }

    private void ChangeTextOfStimuls(int number)
    {
        balls.text = $"Бросков осталось: {number}";
    }    
    private void ChangePoints(int miss, int success)
    {
        _miss += miss;
        _success += success;
        points.text = $"Отраженные: {_success} \nПропущенные: {_miss} \nHeight: {Height} \n Radius: {Radius}";
        if ((setup_config.config.experiment_number == 2) || (setup_config.config.experiment_number == 3))
        {
            ChangeTextOfStimuls(stimuls_number * 2 - _miss - _success);
        }
        else
        {
            ChangeTextOfStimuls(stimuls_number - _miss - _success);
        }
    }

    private int[] MakePseudoRandomList(int number)
    {
        int[] indexes = Enumerable.Range(0, number).ToArray();

        for (int i = indexes.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);
            (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
        }
        return indexes;
    }

    private int[] MakePseudoRandomList(int number, int repeats)
    {
        List<int> all_indexes = new();
        for (int k = 0; k < repeats;k++)
        {
            int[] indexes = Enumerable.Range(0, number).ToArray();
            //for (int i = indexes.Length - 1; i >= 1; i--)
            //{
            //    int j = rand.Next(i + 1);
            //    (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
            //}
            all_indexes.AddRange(indexes);
        }
        var all_indexes_int = all_indexes.ToArray();
        for (int i = all_indexes_int.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);
            (all_indexes_int[i], all_indexes_int[j]) = (all_indexes_int[j], all_indexes_int[i]);
        }
        //for (int i = all_indexes_int.Length - 1; i >= 1; i--)
        //{
        //    int j = rand.Next(i + 1);
        //    (all_indexes_int[i], all_indexes_int[j]) = (all_indexes_int[j], all_indexes_int[i]);
        //}
        return all_indexes_int;
    }

    private IEnumerator StartExperiment()
    {
        index_path = creator.data_path + "/Indexes.csv";
        arrow.SetActive(true);
        for (int i = 0;i < 5; i++)
        {
            click.Play();
            start_text.text = $"Эксперимент начнется через {5 - i} секунд";
            yield return new WaitForSeconds(1f);
        }
        arrow.SetActive(false);
        start_text.gameObject.SetActive(false);
        start.Play();
        if(setup_config.config.experiment_number == 1 || setup_config.config.experiment_number == 2 || setup_config.config.experiment_number == 3)
        {
            float y;
            y = Height;
        }
        GameObject new_shooter = null;
        using (StreamWriter sw = File.AppendText(index_path))
        {
            if (setup_config.config.experiment_number == 1)
            {
                sw.WriteLine("S;H;Angle;Height;Radius");
            }
            else
            {
                sw.WriteLine("L;R;Height;Radius");
            }
        }
        delta_before_shoot = setup_config.config.delta_before_shoot[0];
        delta_t = setup_config.config.delta_t[0];
        int[] experiment_indexes = MakePseudoRandomList(stimuls_number / repeating_counts, repeating_counts);
        //foreach (int experiment_index in experiment_indexes)
        //    Debug.Log(experiment_index);
        for (int j = 0; j < repeating_counts; j++)
        {
            //int[] experiment_indexes = MakePseudoRandomList(stimuls_number / repeating_counts);
            for (int i = 0; i < stimuls_number / repeating_counts; i++)
            {
                if (setup_config.config.experiment_number == 1)
                {
                    new_shooter = SetupExperiment(experiment_indexes[i + j * (int)(stimuls_number / repeating_counts)], 1);
                    //new_shooter = SetupExperiment(experiment_indexes[i], 1);
                    new_shooter.SetActive(false);
                    yield return new WaitForSeconds(delta_before_shoot);
                    new_shooter.SetActive(true);
                    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter.GetComponent<StimulDataWriter>().stimul_number = i + j * (int)(stimuls_number / repeating_counts);
                    new_shooter.GetComponent<Shooter_controller>().setup_config = setup_config;
                    yield return new WaitForSeconds(delta_t);
                }
                else
                {
                    List<int> list = new();
                    if (setup_config.config.experiment_number == 2)
                    {
                        list = second_exp_list[experiment_indexes[i + j * (int)(stimuls_number / repeating_counts)]];
                    }
                    else
                    {
                        list = third_exp_list[experiment_indexes[i + j * (int)(stimuls_number / repeating_counts)]];
                    }
                    System.Random rand = new();
                    int ind = rand.Next(0, 2);
                    new_shooter = SetupExperiment(list[ind], 2);
                    new_shooter.SetActive(false);
                    yield return new WaitForSeconds(delta_before_shoot);
                    new_shooter.SetActive(true);
                    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter.GetComponent<StimulDataWriter>().stimul_number = 2 * i + j * (int)(stimuls_number * 2 / repeating_counts);
                    new_shooter.GetComponent<Shooter_controller>().setup_config = setup_config;
                    var new_shooter2 = SetupExperiment(list[1 - ind], 2);
                    new_shooter2.SetActive(true);
                    new_shooter2.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter2.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter2.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter2.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter2.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter2.GetComponent<Shooter_controller>().setup_config = setup_config;
                    new_shooter2.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter2.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter2.GetComponent<StimulDataWriter>().stimul_number = 2 * i + 1 + j * (int)(stimuls_number * 2 / repeating_counts);
                    using (StreamWriter sw = File.AppendText(index_path))
                    {
                        sw.WriteLine($"{setup_config.config.target_area_alpha[list[1]]};{setup_config.config.target_area_alpha[list[0]]};{Height};{Radius}");
                    }
                    yield return new WaitForSeconds(delta_t);
                }
                    
            }
        }

        while(new_shooter != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        end.Play();
        throw_surface.SetActive(false);
        statistic.transform.rotation = Quaternion.identity;
        statistic.transform.position = new Vector3(0, cam.transform.position.y, 4);
        if (setup_config.config.experiment_number == 2 || setup_config.config.experiment_number == 3)
        {
            start_text.text = $"{PlayerPrefs.GetString("Name", "Имя")},\n\r Ваш результат: \n\r Процент пойманых мячей: {Math.Round((float)_success / (stimuls_number * 2) * 100f, 2)} %";

        }
        else
        {
            start_text.text = $"{PlayerPrefs.GetString("Name", "Имя")}, \n\r Ваш результат: \n\r Процент пойманых мячей: {Math.Round((float)_success / stimuls_number * 100f, 2)} %";
        }
        start_text.gameObject.SetActive(true);
    }

    private GameObject SetupExperiment(int exp_index, int experiment_number)
    {
        Vector3 start_point_in_global;
        Vector3 end_point;
        start_point_in_global = new Vector3(0, Height, setup_config.config.throw_area_for_experiments[0]);
        float R = Radius;
        float P = Height;
        float angle = setup_config.config.target_area_alpha[exp_index];
        if (experiment_number == 1) 
        {
            System.Random random = new();
            int Side = 0;
            int H = 0;
            if (exp_index == 0)
            {
                Side = -1;
                H = 0;
            }
            if (exp_index == 1)
            {
                Side = -1;
                H = -1;
            }
            if (exp_index == 2)
            {
                Side = 1;
                H = -1;
            }
            if (exp_index == 3)
            {
                Side = 1;
                H = 0;
            }
            if (exp_index == 4)
            {
                Side = 1;
                H = 1;
            }
            if (exp_index == 5)
            {
                Side = -1;
                H = 1;
            }
            using (StreamWriter sw = File.AppendText(index_path))
            {
                sw.WriteLine($"{Side};{H};{angle};{Height};{Radius}");
            }
        }
        end_point = new Vector3(R * (float)Math.Cos(Math.PI * angle / 180), R * (float)Math.Sin(Math.PI * angle / 180) + P, 0f);
        direction = end_point - start_point_in_global;
        velocity = direction.magnitude / (setup_config.config.stimuls_time_of_flight[0] / 1000f);
        return Instantiate(shooter, start_point_in_global, Quaternion.identity);
    }

}
