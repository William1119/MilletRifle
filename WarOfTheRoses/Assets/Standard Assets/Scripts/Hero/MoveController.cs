using UnityEngine;

public class MoveController : MonoBehaviour {
    float ladderSpeed = 2.0f;
    float gravity = 20.0f;
    CharacterController controller;
    string _mode = "";
    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isGrounded)
        {
            ////模拟重力
            if (_mode != "Ladder")
            {
                Vector3 moveDirection = Vector3.zero;
                moveDirection.y -= gravity;
                controller.Move(moveDirection * Time.deltaTime);
            }
        }
    }

    public void Move(Vector3 point, float speed, string mode)
    {
        _mode = mode;
        Vector3 moveDirection = point * speed;
        if (mode == "Ladder")
        {
            
            if (point.z > 0.8)
                moveDirection.y += ladderSpeed;
            else if (point.z < -0.8)
                moveDirection.y -= ladderSpeed;
            else
                moveDirection.y = 0;
            moveDirection.x = 0;
        }

        //角色移动
        controller.Move(moveDirection * Time.deltaTime);
    }
}
