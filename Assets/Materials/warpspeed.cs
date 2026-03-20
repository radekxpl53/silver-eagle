using UnityEngine;

public class WarpSpeed : MonoBehaviour
{
    public ParticleSystem ps;
    public float speed = 0;
    public float acceleration = 200f;

    void Update()
    {
        speed += acceleration * Time.deltaTime;

        var main = ps.main;
        main.startSpeed = speed;
    }
}