using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Game.Actors
{
    public class ColorfulGroundActor : Actor<BuildingManager>
    {
        public MeshRenderer mesh;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.ChangeGroundColor,ChangeThis);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.ChangeGroundColor,ChangeThis);
            }
        }

        private void ChangeThis(object[] arguments)
        {
            var colorParse = (int) arguments[0];
            var obj = (GameObject) arguments[1];

            if (obj == gameObject)
            {
                if (colorParse == 0)// Player - Blue
                {
                    mesh.material = Manager.baseBlue;
                }
                if (colorParse == 1)//Red
                {
                    mesh.material = Manager.baseRed;
                }
                if (colorParse == 2)//Green
                {
                    mesh.material = Manager.baseGreen;
                }
                if (colorParse == 3)//Free
                {
                    mesh.material = Manager.baseFree;
                }
            }
        }
    }
}