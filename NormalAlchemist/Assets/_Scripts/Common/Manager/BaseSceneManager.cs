using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneManager
{
    public abstract List<IGetProgress> Init();
    public abstract void OnDestroy();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void onChangeSceneDone(emSceneStatus emStatus, emSceneStatus emLastStatus);
    public abstract void onSetSceneDataDone(emSceneStatus curStatus, emSceneStatus preStatus);
}
