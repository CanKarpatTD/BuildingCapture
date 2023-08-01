using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomaGames.HomaBelly.DataPrivacy
{
    public static class DataPrivacyFlowNotifier
    {
        public static bool FlowCompleted
        {
            get => PlayerPrefs.GetInt(Constants.PersistenceKey.HAS_DATAPRIVACY_FLOW_BEEN_COMPLETED, 0) == 1
                   // Backward compatibility: old GDPR used this as "flow completed"
                   || Manager.Instance.IsTermsAndConditionsAccepted();
            private set
            {
                PlayerPrefs.SetInt(Constants.PersistenceKey.HAS_DATAPRIVACY_FLOW_BEEN_COMPLETED, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        private static event Action _onFlowCompleted;
        public static event Action OnFlowCompleted
        {
            add
            {
                if (FlowCompleted)
                    value.Invoke();
                else
                    _onFlowCompleted += value;
            }
            remove
            {
                if (! FlowCompleted)
                    _onFlowCompleted -= value;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializationFlow()
        {
            if (FlowCompleted)
            {
                return;
            }
#if UNITY_EDITOR
            if(DataPrivacyUtils.IsSceneDataPrivacyScene(SceneManager.GetActiveScene().buildIndex))            
#endif
            WaitForGdprValuesSet(() =>
            {
                WaitForIdfaValueSet(SetFlowCompleted);
            });
#if UNITY_EDITOR
            else
                SetFlowCompleted();
#endif
        }

        private static void WaitForGdprValuesSet(Action after)
        {
            GdprView.OnGdprValuesSet += after;
        }

        private static void WaitForIdfaValueSet(Action after)
        {
#if UNITY_IOS
            var settingsTask = Resources.LoadAsync<Settings>(Constants.SETTINGS_RESOURCE_NAME);

            settingsTask.completed += _ =>
            {
                if ((settingsTask.asset as Settings)?.ShowIdfa == true)
                {
                    IdfaView.OnIdfaValuesSet += after.Invoke;
                }
                else
                {
                    after.Invoke();
                }
            };
#else
        after.Invoke();
#endif
        }

        private static void SetFlowCompleted()
        {
            FlowCompleted = true;
            _onFlowCompleted?.Invoke();
        }
    }
}