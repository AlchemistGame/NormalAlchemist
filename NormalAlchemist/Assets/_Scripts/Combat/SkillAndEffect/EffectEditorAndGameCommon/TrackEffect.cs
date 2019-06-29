using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEffect : EffectBase {
    public emTrackType TrackType;

    Quaternion direction;
    public MoveCurvesDict MoveDict;
    public IDirTrackMovement Movement;
    public bool isUserFire;
    public emTrackMoveType MoveType;
   // public float startMoveMentTime;
    public int MoveId = 0;

    public float Speed;
    // Use this for initialization
    public override void Init () {
        base.Init();
        if (isUserFire)
            gameObject.transform.position = UserGo.transform.position;
        else
            gameObject.transform.position = UsePos;

        direction = Quaternion.LookRotation(this.transform.forward * 1000);
        //this.transform.Rotate(new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z)));
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
                    (Movement as TraceMoveMent).Target = TargetGo;
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
    }
	
	// Update is called once per frame
	void Update () {
        if (!_isInit)
        {
            Init();
        }
        if (!_isPlaying)
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

        if(Time.time - startMoveMentTime > LifeTime && LifeTime!=0)
        {
            Clear();
            CanUseSkill.EffectPool.DestoryPrefabByPool(gameObject.transform);
        }
    }
}
