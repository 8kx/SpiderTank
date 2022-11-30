using UnityEngine;

public class Mover : MonoBehaviour
{
    Vector3 dir;
    public float speed = 0.1f;

    float rotation = 0;
    public float rotationSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        int heading = 1;
        dir = Vector3.zero;
        rotation = 0;
        if(Input.GetKey(KeyCode.UpArrow)){
            dir += transform.forward;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            dir -= transform.forward;
            heading = -1;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            rotation -= rotationSpeed;
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            rotation += rotationSpeed;
        }

        transform.position += dir * speed;
        transform.Rotate(0, rotation * heading, 0);
    }
}
