using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

[CreateAssetMenu]
public class ConfigReader : ScriptableObject
{

    public float number_of_stimuls { get; private set; }
    public int experiment_number { get; private set; }

    public List<float> delta_before_shoot { get; private set; }
    public List<float> delta_t { get; private set; }
    public List<float> stimuls_time_of_flight { get; private set; }
    public List<float> throw_area_for_experiments { get; private set; }

    public List<float> target_area_R { get; private set; }
    public List<float> target_area_H { get; private set; }


    private string delimeter = " ";
    public void ReadConfig(string name)
    {
        using (StreamReader sw  = File.OpenText(name))
        {
            string result = sw.ReadToEnd();
            string[] lines = result.Split("\n");
            throw_area_for_experiments = new();
            stimuls_time_of_flight = new();
            target_area_R = new();
            target_area_H = new();
            delta_t = new();
            delta_before_shoot = new();
            CultureInfo culture = CultureInfo.InvariantCulture;
            for(int i=0; i<lines.Length; i++)
            {
                Debug.Log(lines[i]);
                if (lines[i].StartsWith("#"))
                {
                    switch (lines[i].Split("#")[1])
                    {
                        case "target_area_R":
                            var list2 = lines[i + 1].Split(delimeter);
                            if (list2[0] != " ")
                            {
                                foreach (var elem in list2)
                                {
                                    target_area_R.Add(float.Parse(elem, culture));

                                }
                            }
                            break;
                        case "target_area_H":
                            var list3 = lines[i + 1].Split(delimeter);
                            if (list3[0] != " ")
                            {
                                foreach (var elem in list3)
                                {
                                    target_area_H.Add(float.Parse(elem, culture));

                                }
                            }
                            break;
                        case "number_of_stimuls":
                            number_of_stimuls = float.Parse(lines[i + 1], culture);
                            break;
                        case "experiment_number":
                            experiment_number = int.Parse(lines[i + 1], culture);
                            break;
                        case "delta_t":
                            var list4 = lines[i + 1].Split(delimeter);
                            if (list4[0] != " ")
                            {
                                foreach (var elem in list4)
                                {
                                    delta_t.Add(float.Parse(elem, culture));

                                }
                            }
                            break;
                        case "delta_before_shoot":
                            var list5 = lines[i + 1].Split(delimeter);
                            foreach (var elem in list5)
                            {
                                delta_before_shoot.Add(float.Parse(elem, culture));
                            }
                            break;
                        case "throw_area_for_experiments":
                            var list6 = lines[i + 1].Split(delimeter);
                            foreach (var elem in list6)
                            {
                                throw_area_for_experiments.Add(float.Parse(elem, culture));
                            }
                            break;
                        case "stimuls_time_of_flight":
                            var list1 = lines[i + 1].Split(delimeter);
                            foreach(var elem in list1)
                            {
                                stimuls_time_of_flight.Add(float.Parse(elem, culture));
                            }
                            break;

                    }
                    
                }
            }
        }
    }
}
