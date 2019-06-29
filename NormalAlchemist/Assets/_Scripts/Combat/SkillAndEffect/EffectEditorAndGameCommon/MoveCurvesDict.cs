using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MoveCurvesDict : MonoBehaviour {
    [SerializeField]
    public NormalMoveMent[] NormalMovement;

    [SerializeField]
    public TraceMoveMent[] TraceMovement;

    [SerializeField]
    public LockPosParabolaMoveMent[] LockPosMovement;
}
