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
    [NonSerialized] public List<List<int>> all_indexes = new();

    private float velocity;
    private float delta_before_shoot = -1f;
    private float delta_t;
    private Vector3 direction;
    private GameObject surface;
    private bool is_first = true;
    private int repeating_counts = 0;

    private void Start()
    {
        points_counter = new();
        points_counter.AddListener(ChangePoints);
        //if(setup_config.config.experiment_number == 2)
        //    stimuls_number = (int)setup_config.config.number_of_stimuls * 2;
        //else
        repeating_counts = (int)setup_config.config.number_of_stimuls;
        stimuls_number = (int)(18f * repeating_counts);
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
            if (setup_config.config.throw_area_for_experiments.Count == 2)
            {
                y = Camera.main.transform.position.y;
            }
            else y = setup_config.config.throw_area_for_experiments[2];
            surface = Instantiate(throw_surface, new Vector3(0, y, setup_config.config.throw_area_for_experiments[0]), Quaternion.identity);
            surface.transform.localScale = new Vector3(1, 1, 0.1f) * setup_config.config.throw_area_for_experiments[0];
            if (setup_config.config.delta_before_shoot[0] == 0)
            {
                surface.SetActive(false);
            }
        }
        GameObject new_shooter = null;
        if (setup_config.config.experiment_number == 2)
        {
            //for (int i = 0; i < stimuls_number; i+=2)
            //{
            //    //surface.SetActive(true);
            //    var new_shooter = SetupExperiment(i, setup_config.config.experiment_number);
            //    new_shooter.GetComponent<Shooter_controller>().direction = direction;
            //    new_shooter.SetActive(false);
            //    if (delta_before_shoot > max_delta) max_delta = delta_before_shoot;
            //    var first_delta = delta_before_shoot;
            //    is_first = false;
            //    var new_shooter2 = SetupExperiment(i, setup_config.config.experiment_number);
            //    new_shooter2.GetComponent<Shooter_controller>().direction = direction;
            //    new_shooter2.SetActive(false);
            //    yield return new WaitForSeconds(delta_t);
            //    surface.SetActive(false);
            //    new_shooter.SetActive(true);
            //    new_shooter2.SetActive(true);
            //    new_shooter.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
            //    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
            //    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = first_delta;
            //    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
            //    new_shooter.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
            //    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
            //    new_shooter2.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
            //    new_shooter2.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
            //    new_shooter2.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
            //    if (setup_config.config.experiment_number == 2)
            //    {
            //        var color_string = setup_config.config.stimuls_colors[UnityEngine.Random.Range(0, setup_config.config.stimuls_colors.Count)];
            //        if (color_string == "green")
            //        {
            //            new_shooter.GetComponent<Shooter_controller>().color = Color.green;

            //        }
            //        if (color_string == "blue")
            //        {
            //            new_shooter.GetComponent<Shooter_controller>().color = Color.blue;

            //        }
            //        color_string = setup_config.config.stimuls_colors[UnityEngine.Random.Range(0, setup_config.config.stimuls_colors.Count)];
            //        if (color_string == "green")
            //        {
            //            new_shooter2.GetComponent<Shooter_controller>().color = Color.green;

            //        }
            //        if (color_string == "blue")
            //        {
            //            new_shooter2.GetComponent<Shooter_controller>().color = Color.blue;

            //        }
            //    }
            //    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
            //    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
            //    new_shooter.GetComponent<StimulDataWriter>().stimul_number = i;
            //    new_shooter2.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
            //    new_shooter2.GetComponent<StimulDataWriter>().data_path = creator.data_path;
            //    new_shooter2.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
            //    new_shooter2.GetComponent<Shooter_controller>().velocity = velocity;
            //    new_shooter2.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
            //    new_shooter2.GetComponent<StimulDataWriter>().stimul_number = i+1;
            //    if (delta_before_shoot > max_delta) max_delta = delta_before_shoot;
            //    is_first = true;
            //    yield return new WaitForSeconds(max_delta);

            //}
        }
        else
        {
            //int [] experiment_indexes = MakePseudoRandomList((int)setup_config.config.number_of_stimuls);
            //Debug.Log("repeating_counts = " + repeating_counts);
            for (int j = 0; j < repeating_counts; j++)
            {
                var experiment_indexes = MakePseudoRandomList(stimuls_number / repeating_counts);
                foreach(int el in experiment_indexes) Debug.Log(el);
                for (int i = 0; i < stimuls_number / repeating_counts; i++)
                {
                    //surface.SetActive(true);
                    new_shooter = SetupExperiment(experiment_indexes[i], setup_config.config.experiment_number);
                    //SuperBallSpawnAnimator ballSpawnAnim = new_shooter.GetComponentInChildren<SuperBallSpawnAnimator>();
                    //ballSpawnAnim.animTime = delta_before_shoot;
                    //new_shooter.transform.GetChild(0).localScale = new_shooter.transform.localScale * setup_config.config.diameter_of_stimul;
                    //ballSpawnAnim.Init();
                    new_shooter.SetActive(false);
                    yield return new WaitForSeconds(delta_t);
                    surface.SetActive(false);
                    new_shooter.SetActive(true);
                    new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                    new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                    new_shooter.GetComponent<Shooter_controller>().direction = direction;
                    new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = 0.44f;
                    new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = 17f;
                    //new_shooter.GetComponent<Shooter_controller>().surface = surface;
                    new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                    new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                    //Debug.Log("J = " + j);
                    //Debug.Log(i + j * repeating_counts);
                    //Debug.Log("i = " + i);
                    new_shooter.GetComponent<StimulDataWriter>().stimul_number = i + j * 18;
                    yield return new WaitForSeconds(delta_before_shoot);
                    //while(new_shooter.GetComponent<Shooter_controller>().is_catched == false || new_shooter != null)
                    //{
                    //    continue;
                    //}
                }
            }
        }
        //Debug.Log(new_shooter);
        while(new_shooter != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(delta_t);
        end.Play();
        statistic.transform.rotation = Quaternion.identity;
        statistic.transform.position = new Vector3(0, cam.transform.position.y, 4);
        start_text.text = $"Процент пойманых мячей: {(float)_success/stimuls_number * 100f} %";
        start_text.gameObject.SetActive(true);
        //GetComponent<ResultToExcelWriter>().Init(creator.data_path, all_indexes);
        using(StreamWriter sw = new StreamWriter(creator.data_path + "/Indexes.csv"))
        {
            sw.WriteLine("C;R;H");
            foreach(var index in all_indexes)
            {
                sw.WriteLine($"{index[0]};{index[1]};{index[2]}");
            }
        }
    }

    private IEnumerator WaitForCatch()
    {
        yield return new WaitForSeconds(delta_before_shoot * 100f);
    }
    private GameObject SetupExperiment(int exp_index, int experiment_number)
    {
        Vector3 start_point_in_global = new();
        Vector3 end_point = new();
        List<int> temp = new();
        var sign = 0;
        if (experiment_number == 1)
        {
            if(exp_index % 2 == 0)
                sign = 1;
            else
                sign = -1;
            start_point_in_global = surface.transform.position + sign * new Vector3(setup_config.config.throw_area_for_experiments[1], 0,0);
        }
        //Debug.Log("INDEX: " + exp_index);
        temp.Add(sign * (-1));
        var new_exp_index = exp_index / 2;
        //if (experiment_number == 2)
        //{ 
        //    if(is_first)
        //        start_point_in_global = surface.transform.position + new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
        //    else
        //        start_point_in_global = surface.transform.position - new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
        //}

        System.Random random = new();
        delta_before_shoot = setup_config.config.delta_before_shoot[0];
        //Debug.Log("NEW_INDEX: " + new_exp_index);
        delta_t = setup_config.config.delta_t[0];
        float R = 0f;
        float H = 0f;
        if (new_exp_index % 3 == 0)
        {
            R = setup_config.config.target_area_R[0];
            temp.Add(1);
        }
        if (new_exp_index % 3 == 1)
        {
            R = setup_config.config.target_area_R[1];
            temp.Add(-1);
        }
        if (new_exp_index % 3 == 2)
        {
            R = setup_config.config.target_area_R[2];
            temp.Add(0);
        }
        new_exp_index = new_exp_index / 3;
        if (new_exp_index % 3 == 0)
        {
            H = setup_config.config.target_area_H[0];
            temp.Add(1);
        }
        if (new_exp_index % 3 == 1)
        {
            H = setup_config.config.target_area_H[1];
            temp.Add(-1);
        }
        if (new_exp_index % 3 == 2)
        {
            H = setup_config.config.target_area_H[2];
            temp.Add(0);
        }
        all_indexes.Add(temp);
        float center_of_human = cam.gameObject.transform.position.y / 2;
        if (sign == 1)
            end_point = new Vector3(H, R, 0f);
        else
            end_point = new Vector3(-H, R, 0f);
        //Debug.Log("END point: " + end_point);
        direction = end_point - start_point_in_global;
        velocity = direction.magnitude / (setup_config.config.stimuls_time_of_flight[0] / 1000f);
        return Instantiate(shooter, start_point_in_global, Quaternion.identity);
    }

}
