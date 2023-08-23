using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Hero", fileName = "New hero")]
public class PlayerSO : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] float hp = 100;
    [SerializeField] float mp = 100;
    [SerializeField] float dame = 100;
    [SerializeField] float speed = 5;
    [SerializeField] float jumpFoce = 10;
    [SerializeField] float[] time_spawn;

    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public float Mp
    {
        get { return mp; }
        set { mp = value; }
    }
    public float Dame
    {
        get { return dame; }
        set { dame = value; }
    }
    public float Speed
    {
        get { return speed; }
        set
        {
            speed = value;
        }
    }
    public float JumpFoce
    {
        get { return jumpFoce; }
        set
        {
            jumpFoce = value;
        }
    }
    public float[] TimeSpawn
    {
        get { return time_spawn; }
        set { time_spawn = value; }
    }
}
