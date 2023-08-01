using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalVariables
{
    /// <summary>
    /// Add custom managers events here.
    /// <example> <code> public const string SomeEvent = nameof(SomeEvent); </code> </example>
    /// </summary>
    public static partial class CustomManagerEvents
    {
        public const string MovingObject = nameof(MovingObject);
        public const string ResetMergeBools = nameof(ResetMergeBools);
        
        public const string Merging = nameof(Merging);
        public const string WrongMerge = nameof(WrongMerge);
        
        public const string SpawnSoldier = nameof(SpawnSoldier);

        public const string DamageValue = nameof(DamageValue);
        public const string GiveDamage = nameof(GiveDamage);
        
        public const string SoldierAdd = nameof(SoldierAdd);
        public const string ChangeGroundColor = nameof(ChangeGroundColor);
        
        public const string TutorialEffect = nameof(TutorialEffect);
        public const string MakeCameraMove = nameof(MakeCameraMove);
    }
}
