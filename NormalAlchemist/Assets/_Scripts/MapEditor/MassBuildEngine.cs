using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassBuildEngine : MonoBehaviour
{
    private List<VoxelInfo> newTiles = new List<VoxelInfo>();
    private List<GameObject> dummyTrash = new List<GameObject>();
    private GameObject mDummy;
    private GameObject mTile;
    private string mGuid;
    private UndoSystem UndoSystem;
    private Vector2 lastDistance;
    private string xzCountLabel;
    private bool isBuilding;
    private int heightStep;

    // reminders
    private int lastDistanceX;
    private int lastDistanceZ;
    private int lastXfill;
    private int lastZfill;
    private GameObject lastObj;
    private bool isAboutToCancel;
    private Material mDummyMat;

    private void Start()
    {
        isAboutToCancel = false;
        lastXfill = 0;
        lastZfill = 0;
        lastDistanceZ = 0;
        lastDistanceX = 0;
        heightStep = 1;
        isBuilding = false;
        lastDistance = new Vector3(-10000, -10000, -10000);
        mDummy = (GameObject)Resources.Load("uteForEditor/uteTcDummy");
        mDummyMat = (Material)Resources.Load("uteForEditor/uteTcDummyMat");
        UndoSystem = this.gameObject.GetComponent<UndoSystem>();
    }

    public void massBuildStart(GameObject _mTile, string _mGuid)
    {
        mDummy = Instantiate(_mTile);
        isAboutToCancel = false;
        isBuilding = true;
        isItFirst = true;
        mTile = _mTile;
        mGuid = _mGuid;
        lastXfill = 0;
        lastZfill = 0;

        if (newTiles.Count > 0)
        {
            newTiles.Clear();
        }

        StartCoroutine(AddTile(_mTile));
    }

    private bool isItFirst;
    private Vector3 startPosition;

    public IEnumerator AddTile(GameObject obj, bool refresh = false)
    {
        bool isGoodToGo = true;

        if (isItFirst && !isAboutToCancel)
        {
            lastXfill = 0;
            lastZfill = 0;
            lastObj = obj;
            isGoodToGo = false;
            isItFirst = false;

            startPosition = obj.transform.position;
            newTiles.Add(new VoxelInfo(new VoxelPos(startPosition)));

            if (mDummy)
            {
                GameObject newDummy = Instantiate(mDummy, startPosition + new Vector3(0, 0.03f, 0), obj.transform.rotation);
                newDummy.transform.localScale = obj.transform.localScale + obj.GetComponent<Collider>().bounds.size;

                if (newDummy.transform.localScale.y < 0.5f)
                {
                    newDummy.transform.localScale += new Vector3(0.04f, 0.04f, 0.04f);
                }
                else
                {
                    newDummy.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (newDummy.GetComponent<BoxCollider>() == false)
                {
                    newDummy.AddComponent<BoxCollider>();
                }

                dummyTrash.Add(newDummy);

                int Xdistance = (int)Mathf.Ceil(obj.transform.position.x - startPosition.x);
                int Zdistance = (int)Mathf.Ceil(obj.transform.position.z - startPosition.z);

                lastDistanceX = Xdistance;
                lastDistanceZ = Zdistance;
                lastDistance = new Vector2(Xdistance, Zdistance);

                StartCoroutine(AddTile(null, true));
            }
        }

        if ((isGoodToGo || refresh) && !isAboutToCancel)
        {
            int Xdistance = 0;
            int Zdistance = 0;

            if (!refresh)
            {
                lastObj = obj;
                Xdistance = (int)Mathf.Ceil(obj.transform.position.x - startPosition.x);
                Zdistance = (int)Mathf.Ceil(obj.transform.position.z - startPosition.z);
                lastDistanceX = Xdistance;
                lastDistanceZ = Zdistance;
            }
            else
            {
                Xdistance = lastDistanceX;
                Zdistance = lastDistanceZ;
            }

            Vector2 newDistance = new Vector2(Xdistance, Zdistance);

            if ((newDistance != lastDistance) || refresh)
            {
                if (dummyTrash.Count > 0)
                {
                    for (int i = 0; i < dummyTrash.Count; i++)
                    {
                        GameObject go = dummyTrash[i];
                        Destroy(go);
                    }

                    dummyTrash.Clear();
                }

                if (newTiles.Count > 0)
                {
                    newTiles.Clear();
                }

                lastDistance = newDistance;
                int Xfill = 0;
                int Zfill = 0;

                if (!refresh)
                {
                    Xfill = (int)(Mathf.Abs(Xdistance) / obj.GetComponent<Collider>().bounds.size.x);
                    Zfill = (int)(Mathf.Abs(Zdistance) / obj.GetComponent<Collider>().bounds.size.z);
                    lastXfill = Xfill;
                    lastZfill = Zfill;
                }
                else
                {
                    Xfill = lastXfill;
                    Zfill = lastZfill;
                }

                string pM = "";

                if (Xdistance >= 0 && Zdistance >= 0)
                {
                    pM = "++";
                }
                else if (Xdistance <= 0 && Zdistance <= 0)
                {
                    pM = "--";
                }
                else if (Xdistance <= 0 && Zdistance >= 0)
                {
                    pM = "-+";
                }
                else if (Xdistance >= 0 && Zdistance <= 0)
                {
                    pM = "+-";
                }

                if (heightStep <= 0)
                {
                    heightStep = 1;
                }

                xzCountLabel = "X:" + (Xfill + 1) + " Z:" + (Zfill + 1) + " Y:" + heightStep;

                if (refresh)
                {
                    obj = lastObj;
                }

                for (int k = 0; k < heightStep; k++)
                {
                    for (int i = 0; i < Mathf.Abs(Xfill) + 1; i++)
                    {
                        for (int j = 0; j < Mathf.Abs(Zfill) + 1; j++)
                        {
                            Vector3 position = Vector3.zero;

                            float addPosY = k * obj.GetComponent<Collider>().bounds.size.y;

                            if (pM.Equals("--"))
                            {
                                position = new Vector3(startPosition.x - (obj.GetComponent<Collider>().bounds.size.x * i), obj.transform.position.y, startPosition.z - (obj.GetComponent<Collider>().bounds.size.z * j));
                            }
                            else if (pM.Equals("++"))
                            {
                                position = new Vector3(startPosition.x + (obj.GetComponent<Collider>().bounds.size.x * i), obj.transform.position.y, startPosition.z + (obj.GetComponent<Collider>().bounds.size.z * j));
                            }
                            else if (pM.Equals("-+"))
                            {
                                position = new Vector3(startPosition.x - (obj.GetComponent<Collider>().bounds.size.x * i), obj.transform.position.y, startPosition.z + (obj.GetComponent<Collider>().bounds.size.z * j));
                            }
                            else if (pM.Equals("+-"))
                            {
                                position = new Vector3(startPosition.x + (obj.GetComponent<Collider>().bounds.size.x * i), obj.transform.position.y, startPosition.z - (obj.GetComponent<Collider>().bounds.size.z * j));
                            }

                            newTiles.Add(new VoxelInfo(new VoxelPos(position + new Vector3(0, addPosY, 0))));

                            if (mDummy)
                            {
                                GameObject newDummy = Instantiate(mDummy, position + new Vector3(0, 0.03f, 0) + new Vector3(0, addPosY, 0), obj.transform.rotation);
                                MeshRenderer[] mrs = mDummy.GetComponentsInChildren<MeshRenderer>();

                                for (int m = 0; m < mrs.Length; m++)
                                {
                                    mrs[m].GetComponent<Renderer>().material = mDummyMat;
                                }

                                SkinnedMeshRenderer[] ssmrs = mDummy.GetComponentsInChildren<SkinnedMeshRenderer>();

                                for (int sm = 0; sm < ssmrs.Length; sm++)
                                {
                                    ssmrs[sm].GetComponent<Renderer>().material = mDummyMat;
                                }

                                newDummy.transform.localScale = obj.transform.localScale;

                                if (newDummy.transform.localScale.y < 0.5f)
                                {
                                    newDummy.transform.localScale += new Vector3(0.04f, 0.04f, 0.04f);
                                }
                                else
                                {
                                    newDummy.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                                }

                                if (newDummy.GetComponent<BoxCollider>() == false)
                                {
                                    newDummy.AddComponent<BoxCollider>();
                                }

                                newDummy.layer = 2;
                                dummyTrash.Add(newDummy);
                            }
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.4f);
    }

    public void StepDown()
    {
        if (heightStep > 1)
        {
            heightStep -= 1;

            if (isBuilding)
            {
                StartCoroutine(AddTile(null, true));
            }
        }
    }

    public void StepUp()
    {
        heightStep += 1;

        if (isBuilding)
        {
            StartCoroutine(AddTile(null, true));
        }
    }

    public void StepOne()
    {
        heightStep = 1;

        if (isBuilding)
        {
            StartCoroutine(AddTile(null, true));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StepDown();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StepUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StepOne();
        }

        if (isBuilding)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (isBuilding && !isAboutToCancel)
                {
                    FinishUp();
                }
            }
        }
    }

    public void FinishUp()
    {
        isItFirst = true;

        if (mDummy)
        {
            Destroy(mDummy);
        }

        if (newTiles.Count > 0)
        {
            GlobalSettings.UndoSession = 1;

            for (int i = 0; i < newTiles.Count; i++)
            {
                VoxelInfo tile = newTiles[i];

                if (newTiles.Count == 1)
                {
                    GlobalSettings.UndoSession = 2;
                }

                if (mTile != null)
                {
                    MapManager.Instance.PlaceBlock(tile, mTile.GetComponent<TagMapObject>());

                    GlobalSettings.mapObjectCount++;
                }

                if (i == newTiles.Count - 2)
                {
                    GlobalSettings.UndoSession = 2;
                }
            }

            if (newTiles.Count > 0)
            {
                newTiles.Clear();
            }

            if (dummyTrash.Count > 0)
            {
                for (int i = 0; i < dummyTrash.Count; i++)
                {
                    GameObject go = dummyTrash[i];
                    Destroy(go);
                }

                dummyTrash.Clear();
            }
        }

        isBuilding = false;
    }
}