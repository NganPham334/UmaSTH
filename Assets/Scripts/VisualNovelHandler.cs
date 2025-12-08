using System;
using UnityEngine;

public class VisualNovelHandler : MonoBehaviour
{
    /*
     * vn_type:
     * studying: add another param named "study_type" with value spd / mem / wit / luk
     * pastime
     * rest
     * pre_test
     * post_test
     * determined
     * random
     */

    private String _vnType;
    
    void Awake()
    {
        if (!GameStateMan.Instance.TryGetStateParameter<String>("vn_type", out var type))
        {
            Debug.LogError("VisualNovelHandler: Novel Type not found");
            return;
        }
        _vnType = type;
        Debug.Log(type);
    }

    public static void FinishScene()
    {
        // for now, testing
        GameStateMan.Instance.ReportActionComplete();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
