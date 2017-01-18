using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (transform.parent)
        {
            if (other.tag == "Enemy")
            {
                Hero target = other.gameObject.GetComponent<Hero>();
                target.heroData.Hp -= 500;
                target.showHP_SliderTime = 1.5f;
            }
        }
    }
}
