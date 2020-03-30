using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum;
using DefineStruct;
using DataConfig;
public class TestDll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var templeteXLSX = new templeteXLSX();
        templeteXLSX.MainAttribute = ABILLITY.CON;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
