using UnityEngine;
using System.Collections;
//using System;

public class TrackMover : MonoBehaviour
{
    public float Speed = 1;
    public Vector3 Noise = Vector3.zero;
    public float Damping = 0.3f;

    public GameObject SkillUser;
    public GameObject Target;

    Quaternion direction;

    public int MoveId = 0;

    public MoveCurvesDict MoveDict;

    public emTrackMoveType MoveType;

    public IDirTrackMovement Movement;

    public float startMoveMentTime;
    public LineRenderer FlyLine;

    public bool isUserFire;
    void DrawFlyLine(int Value)
    {
        Vector3[] LineVec3 = new Vector3[Value];
        //FlyLine.SetVertexCount(Value);
        FlyLine.positionCount = Value;
        LineVec3[0] = transform.position;
        if (Movement.GetMoveType() == emTrackMoveType.LockPos)
        {
            for (int i = 1; i < Value; i++)
            {
                LineVec3[i] = (Movement as LockPosParabolaMoveMent).GetPos((i+1) / (float)Value * (Movement as LockPosParabolaMoveMent).ScaledTime);
            }
        }
        else
        {
            for (int i = 1; i < Value; i++)
            {
                LineVec3[i] = LineVec3[i - 1] + Movement.GetDir(i / (float)Value * Movement.GetLength()) * Speed * Movement.GetLength() / Value;
            }
        }
        FlyLine.SetPositions(LineVec3);
    }

    void Start()
    {
        if (isUserFire)
            gameObject.transform.position = SkillUser.transform.position;

        direction = Quaternion.LookRotation(this.transform.forward * 1000);
        this.transform.Rotate(new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z)));
        switch (MoveType)
        {
            case emTrackMoveType.LockPos:
                if (MoveDict.LockPosMovement.Length > MoveId && MoveId >= 0)
                {
                    Movement = MoveDict.LockPosMovement[MoveId];
                    (Movement as LockPosParabolaMoveMent).ThisPos = gameObject.transform.position;
                }
                break;
            case emTrackMoveType.Normal:
                if (MoveDict.NormalMovement.Length > MoveId && MoveId >= 0)
                {
                    Movement = MoveDict.NormalMovement[MoveId];
                }
                break;
            case emTrackMoveType.Trace:
                if (MoveDict.TraceMovement.Length > MoveId && MoveId >= 0)
                {
                    Movement = MoveDict.TraceMovement[MoveId];
                    (Movement as TraceMoveMent).Self = gameObject;
                    (Movement as TraceMoveMent).Target = Target;
                }
                break;
            default:
                if (MoveDict.NormalMovement.Length >= MoveId && MoveId > 0)
                {
                    Movement = MoveDict.NormalMovement[MoveId];
                }
                break;
        }
        // Movement = MoveDict.LockPosMovement[MoveId];
        startMoveMentTime = Time.time;
        if(FlyLine)
            DrawFlyLine(41);
    }

    void LateUpdate()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, direction, Damping);
        //this.transform.position += moveMent.GetDir(Time.time - startMoveMentTime) * Speed * Time.deltaTime;
        MoveByMoveMent();
    }

    void MoveByMoveMent()
    {
        if (Target == null)
        {
            return;
        }
        switch (MoveType)
        {
            case emTrackMoveType.LockPos:
                Vector3 DeltaV_LockPos = (Movement as LockPosParabolaMoveMent).GetPos(Time.time - startMoveMentTime);
                this.transform.LookAt(DeltaV_LockPos);
                this.transform.position = (Movement as LockPosParabolaMoveMent).GetPos(Time.time - startMoveMentTime);
                //this.transform.position += DeltaV_LockPos * Speed * Time.deltaTime;
                break;
            case emTrackMoveType.Normal:
                Vector3 DeltaV_Normal = (Movement as NormalMoveMent).GetDir(Time.time - startMoveMentTime);
                this.transform.forward = DeltaV_Normal;
                this.transform.position += DeltaV_Normal * Speed * Time.deltaTime;
                break;
            case emTrackMoveType.Trace:
                Vector3 DeltaV_Trace = (Movement as TraceMoveMent).GetDir(Time.time - startMoveMentTime);
                this.transform.forward = DeltaV_Trace;
                this.transform.position += DeltaV_Trace * Speed * Time.deltaTime;
                break;
            default:
                break;
        }
        //Debug.Log(moveMent.GetDir(Time.time - startMoveMentTime));
        //this.transform.position += moveMent.GetDir(Time.time - startMoveMentTime) * Speed * Time.deltaTime;
        //this.transform.position = moveMent.GetPos(Time.time - startMoveMentTime);
    }
}

public enum emTrackMoveType
{
    Trace,
    Normal,
    LockPos,
}

//朝某个方向前进
[System.Serializable]
public class NormalMoveMent : IDirTrackMovement
{
    public int MovementId;
    public string MovementName;
    public AnimationCurve SpeedRight;
    public AnimationCurve SpeedUp;
    public AnimationCurve SpeedForward;
    public emTrackMoveType MoveType = emTrackMoveType.Normal;
    private Vector3 Direction;

    public float ScaledTime;

    private float TureTime = 1;
    public Vector3 GetDir(float Time)
    {
        Direction.x = SpeedRight.Evaluate(TureTime * Time / ScaledTime);
        Direction.y = SpeedUp.Evaluate(TureTime * Time / ScaledTime);
        Direction.z = SpeedForward.Evaluate(TureTime * Time / ScaledTime);
        return Direction;
    }

    public emTrackMoveType GetMoveType()
    {
        return MoveType;
    }

    public float GetLength()
    {
        return ScaledTime;
    }
}

//朝某个对象追踪飞行
[System.Serializable]
public class TraceMoveMent : IDirTrackMovement
{
    public int TraceId;
    public string TraceName;
    public GameObject Target;
    public GameObject Self;

    public AnimationCurve SpeedCurve;

    public emTrackMoveType MoveType = emTrackMoveType.Trace;
    public float ScaledTime;
    public Vector3 GetDir(float Time)
    {
        if (Target == null)
        {
            return Vector3.zero;
        }
        if (Self == null)
        {
            return Vector3.zero;
        }
        return (Target.transform.position - Self.transform.position).normalized * SpeedCurve.Evaluate(Time / ScaledTime);
    }

    public emTrackMoveType GetMoveType()
    {
        return MoveType;
    }

    public float GetLength()
    {
        return ScaledTime;
    }
}

//锁定某个地点抛物线
[System.Serializable]
public class LockPosParabolaMoveMent : IDirTrackMovement, IMPosTrackMovement
{
    public int LockPosId;
    public string LockName;
    public Vector3 LockPos;
    private Vector3 _ThisPos;
    public AnimationCurve SpeedCurve;

    public emTrackMoveType MoveType = emTrackMoveType.LockPos;

    public float Rotate;
    private float World_H;

    public float H;
    private float Distance;
    public float ScaledTime;

    private Vector3 DistanceV3;
    float a;
    float b;
    bool isCalculated = false;
    float y1;
    float x1;
    public Vector3 ThisPos
    {
        get
        {
            return _ThisPos;
        }
        set
        {
            _ThisPos = value;
            isCalculated = false;
        }
    }

    private void CalculateFunc()
    {
        DistanceV3 = LockPos - _ThisPos;
        ///以目标和自己高者为准，向上增加H为最高点
        World_H =  Mathf.Max(LockPos.y, _ThisPos.y)+H;
        y1 = DistanceV3.y;
        x1 = Mathf.Sqrt(DistanceV3.z * DistanceV3.z + DistanceV3.x * DistanceV3.x);
        //float y1;
        //float x1;
        //y1 = LockPos.y;
        //x1 = LockPos.z;
        //a = y1 / (x1 * x1 + 2 * H * x1);
        //b = 2 * H * y1 / (x1 * x1 + 2 * H * x1);
        b = (x1 + x1 * Mathf.Sqrt(1 - y1 / World_H)) / (x1 * x1 / 2 / World_H);
        a = -b * b / 4 / World_H;
        isCalculated = true;
        Vector3 DirX;
        DirX.x = LockPos.x;
        DirX.z = LockPos.z;
        DirX.y = 0;
        
        Distance = Vector3.Distance(DirX, new Vector3(_ThisPos.x,0,_ThisPos.z));
    }



    public Vector3 GetPos(float Time)
    {
        if (!isCalculated)
            CalculateFunc();
        float offset;
        offset = Time / ScaledTime * Distance;

        // float y1;
        // float x1;
        // y1 = LockPos.y;
        // x1 = LockPos.z;

        //Debug.Log(a+","+b);
        return new Vector3(_ThisPos.x + DistanceV3.x * Time / ScaledTime, a * offset * offset + b * offset + _ThisPos.y, _ThisPos.z + DistanceV3.z * Time / ScaledTime);
    }

    public Vector3 GetDir(float Time)
    {
        if (!isCalculated)
            CalculateFunc();
        //Vector3 DirX;
        //DirX.x = LockPos.x;
        //DirX.z = LockPos.z;
        //DirX.y = LockPos.y;

        //float y1;
        //float x1;
        // y1 = LockPos.y;
        //x1 = LockPos.z;
        //a = y1 / (x1 * x1 + 2 * H * x1);
        //b = 2 * H * y1 / (x1 * x1 + 2 * H * x1);

        //b = (x1 + x1 * Mathf.Sqrt(1 - y1 / H)) / (x1 * x1 / 2 / H);

        //a = -b * b / 4 / H;
        //Debug.Log(a+","+b);
        return new Vector3(0, 2 * a * Time + b, Time);
    }

    public emTrackMoveType GetMoveType()
    {
        return MoveType;
    }

    public float GetLength()
    {
        return ScaledTime;
    }
}
public interface IDirTrackMovement
{
    Vector3 GetDir(float Time);
    emTrackMoveType GetMoveType();

    float GetLength();
}

public interface IMPosTrackMovement
{
    Vector3 GetPos(float Time);
}
