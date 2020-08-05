using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion rot = Quaternion.LookRotation(transform.position - mousePosition, Vector3.forward);

        transform.rotation = rot;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        GetComponent<Rigidbody2D>().angularVelocity = 0;

        if (Input.GetKey("w"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, speed));
        }

        if (Input.GetKey("a"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-speed, 0));
        }

        if (Input.GetKey("s"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -speed));
        }

        if (Input.GetKey("d"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(speed, 0));
        }

    }
}
