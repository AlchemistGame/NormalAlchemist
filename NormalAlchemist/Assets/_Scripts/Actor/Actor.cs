using UnityEngine;

namespace MyBattle
{
    public class Actor : MonoBehaviour
    {
        public ActorData data;

        public void Init(ActorData data)
        {
            this.data = data;
        }
    }
}


