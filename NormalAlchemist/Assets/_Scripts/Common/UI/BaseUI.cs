using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BaseUI : MonoBehaviour
{
    public delegate void eventHandlerOnShow(object sender, bool isShow);
    public event eventHandlerOnShow evtOnShow;

    private List<UIBehaviour> listControls;

    UITween UITween = null;

    public Enum_BaseUIType UIType = Enum_BaseUIType.Except;
    public Enum_BaseUIShowMode ShowMode = Enum_BaseUIShowMode.Normal;

    public virtual void Awake()
    {
        //instance = this;
        DontDestroyOnLoad(this);

        isActive = false;

        listControls = new List<UIBehaviour>();

        UITween = GetComponent<UITween>();
        if (UITween != null)
        {
            UITween.EndHideMoveEvent += EndTweenHide;
            UITween.EndShowMoveEvnet += EndTweenShow;
        }
        InitControls();
    }

    protected bool _isInit = false;
    public virtual void Init()
    {
        if (_isInit)
            return;
        _isInit = true;
    }

    public virtual void ReInit()
    {
        //_isInit = false;
        //Init();
    }

    private void InitControls()
    {
        UIBehaviour[] controls = GetComponentsInChildren<UIBehaviour>();
        if (controls.Length != 0)
        {
            foreach (UIBehaviour ui in controls)
            {
                listControls.Add(ui);
            }
        }
    }

    /// <summary>
    /// 显示Ui
    /// </summary>
    public virtual void Show()
    {
        if (!gameObject.activeSelf)
        {
            isActive = true;
            if (UITween != null)
            {
                UITween.ShowTween();
                _UIShowStatus = Enum_UIShowStatus.Showing;
            }
            else
            {
                _UIShowStatus = Enum_UIShowStatus.Show;
            }
            //UIManager.Instance.ShowUI(GetType(), ShowMode);
        }
    }

    System.Action EndShowTweenCallAction = null;
    bool isEndShowTweenActionOnce = true;
    public virtual void Show(System.Action endShowEffectAction, bool isOnce = true)
    {
        EndShowTweenCallAction = endShowEffectAction;
        Show();
    }

    public virtual void OnShow()
    {
        if (!isActive)
            UIManager.Instance.ShowUI(GetType(), ShowMode);
    }

    /// <summary>
    /// 重新刷新数据
    /// </summary>
    public virtual void ReDisplay()
    {
        isActive = true;
    }

    bool _isActive = false;
    public bool isActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;
            gameObject.SetActive(value);
        }
    }
    public virtual void Hide()
    {
        if (UITween != null && gameObject.activeSelf)
        {
            UITween.HideTween();
            isActive = false;
            _UIShowStatus = Enum_UIShowStatus.Hiding;
            //UIManager.Instance.CloseOrReturnUIForms(GetType(), ShowMode);
        }
        else
        {
            if (gameObject.activeSelf)
            {
                isActive = false;
                //UIManager.Instance.CloseOrReturnUIForms(GetType(), ShowMode);
            }
            _UIShowStatus = Enum_UIShowStatus.Hide;
        }
    }

    public virtual void OnHide()
    {
        if (isActive)

            UIManager.Instance.CloseOrReturnUIForms(GetType(), ShowMode);
    }

    private Enum_UIShowStatus _UIShowStatus = Enum_UIShowStatus.Hide;
    public Enum_UIShowStatus UIShowStatus
    {
        get
        {
            return _UIShowStatus;
        }
    }
    public void EndTweenHide(GameObject go)
    {
        _UIShowStatus = Enum_UIShowStatus.Hide;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void EndTweenShow(GameObject go)
    {
        _UIShowStatus = Enum_UIShowStatus.Show;
        if (EndShowTweenCallAction != null)
        {
            EndShowTweenCallAction();
            if (isEndShowTweenActionOnce)
                EndShowTweenCallAction = null;
        }
    }
}