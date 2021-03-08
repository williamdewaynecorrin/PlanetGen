using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToTarget : MonoBehaviour
{
    [SerializeField]
    private LerpTarget type;
    [SerializeField]
    [ConditionalHide("type", 0)]
    private Transform target;
    [SerializeField]
    private float lerpspeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (target != null)
            return;

        switch(type)
        {
            case LerpTarget.Parent:
                this.target = transform.parent;
                break;
            case LerpTarget.Root:
                this.target = transform.root;
                break;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerpspeed * Time.deltaTime);
    }
}

public enum LerpTarget
{
    Explicit = 0,
    Parent = 1,
    Root = 10
}