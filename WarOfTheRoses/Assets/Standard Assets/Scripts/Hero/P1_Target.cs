using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class P1_Target : MonoBehaviour {
    public Transform target;
    float updataTime = 0;

    void Start()
    {

    }

    void Update()
    {
        updataTime += Time.deltaTime; //计时

        if (triggerTime + 0.1f < Time.time)
            target = null;
    }

    float triggerTime = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "P1" || other.tag == "RedAI" || other.tag == "Enemy")
        {
            target = other.transform;
            triggerTime = Time.time;
        }
    }
}
