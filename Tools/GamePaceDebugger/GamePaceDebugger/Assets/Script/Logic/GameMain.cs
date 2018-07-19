using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    List<BaseManager> ls_BaseManager = new List<BaseManager>();
    // Use this for initialization
    void Start()
    {
        ls_BaseManager.Add(MapManager.Instance);
        ls_BaseManager.Add(ActorManager.Instance);
        ls_BaseManager.Add(ViewManager.Instance);
    }

    // Update is called once per frame
    void Update()
    {
        if (ls_BaseManager != null && ls_BaseManager.Count > 0)
            foreach (var i in ls_BaseManager)
            {
                i.Update();
            }
    }

    private void OnDestroy()
    {
        foreach (var i in ls_BaseManager)
        {
            i.Release();
        }
        ls_BaseManager.Clear();
    }
}
