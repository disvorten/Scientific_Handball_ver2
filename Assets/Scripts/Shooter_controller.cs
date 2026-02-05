using System.Collections;
using UnityEngine;

public class Shooter_controller : MonoBehaviour
{
    [SerializeField] private GameObject[] projectiles;
    [SerializeField]
    private Color color1 = new Color(0.171f, 0.013f, 0),
        color2 = new Color(0.096f, 0, 0.003f);
    public bool is_false_stimul = false;
    public float velocity;
    public Vector3 direction;
    public float delta_before_shoot;
    public float diameter_of_stimul;
    public float mass_of_stimul;
    public bool is_catched = false;
    public bool use_gravity = false;
    public Color color;
    public GameObject surface = null;

    public SetupConfig setup_config = null;
    void Start()
    {
        gameObject.transform.localScale = Vector3.one * diameter_of_stimul;
        StartCoroutine(CreateStimul());
    }

    private IEnumerator CreateStimul()
    {
        yield return new WaitForSeconds(delta_before_shoot);
        var projectile = projectiles[0];
        var ball = Instantiate(projectile, transform.position, transform.rotation, transform);
        ball.transform.localScale = Vector3.one;
        if (setup_config)
        {
            if (setup_config.config.experiment_number == 2 || setup_config.config.experiment_number == 3)
            {
                ball.GetComponent<SphereCollider>().enabled = false;
            }
        }
        //if(color != null)
        //{
        //    ball.GetComponent<MeshRenderer>().materials[0].color = color;
        //    ball.GetComponent<MeshRenderer>().materials[1].color = color;
        //}
        //if (is_false_stimul)
        //{
        //    ball.GetComponent<MeshRenderer>().materials[0].color = color1;
        //    ball.GetComponent<MeshRenderer>().materials[1].color = color2;
        //}
        //if(use_gravity)
        //{
        //    ball.GetComponent<Rigidbody>().useGravity = true;
        //}
        //else
        // ball.GetComponent<Rigidbody>().useGravity = false;
        //ball.GetComponent<Rigidbody>().mass = mass_of_stimul;
        direction = direction.normalized;
        ball.GetComponent<Rigidbody>().AddForce(direction * velocity, ForceMode.VelocityChange);
        //ball.GetComponent<Rigidbody>().AddForce(direction * velocity/3.6f/7.87f, ForceMode.VelocityChange);
        ball.GetComponent<Rigidbody>().AddTorque(new Vector3(0.7f, 0.7f, 0.7f) * velocity, ForceMode.VelocityChange);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (setup_config)
        {
            if (setup_config.config.experiment_number == 2 || setup_config.config.experiment_number == 3)
            {
                yield return new WaitForSeconds(setup_config.config.stimuls_time_of_flight[0] / 2000);
                ball.GetComponent<SphereCollider>().enabled = true;
            }
        }
    }
    private void OnDestroy()
    {
        if(surface != null)
            surface.SetActive(true);
    }
}
