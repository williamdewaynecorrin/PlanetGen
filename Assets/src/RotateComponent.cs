using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateComponent : MonoBehaviour
{
    public RotateMethod type = RotateMethod.Explicit;

    [ConditionalHide("type", 0)]
    public Vector3 rotaxis = Vector3.right;
    public float rotspeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(type)
        {
            case RotateMethod.Explicit:
                this.transform.Rotate(rotaxis, rotspeed);
                break;
            case RotateMethod.RightOrient:
                this.transform.Rotate(Vector3.right, rotspeed);
                break;
            case RotateMethod.UpOrient:
                this.transform.Rotate(Vector3.up, rotspeed);
                break;
            case RotateMethod.ForwardOrient:
                this.transform.Rotate(Vector3.forward, rotspeed);
                break;
        }
    }
}

public enum RotateMethod
{
    Explicit = 0,
    RightOrient = 1,
    UpOrient = 2,
    ForwardOrient = 3
}
