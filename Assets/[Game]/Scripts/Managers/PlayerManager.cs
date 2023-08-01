using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

public class PlayerManager : Manager<PlayerManager>
{
    public GameObject movingHand;
    public GameObject clickEffect;

    public GameObject mergeEffect;
    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Init, GameStatus_Init);
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
        }
        else
        {
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Init, GameStatus_Init);
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
        }
    }

    private void GameStatus_Init(object[] args)
    {
    }

    private void GameStatus_Restart(object[] args)
    {
    }

    protected override void MB_Update()
    {
        var screenPos = Input.mousePosition;
        screenPos.z = Camera.main.nearClipPlane = 2;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        movingHand.transform.position = Vector3.Lerp(movingHand.transform.position,screenPos,0.5f);

        if (Input.GetMouseButtonDown(0))
        {
            clickEffect.SetActive(true);
            movingHand.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f).OnComplete(() =>
            {
                movingHand.transform.localScale = new Vector3(1, 1, 1);
                clickEffect.SetActive(false);
            });
        }
    }
}
