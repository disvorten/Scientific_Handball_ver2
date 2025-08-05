using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System;
using UnityEngine.XR.Interaction.Toolkit;
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
    private GameObject surface;
    private bool is_first = true;
    private int repeating_counts = 0;
    private string index_path;
    public float arm_sphere_diameter;
    private List<List<int>> second_exp_list = new() {new(){0,3},new(){0,2}, new() { 1, 3 }, new() { 1, 2 } };
    private void Start()
    {
        points_counter = new();
        arm_sphere_diameter = setup_config.config.diameter_of_arm;
        points_counter.AddListener(ChangePoints);
        //if(setup_config.config.experiment_number == 2)
        //    stimuls_number = (int)setup_config.config.number_of_stimuls * 2;
        //else
        repeating_counts = (int)setup_config.config.number_of_stimuls;
        if(setup_config.config.experiment_number == 1)
            stimuls_number = (int)(6f * repeating_counts);
        else
            stimuls_number = (int)(4f * repeating_counts);
        ChangeTextOfStimuls(stimuls_number);
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
        points.text = $"Отраженные: {_success} \nПропущенные: {_miss}";
        ChangeTextOfStimuls(stimuls_number - _miss - _success);
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
        float max_delta = 0f;
        if(setup_config.config.experiment_number == 1 || setup_config.config.experiment_number == 2)
        {
            float y;
            y = setup_config.config.throw_area_for_experiments[1];
        }
        GameObject new_shooter = null;
        using (StreamWriter sw = File.AppendText(index_path))
        {
            sw.WriteLine("S;H;Angle");
        }
        delta_before_shoot = setup_config.config.delta_before_shoot[0];
        delta_t = setup_config.config.delta_t[0];

        for (int j = 0; j < repeating_counts; j++)
        {
            var experiment_indexes = MakePseudoRandomList(stimuls_number / repeating_counts);
            for (int i = 0; i < stimuls_number / repeating_counts; i++)
            {
                if(setup_config.config.experiment_number == 1)
                {
                    new_shooter = SetupExperiment(experiment_indexes[i], setup_config.config.experiment_number);
                    new_shooter.SetActive(false);
                    yield return new WaitForSeconds(delta_t);
                    new_shooter.SetActive(true);
                    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter.GetComponent<StimulDataWriter>().stimul_number = i + j * (int)(stimuls_number / repeating_counts);
                    yield return new WaitForSeconds(delta_before_shoot);
                }
                if (setup_config.config.experiment_number == 2)
                {
                    System.Random rand = new();
                    int ind = rand.Next(0, 2);
                    new_shooter = SetupExperiment(second_exp_list[i][ind], setup_config.config.experiment_number);
                    new_shooter.SetActive(false);
                    yield return new WaitForSeconds(delta_t);
                    new_shooter.SetActive(true);
                    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter.GetComponent<StimulDataWriter>().stimul_number = 2*i + j * (int)(stimuls_number / repeating_counts);
                    yield return new WaitForSeconds(delta_t);
                    var new_shooter2 = SetupExperiment(second_exp_list[i][1 - ind], setup_config.config.experiment_number);
                    new_shooter2.SetActive(false);
                    yield return new WaitForSeconds(delta_t);
                    new_shooter2.SetActive(true);
                    new_shooter2.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter2.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter2.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter2.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter2.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                    new_shooter2.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter2.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    new_shooter2.GetComponent<StimulDataWriter>().stimul_number = 2*i + 1 + j * (int)(stimuls_number / repeating_counts);
                    yield return new WaitForSeconds(delta_before_shoot);
                }
            }
        }

        //Debug.Log(new_shooter);
        while(new_shooter != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        end.Play();
        statistic.transform.rotation = Quaternion.identity;
        statistic.transform.position = new Vector3(0, cam.transform.position.y, 4);
        start_text.text = $"Процент пойманых мячей: {(float)_success/stimuls_number * 100f} %";
        start_text.gameObject.SetActive(true);
    }

    private IEnumerator WaitForCatch()
    {
        yield return new WaitForSeconds(delta_before_shoot * 100f);
    }
    private GameObject SetupExperiment(int exp_index, int experiment_number)
    {
        Vector3 start_point_in_global;
        Vector3 end_point;
        start_point_in_global = new Vector3(0, setup_config.config.throw_area_for_experiments[1], setup_config.config.throw_area_for_experiments[0]);
        //if (experiment_number == 2)
        //{ 
        //    if(is_first)
        //        start_point_in_global = surface.transform.position + new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
        //    else
        //        start_point_in_global = surface.transform.position - new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
        //}

        System.Random random = new();
        float R = setup_config.config.target_area_R[0];
        float P = setup_config.config.throw_area_for_experiments[1];
        float angle = setup_config.config.target_area_alpha[exp_index];
        int Side = 0;
        int H = 0;
        if(exp_index == 0)
        {
            Side = -1;
            H = 0;
        }
        if(exp_index == 1)
        {
            Side = -1;
            H = -1;
        }
        if(exp_index == 2)
        {
            Side = 1;
            H = -1;
        }
        if(exp_index == 3)
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
            sw.WriteLine($"{Side};{H};{angle}");
        }
        end_point = new Vector3(R * (float)Math.Cos(Math.PI * angle / 180), R * (float)Math.Sin(Math.PI * angle / 180) + P, 0f);
        direction = end_point - start_point_in_global;
        velocity = direction.magnitude / (setup_config.config.stimuls_time_of_flight[0] / 1000f);
        return Instantiate(shooter, start_point_in_global, Quaternion.identity);
    }

}
