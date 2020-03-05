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
        public ActorData CreateActor(string model_path, string tag, string name, Vector2Int coord, int speed)
        {
            Actor actor = Instantiate<Actor>(Resources.Load<Actor>(model_path));
            actor.tag = tag;
            actor.transform.SetParent(this.transform);

            CharacterData characterData = new CharacterData(name, coord, speed, actor);
            characterData.AddCard(new ActorCardData("Unity娘", "再次召唤一个Unity娘，子子孙孙无穷尽也", characterData));
            characterData.Refresh();
            allActorDataList.Add(characterData);

            return characterData;
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