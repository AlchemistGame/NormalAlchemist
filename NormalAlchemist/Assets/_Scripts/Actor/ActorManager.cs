using UnityEngine;

namespace MyBattle
{
    public class ActorManager : MonoBehaviour
    {
        public event UpdateEventHandler OnUpdate;
        public static ActorManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        public ActorData CreateActor(string model_path, string tag, string name, Int3 coord, int speed)
        {
            Actor actor = Instantiate<Actor>(Resources.Load<Actor>(model_path));
            actor.tag = tag;
            actor.transform.SetParent(this.transform);

            CharacterData characterData = new CharacterData(name, coord, speed, actor);
            characterData.AddCard(new ActorCardData("Unity娘", "再次召唤一个Unity娘，子子孙孙无穷尽也", characterData));
            characterData.Refresh();

            return characterData;
        }
    }
}