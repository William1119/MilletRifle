using UnityEngine;

public class Mine : MonoBehaviour {
    public Object hitEffectPrefab;
    public BulletData mine;

    AudioSource audioSource;
    AudioClip audioClip_Bomb;
    float mineTime = 0;
    // Use this for initialization
    void Start () {
        audioClip_Bomb = Resources.Load<AudioClip>("Sound/MineBomb");
    }
	
	// Update is called once per frame
	void Update () {
        if (mineTime < 0.5f)
            mineTime += Time.deltaTime;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && mineTime >= 0.5f)
        {
            GameObject effect = Instantiate(hitEffectPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            audioSource = effect.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1;
            audioSource.maxDistance = 15;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.clip = audioClip_Bomb;
            audioSource.Play();
            Destroy(effect, 1.2f);
            Destroy(gameObject);
        
            Tank tankScript = other.gameObject.GetComponent<Tank>();
            if (mine.Pierce > tankScript.carriersData.Armor)
                tankScript.carriersData.Hp -= (int)mine.Damage;
            else
                tankScript.carriersData.Hp -= (int)(mine.Damage - (tankScript.carriersData.Armor - mine.Pierce));
            tankScript.showHP_SliderTime = 1.5f;
        }
    }
}
