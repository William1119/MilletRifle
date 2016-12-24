using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
    float updateInterval = 0.5F;
    double lastInterval;
    int frames = 0;
    float fps;
    Text fps_text;
    // Use this for initialization
    void Start () {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        fps_text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
            fps_text.text = "FPS: " + fps.ToString("f2");
        }
    }
}
