using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControll : MonoBehaviour
{
    protected dynamic data;
    protected dynamic view;

    private void Awake()
    {
        view = GetComponent<IBindView>();
    }

    protected T SetModel<T>()where T : BaseModel
    {
        T data = System.Activator.CreateInstance<T>();
        data.View = view;
        this.data = data;
        return data;
    }
}

