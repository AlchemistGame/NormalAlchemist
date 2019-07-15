using System;
using System.Collections;
using System.Collections.Generic;

namespace MyBattle
{
    public class TurnOrderController
    {
        private class TurnOrder
        {
            public readonly ActorData actor;
            public int counter;

            public TurnOrder(ActorData actor)
            {
                this.actor = actor;
                this.counter = 0;
            }

            public void Tick()
            {
                counter += actor.speed;
            }
        }

        private const int turnActivation = 1000;    // 一个单位的行动条走到多少时可以开始回合
        private const int turnCost = 500;           // 一个单位每完成一个回合, 消耗多少行动力

        private List<TurnOrder> orderList = new List<TurnOrder>();

        public static event EventHandler roundBeginEvent;
        public static event EventHandler roundEndEvent;
        public static event EventHandler turnCompleteEvent;

        public IEnumerator Tick()
        {
            while (true)
            {
                // TODO:
                // During the status check phase, each active time-dependent status 
                // effect has its clocktick countdown decreased by 1.  Status effects whose
                // clocktick countdowns have reached zero are removed.
                if (roundBeginEvent != null)
                    roundBeginEvent(this, EventArgs.Empty);

                // 循环遍历所有单位, apply a "tick" where their counter is increased by their speed.
                for (int i = 0; i < orderList.Count; ++i)
                    orderList[i].Tick();

                // Sort the units here
                orderList.Sort((a, b) => a.counter.CompareTo(b.counter));

                // Make a temporary list of units to move, just in case the order list changes while executing moves.
                List<TurnOrder> toMove = new List<TurnOrder>();
                for (int i = orderList.Count - 1; i >= 0; --i)
                    if (orderList[i].counter >= turnActivation)
                        toMove.Add(orderList[i]);

                for (int i = toMove.Count - 1; i >= 0; --i)
                {
                    TurnOrder t = toMove[i];
                    if (toMove[i].actor.IsDead)
                        continue;

                    yield return t.actor;

                    t.counter -= turnCost;

                    if (turnCompleteEvent != null)
                        turnCompleteEvent(this, EventArgs.Empty);
                }

                if (roundEndEvent != null)
                    roundEndEvent(this, EventArgs.Empty);
            }
        }

        public void AddActor(ActorData actor)
        {
            int actorIndex = GetActorIndexInOrderList(actor);
            if (actorIndex == -1)
            {
                orderList.Add(new TurnOrder(actor));
            }
        }

        public void RemoveActor(ActorData actor)
        {
            int actorIndex = GetActorIndexInOrderList(actor);
            if (actorIndex != -1)
            {
                orderList.RemoveAt(actorIndex);
            }
        }

        // 查找是否已经有了某 actor
        private int GetActorIndexInOrderList(ActorData actor)
        {
            for (int i = 0; i < orderList.Count; i++)
            {
                if (orderList[i].actor == actor)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}