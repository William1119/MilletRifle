using UnityEngine;

public class CameraFlow : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        raycastLayers = ~ignoreLayers;
    }

    void Update()
    {

    }

    GameObject target;
    Rigidbody targetRigidbody;
    float height = 1f;
    //float positionDamping = 3f;
    //float velocityDamping = 3f;
    //float distance = 4f;
    LayerMask ignoreLayers = -1;

    RaycastHit hit = new RaycastHit();

    //Vector3 prevVelocity = Vector3.zero;
    LayerMask raycastLayers = -1;

    //Vector3 currentVelocity = Vector3.zero;

    //void Start()
    //{
        //raycastLayers = ~ignoreLayers;
    //}

    //void FixedUpdate()
    //{
        //if (target == null)
        //    target = GameObject.Find("P1(Clone)");
        //if (target != null)
        //{
            //currentVelocity = Vector3.Lerp(prevVelocity, target.GetComponent<Rigidbody>().velocity, velocityDamping * Time.deltaTime);
            //currentVelocity.y = 0;
            //prevVelocity = currentVelocity;
        //}
    //}

    void LateUpdate()
    {
        if (target == null)
            target = GameObject.Find("P1(Clone)");
        if (target != null)
        {
            if (targetRigidbody == null)
                targetRigidbody = target.GetComponent<Rigidbody>();
            float speedFactor = Mathf.Clamp01(targetRigidbody.velocity.magnitude / 70.0f);
            //GetComponent<Camera>().fieldOfView = Mathf.Lerp(55, 72, speedFactor); //相机的视野，以度为单位。
            float currentDistance = Mathf.Lerp(7.5f, 6.5f, speedFactor);

            //currentVelocity = currentVelocity.normalized;

            Vector3 newTargetPosition = target.transform.position + Vector3.up * height;
            Vector3 newPosition = newTargetPosition;// - (currentVelocity * currentDistance);
            newPosition.y = newTargetPosition.y + 18; //相机高度
            newPosition.z = newTargetPosition.z-7;

            Vector3 targetDirection = newPosition - newTargetPosition;
            if (Physics.Raycast(newTargetPosition, targetDirection, out hit, currentDistance, raycastLayers))
                newPosition = hit.point;

            transform.position = newPosition;
            //transform.LookAt(newTargetPosition); //根据目标旋转相机
        }
    }
}