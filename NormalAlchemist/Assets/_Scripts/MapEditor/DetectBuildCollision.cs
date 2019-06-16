using UnityEngine;

public class DetectBuildCollision : MonoBehaviour
{
    void OnTriggerEnter()
    {
        if (GlobalMapEditor.OverlapDetection)
            GlobalMapEditor.canBuild = false;
        else
            GlobalMapEditor.canBuild = true;
    }

    void OnTriggerStay()
    {
        if (GlobalMapEditor.OverlapDetection)
            GlobalMapEditor.canBuild = false;
        else
            GlobalMapEditor.canBuild = true;
    }

    void OnTriggerExit()
    {
        GlobalMapEditor.canBuild = true;
    }
}
