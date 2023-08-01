using DG.Tweening;
using Game.GlobalVariables;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

namespace Game.Actors
{
    public class TutorialActor : Actor<LevelManager>
    {
        public GameObject hand,secondHand;

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                SoldierManager.Instance.Subscribe(CustomManagerEvents.TutorialEffect,MoveHand);
            }
            else
            {
                SoldierManager.Instance.Unsubscribe(CustomManagerEvents.TutorialEffect,MoveHand);
            }
        }

        private void MoveHand(object[] arguments)
        {
            // -7.59f, 4.11 , -2.4
            if (hand != null)
            {
                hand.transform.DOScale(1, 1f).OnComplete(() =>
                {
                    hand.transform.DOMoveZ(-15f, 0.7f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        hand.transform.DOScale(0, 1f).SetEase(Ease.InBack).OnComplete(() => { Destroy(hand); });
                    });
                });
            }
        }
    }
}