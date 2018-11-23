using System;
using UnityEngine;

public class UITween : MonoBehaviour
{
    public float ShowTime = 0;
    public float HideTime = 0;
    public float StayTime = 0;

    public string Tag;

    private Enum_UIShowStatus state = Enum_UIShowStatus.Hide;

    

    [Serializable]
    public enum TargetType
    {
        To,
        From
    }
    [SerializeField]
    public UIStatus TargetStatus;
    [SerializeField]
    public UIStatus OrignalStatus;
    [SerializeField]
    public TargetType targetType;
    [Serializable]
    public struct UIStatus
    {
        public Vector3 AnchoredPos;
        public float Alpha;
        public Vector3 Scale;
        public Vector3 Rotation;
    }

    //public float MoveStep = 2;
    //public float MoveSpeed = 1;
    public float MoveTimeScale = 1;

    [SerializeField]
    public bool isAutoHide;
    [SerializeField]
    public bool isUIShowMove;
    //[SerializeField]
    //public Vector2 StartPos;
    //[SerializeField]
    //public Vector2 EndPos;

    [SerializeField]
    public AnimationCurve m_UIShowMove_X;
    [SerializeField]
    public AnimationCurve m_UIShowMove_Y;

    [SerializeField]
    public bool isUIShowAlpha;
    [SerializeField]
    public AnimationCurve m_UIShowAlpha = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    public bool isUIShowScale;

    //public Vector3 StartScale;
    //public Vector3 EndScale;

    [SerializeField]
    public Animation3DCurve m_UIShowScale = new Animation3DCurve();

    [SerializeField]
    public bool isUIHideScale;

    [SerializeField]
    public Animation3DCurve m_UIHideScale = new Animation3DCurve();

    [Serializable]
    public struct Animation3DCurve
    {
        public AnimationCurve X;
        public AnimationCurve Y;
        public AnimationCurve Z;
    }


    [SerializeField]
    public bool isUIHideMove;
    [SerializeField]
    public AnimationCurve m_UIHideMove_X;
    [SerializeField]
    public AnimationCurve m_UIHideMove_Y;
    [SerializeField]
    public bool isUIHideAlpha;
    [SerializeField]
    public AnimationCurve m_UIHideAlpha = AnimationCurve.EaseInOut(0, 1, 1, 0);

    //private Vector3 originalPos = Vector3.zero;
    //public bool isMoveToPos;
    //public Transform targetPos;
    [SerializeField]
    public bool isUIShowRotate;
    [SerializeField]
    public Animation3DCurve m_UIShowRotate = new Animation3DCurve();

    [SerializeField]
    public bool isUIHideRotate;
    [SerializeField]
    public Animation3DCurve m_UIHideRotate = new Animation3DCurve();
    [SerializeField]
    public bool isDataOriginal = false;
    private CanvasGroup _cvsGp;
    private CanvasGroup cvsGp
    {
        get
        {
            if (_cvsGp == null)
            {
                _cvsGp = GetComponent<CanvasGroup>();
                if (_cvsGp == null)
                {
                    _cvsGp = gameObject.AddComponent<CanvasGroup>();
                }
            }

            return _cvsGp;
        }
    }

    private RectTransform m_RectTf;
    private RectTransform RectTf
    {
        get
        {
            if (m_RectTf == null)
            {
                m_RectTf = GetComponent<RectTransform>();
            }
            return m_RectTf;
        }
    }

    public delegate void EndMoveDlg(GameObject go);
    public EndMoveDlg EndHideMoveEvent;
    public EndMoveDlg EndShowMoveEvnet;
    // Use this for initialization
    void Start()
    {
        if (!isSaved)
        {
            SaveOrignalStatus();
        }
        //originalPos = GetComponent<RectTransform>().anchoredPosition;
    }

    float moveX;
    float moveY;
    float scaleX;
    float scaleY;
    float scaleZ;
    // Update is called once per frame
    Vector2 currentPos;
    Vector3 currentScale;
    Vector3 currentEuler;
    void Update()
    {
        PassTime += Time.deltaTime * MoveTimeScale;

        if (state == Enum_UIShowStatus.Showing)
        {
            if (ShowTime != 0)
            {
                if (T_Show + ShowTime < PassTime)
                {
                    state = Enum_UIShowStatus.Show;
                    T_Stay = PassTime;
                    ShowTargetStatus();
                    return;
                }

                if (isUIShowMove)
                {
                    switch (targetType)
                    {
                        case TargetType.From:
                            currentPos.x = TargetStatus.AnchoredPos.x + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).x * m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentPos.y = TargetStatus.AnchoredPos.y + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).y * m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        case TargetType.To:
                            currentPos.x = OrignalStatus.AnchoredPos.x + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).x * m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentPos.y = OrignalStatus.AnchoredPos.y + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).y * m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        default:
                            currentPos.x = OrignalStatus.AnchoredPos.x + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).x * m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentPos.y = OrignalStatus.AnchoredPos.y + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).y * m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                    }

                    //currentPos = (EndPos - StartPos) * m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime);
                    //moveY = m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime) * MoveStep;
                    RectTf.anchoredPosition = currentPos;
                }

                if (isUIShowAlpha)
                {
                    switch (targetType)
                    {
                        case TargetType.From:
                            cvsGp.alpha = TargetStatus.Alpha + (OrignalStatus.Alpha - TargetStatus.Alpha) * m_UIShowAlpha.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        case TargetType.To:
                            cvsGp.alpha = OrignalStatus.Alpha + (TargetStatus.Alpha - OrignalStatus.Alpha) * m_UIShowAlpha.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        default:
                            cvsGp.alpha = OrignalStatus.Alpha + (TargetStatus.Alpha - OrignalStatus.Alpha) * m_UIShowAlpha.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                    }
                    //cvsGp.alpha = m_UIShowAlpha.Evaluate((PassTime - T_Show) / ShowTime);
                }

                if (isUIShowScale)
                {
                    switch (targetType)
                    {
                        case TargetType.From:
                            currentScale.x = TargetStatus.Scale.x + (OrignalStatus.Scale - TargetStatus.Scale).x * m_UIShowScale.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentScale.y = TargetStatus.Scale.y + (OrignalStatus.Scale - TargetStatus.Scale).y * m_UIShowScale.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        case TargetType.To:
                            currentScale.x = OrignalStatus.Scale.x + (TargetStatus.Scale - OrignalStatus.Scale).x * m_UIShowScale.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentScale.y = OrignalStatus.Scale.y + (TargetStatus.Scale - OrignalStatus.Scale).y * m_UIShowScale.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        default:
                            currentScale.x = OrignalStatus.Scale.x + (TargetStatus.Scale - OrignalStatus.Scale).x * m_UIShowScale.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentScale.y = OrignalStatus.Scale.y + (TargetStatus.Scale - OrignalStatus.Scale).y * m_UIShowScale.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                    }

                    RectTf.localScale = currentScale;
                    //moveX = m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime) * MoveStep;
                    //moveY = m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime) * MoveStep;
                }

                if (isUIShowRotate)
                {
                    Vector3 deltaRotate;
                    switch (targetType)
                    {
                        case TargetType.To:
                            deltaRotate = TargetStatus.Rotation - OrignalStatus.Rotation;

                            currentEuler.x = OrignalStatus.Rotation.x + deltaRotate.x * m_UIShowRotate.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.y = OrignalStatus.Rotation.y + deltaRotate.y * m_UIShowRotate.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.z = OrignalStatus.Rotation.z + deltaRotate.z * m_UIShowRotate.Z.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        case TargetType.From:
                            deltaRotate = OrignalStatus.Rotation - TargetStatus.Rotation;

                            currentEuler.x = TargetStatus.Rotation.x + deltaRotate.x * m_UIShowRotate.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.y = TargetStatus.Rotation.y + deltaRotate.y * m_UIShowRotate.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.z = TargetStatus.Rotation.z + deltaRotate.z * m_UIShowRotate.Z.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                        default:
                            deltaRotate = OrignalStatus.Rotation - TargetStatus.Rotation;

                            currentEuler.x = TargetStatus.Rotation.x + deltaRotate.x * m_UIShowRotate.X.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.y = TargetStatus.Rotation.y + deltaRotate.y * m_UIShowRotate.Y.Evaluate((PassTime - T_Show) / ShowTime);
                            currentEuler.z = TargetStatus.Rotation.z + deltaRotate.z * m_UIShowRotate.Z.Evaluate((PassTime - T_Show) / ShowTime);
                            break;
                    }
                    RectTf.localRotation = Quaternion.Euler(currentEuler);
                }

            }

        }

        if (state == Enum_UIShowStatus.Show)
        {
            if (!isSendShowEvent)
            {
                if (EndShowMoveEvnet != null)
                    EndShowMoveEvnet(gameObject);
            }
            if (isAutoHide)
                if (T_Stay + StayTime < PassTime)
                {
                    HideTween();
                }
        }

        if (state == Enum_UIShowStatus.Hiding)
        {
            if (HideTime != 0)
            {
                if (isUIHideMove)
                {

                    switch (targetType)
                    {

                        case TargetType.From:
                            currentPos.x = OrignalStatus.AnchoredPos.x + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).x * m_UIHideMove_X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentPos.y = OrignalStatus.AnchoredPos.y + (TargetStatus.AnchoredPos - OrignalStatus.AnchoredPos).y * m_UIHideMove_Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        case TargetType.To:
                            currentPos.x = TargetStatus.AnchoredPos.x + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).x * m_UIHideMove_X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentPos.y = TargetStatus.AnchoredPos.y + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).y * m_UIHideMove_Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        default:
                            currentPos.x = TargetStatus.AnchoredPos.x + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).x * m_UIHideMove_X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentPos.y = TargetStatus.AnchoredPos.y + (OrignalStatus.AnchoredPos - TargetStatus.AnchoredPos).y * m_UIHideMove_Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                    }

                    //float moveX = m_UIHideMove_X.Evaluate((PassTime - T_Hide) / HideTime) * MoveStep;
                    //float moveY = m_UIHideMove_Y.Evaluate((PassTime - T_Hide) / HideTime) * MoveStep;
                    RectTf.anchoredPosition = currentPos;
                }

                if (isUIHideRotate)
                {
                    Vector3 deltaRotate;
                    switch (targetType)
                    {
                        case TargetType.From:
                            deltaRotate = TargetStatus.Rotation - OrignalStatus.Rotation;

                            currentEuler.x = OrignalStatus.Rotation.x + deltaRotate.x * m_UIHideRotate.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.y = OrignalStatus.Rotation.y + deltaRotate.y * m_UIHideRotate.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.z = OrignalStatus.Rotation.z + deltaRotate.z * m_UIHideRotate.Z.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        case TargetType.To:
                            deltaRotate = OrignalStatus.Rotation - TargetStatus.Rotation;

                            currentEuler.x = TargetStatus.Rotation.x + deltaRotate.x * m_UIHideRotate.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.y = TargetStatus.Rotation.y + deltaRotate.y * m_UIHideRotate.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.z = TargetStatus.Rotation.z + deltaRotate.z * m_UIHideRotate.Z.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        default:
                            deltaRotate = OrignalStatus.Rotation - TargetStatus.Rotation;

                            currentEuler.x = TargetStatus.Rotation.x + deltaRotate.x * m_UIHideRotate.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.y = TargetStatus.Rotation.y + deltaRotate.y * m_UIHideRotate.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            currentEuler.z = TargetStatus.Rotation.z + deltaRotate.z * m_UIHideRotate.Z.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                    }
                    RectTf.localRotation = Quaternion.Euler(currentEuler);
                }

                if (isUIHideAlpha)
                {

                    switch (targetType)
                    {
                        case TargetType.From:
                            cvsGp.alpha = OrignalStatus.Alpha + (TargetStatus.Alpha - OrignalStatus.Alpha) * m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                            //cvsGp.alpha = TargetStatus.Alpha + (OrignalStatus.Alpha - TargetStatus.Alpha) * m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        case TargetType.To:
                            cvsGp.alpha = TargetStatus.Alpha + (OrignalStatus.Alpha - TargetStatus.Alpha) * m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                            //cvsGp.alpha = OrignalStatus.Alpha + (TargetStatus.Alpha - OrignalStatus.Alpha) * m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        default:
                            cvsGp.alpha = TargetStatus.Alpha + (OrignalStatus.Alpha - TargetStatus.Alpha) * m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                    }

                    //cvsGp.alpha = m_UIHideAlpha.Evaluate((PassTime - T_Hide) / HideTime);
                }

                if (isUIHideScale)
                {
                    switch (targetType)
                    {
                        case TargetType.To:
                            currentScale.x = TargetStatus.Scale.x + (OrignalStatus.Scale - TargetStatus.Scale).x * m_UIHideScale.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentScale.y = TargetStatus.Scale.y + (OrignalStatus.Scale - TargetStatus.Scale).y * m_UIHideScale.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        case TargetType.From:
                            currentScale.x = OrignalStatus.Scale.x + (TargetStatus.Scale - OrignalStatus.Scale).x * m_UIHideScale.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentScale.y = OrignalStatus.Scale.y + (TargetStatus.Scale - OrignalStatus.Scale).y * m_UIHideScale.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                        default:
                            currentScale.x = TargetStatus.Scale.x + (OrignalStatus.Scale - TargetStatus.Scale).x * m_UIHideScale.X.Evaluate((PassTime - T_Hide) / HideTime);
                            currentScale.y = TargetStatus.Scale.y + (OrignalStatus.Scale - TargetStatus.Scale).y * m_UIHideScale.Y.Evaluate((PassTime - T_Hide) / HideTime);
                            break;
                    }

                    RectTf.localScale = currentScale;
                    //moveX = m_UIShowMove_X.Evaluate((PassTime - T_Show) / ShowTime) * MoveStep;
                    //moveY = m_UIShowMove_Y.Evaluate((PassTime - T_Show) / ShowTime) * MoveStep;
                }


                if (T_Hide + HideTime < PassTime)
                {
                    EndHideMoveAndHide();
                }
            }
        }
    }

    public void ShowTween(float showTimeScale)
    {
        MoveTimeScale = showTimeScale;
        ShowTween();
    }

    float T_Show = 0;
    float PassTime = 0;

    public void InitStatus()
    {
        if (!isSaved)
        {
            SaveOrignalStatus();
        }

        if (cvsGp != null)
            cvsGp.alpha = OrignalStatus.Alpha;
        RectTf.localScale = OrignalStatus.Scale;
        RectTf.anchoredPosition = OrignalStatus.AnchoredPos;
        RectTf.localRotation = Quaternion.Euler(OrignalStatus.Rotation);
    }

    bool isSaved = false;
    public void SaveOrignalStatus()
    {
        OrignalStatus = new UIStatus();

        if (isDataOriginal)
        {
            if (cvsGp != null)
                OrignalStatus.Alpha = OrignalStatus.Alpha;
            else
                OrignalStatus.Alpha = 1;
        }
        else
        {
            if (cvsGp != null)
                OrignalStatus.Alpha = cvsGp.alpha;
            else
                OrignalStatus.Alpha = 1;
            OrignalStatus.AnchoredPos = RectTf.anchoredPosition;
            OrignalStatus.Rotation = RectTf.localRotation.eulerAngles;
            OrignalStatus.Scale = RectTf.localScale;
        }
        isSaved = true;
    }

    public void ShowTargetStatus()
    {
        if (isUIShowAlpha)
        {
            if (cvsGp != null)
                cvsGp.alpha = TargetStatus.Alpha;
        }
        if (isUIShowScale)
        {
            RectTf.localScale = TargetStatus.Scale;
        }
        if (isUIShowMove)
            RectTf.anchoredPosition = TargetStatus.AnchoredPos;
        if (isUIShowRotate)
            RectTf.localRotation = Quaternion.Euler(TargetStatus.Rotation);

    }

    bool isSendShowEvent = false;
    public void ShowTween()
    {
        state = Enum_UIShowStatus.Showing;
        PassTime = 0;
        T_Show = PassTime;
        isSendShowEvent = false;
        //if (state != ShowState.Hide)
        //{
        InitStatus();
        //}
        //else
        //{
        //    OrignalStatus = new UIStatus();

        //    if (cvsGp != null)
        //        OrignalStatus.Alpha = cvsGp.alpha;
        //    else
        //        OrignalStatus.Alpha = 1;
        //    OrignalStatus.AnchoredPos = RectTf.anchoredPosition;
        //    OrignalStatus.Rotation = RectTf.localRotation.eulerAngles;
        //    OrignalStatus.Scale = RectTf.localScale;
        //}
        //if (originalPos == Vector3.zero)
        //{
        //    originalPos = GetComponent<RectTransform>().anchoredPosition;
        //}

        //GetComponent<RectTransform>().anchoredPosition = originalPos;
        //gameObject.SetActive(true);

        //if (ShowTime != 0)
        //{
        //    if (isUIShowMove)
        //    {
        //        //RectTf.localPosition = RectTf.localPosition + new Vector3();
        //    }
        //    if (isUIShowAlpha)
        //    {
        //        cvsGp.alpha = m_UIShowAlpha.Evaluate(0);
        //    }
        //}
        //else
        //{
        //    cvsGp.alpha = 1;
        //}
    }

    float T_Hide = 0;
    public void HideTween()
    {
        T_Hide = PassTime;
        if (HideTime != 0)
        {
            state = Enum_UIShowStatus.Hiding;

            //if (isUIHideMove)
            //{
            //    //RectTf.localPosition = RectTf.localPosition + new Vector3();
            //}
            //if (isUIHideAlpha)
            //{
            //    cvsGp.alpha = m_UIHideAlpha.Evaluate(0);
            //}
        }
        else
        {
            EndHideMoveAndHide();
        }
    }

    float T_Stay = 0;

    public void EndHideMoveAndHide()
    {
        //gameObject.SetActive(false);
        state = Enum_UIShowStatus.Hide;
        //GetComponent<RectTransform>().anchoredPosition = originalPos;
        //cvsGp.alpha = 1;
        //MoveTimeScale = 1;

        InitStatus();

        if (EndHideMoveEvent != null)
            EndHideMoveEvent(gameObject);
    }
}
