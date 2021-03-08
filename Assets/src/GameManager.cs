using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool DEBUGUTILS = true;
    public static GameManager instance = null;

    // -- spawnable gameplay prefabs
    [SerializeField]
    private GameObject particlefeetsmokeprefab;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public static GameObject GetParticleFeetSmokePrefab()
    {
        return instance.particlefeetsmokeprefab;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            DEBUGUTILS = !DEBUGUTILS;
        }
    }
}
