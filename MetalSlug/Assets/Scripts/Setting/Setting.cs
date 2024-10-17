using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    [Min(0.0f)]
    public float time = 0.1f;
    void Start()
    {
        Time.timeScale = time;
    }



}
