using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndoSystem : MonoBehaviour {

	public class uteUndo
	{
		public bool isMass;

		// single
		public GameObject obj;
		public string GUIDID;
		public Vector3 pos;
		public Vector3 rot;
		public bool isForInit;
		public GameObject mparent;

		// mass
		public List<GameObject> objs = new List<GameObject>();
		public List<string> GUIDIDs = new List<string>();
		public List<Vector3> poss = new List<Vector3>();
		public List<Vector3> rots = new List<Vector3>();

		public uteUndo(GameObject obj, string GUIDID, Vector3 pos, Vector3 rot, bool _isForInit = false, GameObject _parent = null)
		{
			isMass = false;

			this.obj = obj;
			this.GUIDID = GUIDID;
			this.pos = pos;
			this.rot = rot;
			this.isForInit = _isForInit;
			this.mparent = _parent;
		}

		public uteUndo(List<GameObject> objs, List<string> GUIDIDs, List<Vector3> poss, List<Vector3> rots)
		{
			isMass = true;

			this.objs = objs;
			this.GUIDIDs = GUIDIDs;
			this.poss = poss;
			this.rots = rots;
		}
	}

	public List<uteUndo> UndoHistory = new List<uteUndo>();
	public bool isUndoEnabled;
	public bool passUndoA;
	public bool passUndoB;
	private GameObject HISTORY;

	private void Start()
	{
		HISTORY = new GameObject("HISTORY");
		HISTORY.transform.position = new Vector3(-900000,0,-900000);
		isUndoEnabled = true;
		passUndoA = false;
		passUndoB = false;
	}

	public void AddToUndoDestroy(GameObject obj, string GUIDID, Vector3 pos, Vector3 rot, bool isForInit, GameObject _parent)
	{
		if(isUndoEnabled)
		{
			UndoHistory.Add(new uteUndo(obj,GUIDID,pos,rot,isForInit,_parent));
			obj.transform.parent = HISTORY.transform;
			obj.transform.position = Vector3.zero;
		}
	}

	public void AddToUndo(GameObject obj, string GUIDID, Vector3 pos, Vector3 rot)
	{
		if(isUndoEnabled)
		{
			UndoHistory.Add(new uteUndo(obj,GUIDID,pos,rot));
		}
	}

	public void AddToUndoMass(List<GameObject> objs, List<string> GUIDIDs, List<Vector3> poss, List<Vector3> rots)
	{
		if(isUndoEnabled)
		{
			List<GameObject> r_objs = new List<GameObject>();
			List<string> r_guids = new List<string>();
			List<Vector3> r_pos = new List<Vector3>();
			List<Vector3> r_rot = new List<Vector3>();

			for(int i=0;i<objs.Count;i++)
			{
				r_objs.Add(objs[i]);
				r_guids.Add(GUIDIDs[i]);
				r_pos.Add(poss[i]);
				r_rot.Add(rots[i]);
			}

			UndoHistory.Add(new uteUndo(r_objs,r_guids,r_pos,r_rot));
		}
	} 

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftCommand))
		{
			passUndoA = true;
		}

		if(Input.GetKeyDown(KeyCode.Z))
		{
			passUndoB = true;
		}

		if(Input.GetKeyUp(KeyCode.LeftCommand))
		{
			passUndoA = false;
		}

		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			passUndoA = true;
		}

		if(Input.GetKeyUp(KeyCode.LeftControl))
		{
			passUndoA = false;
		}

		if(Input.GetKeyUp(KeyCode.Z))
		{
			passUndoB = false;
		}

		if(passUndoA&&passUndoB)
		{
			passUndoB = false;

			if(UndoHistory.Count>0)
			{
				if(UndoHistory[UndoHistory.Count-1].isMass==false)
				{
					if(UndoHistory[UndoHistory.Count-1].isForInit==false)
					{
						Destroy(UndoHistory[UndoHistory.Count-1].obj);
						UndoHistory.RemoveAt(UndoHistory.Count-1);
					}
					else
					{
						UndoHistory[UndoHistory.Count-1].obj.transform.parent = UndoHistory[UndoHistory.Count-1].mparent.transform;
						UndoHistory[UndoHistory.Count-1].obj.transform.position = UndoHistory[UndoHistory.Count-1].pos;
						UndoHistory[UndoHistory.Count-1].obj.transform.localEulerAngles = UndoHistory[UndoHistory.Count-1].rot;
						UndoHistory.RemoveAt(UndoHistory.Count-1);
					}
				}
				else
				{
					for(int i=0;i<UndoHistory[UndoHistory.Count-1].objs.Count;i++)
					{
						Destroy(UndoHistory[UndoHistory.Count-1].objs[i]);
					}

					UndoHistory.RemoveAt(UndoHistory.Count-1);
				}
			}
		}
	}
}
