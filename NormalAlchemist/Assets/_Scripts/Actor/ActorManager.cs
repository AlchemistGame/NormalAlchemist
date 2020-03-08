using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public class ActorManager : MonoBehaviour
    {
        public event UpdateEventHandler OnUpdate;
        public static ActorManager Instance { get; private set; }

        private List<ActorData> allActorDataList = new List<ActorData>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        #region public 接口
        public ActorData CreateActor(string prefabPath, ActorCamp camp, string name, Vector2Int coord, int speed)
        {
            CharacterData characterData = new CharacterData(name, camp, coord, speed, prefabPath);
            characterData.CreateSceneObject();
            allActorDataList.Add(characterData);
            return characterData;
        }

        public void DestroyActor(ActorData actor)
        {
            allActorDataList.Remove(actor);
        }

        public ActorData GetActorDataFrom2DCoord(Vector2Int coord)
        {
            ActorData retVal = null;

            for (int i = 0; i < allActorDataList.Count; i++)
            {
                if (allActorDataList[i].coord == coord)
                {
                    retVal = allActorDataList[i];
                    break;
                }
            }

            return retVal;
        }
        #endregion
    }
}