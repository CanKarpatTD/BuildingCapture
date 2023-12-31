﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HomaGames.HomaBelly.DataPrivacy
{
    public class GdprView : MonoBehaviour
    {
        [SerializeField] private GameObject LoadingPane;
        [SerializeField] private GameObject progressBar;
        [SerializeField] private Button AcceptAndPlayButton;

        #region OnGdprValuesSet definition 
        private static bool _gdprValuesSet;
        private static bool GdprValuesSet
        {
            get => _gdprValuesSet;
            set
            {
                bool fromFalseToTrue = _gdprValuesSet != value && value;
                _gdprValuesSet = value;
                
                if (fromFalseToTrue)
                    _onGdprValuesSet?.Invoke();
            }
        }
        public static event Action _onGdprValuesSet;
        public static event Action OnGdprValuesSet
        {
            add
            {
                if (GdprValuesSet)
                    value.Invoke();
                
                _onGdprValuesSet += value;
            }
            remove => _onGdprValuesSet -= value;
        }
        #endregion

        private Settings Settings;
        private bool InternetReachable => Application.internetReachability != NetworkReachability.NotReachable;

        private Task GdprProtectedFetchTask;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void MakeSureThatFLowProceeds()
        {
            if (!DataPrivacyFlowNotifier.FlowCompleted)
            {
                Settings settings = await ResourcesUtils.LoadAsync<Settings>(Constants.SETTINGS_RESOURCE_NAME);
                if (!settings.GdprEnabled || settings.ForceDisableGdpr)
                {
                    ModelSaver.SetAllAccepted();
                    ModelSaver.Save();
                    
                    GdprValuesSet = true;
                }
            }
        }

        private void Awake()
        {
            if (InternetReachable)
            {
                GdprProtectedFetchTask = Manager.Instance.FetchIsGdprProtectedRegion();
            }
        }
        
        private async void Start()
        {
            var loadRequest = ResourcesUtils.LoadAsync<Settings>(Constants.SETTINGS_RESOURCE_NAME);
            UpdateAcceptAndPlayButton();

            if (GdprProtectedFetchTask != null)
            {
                await GdprProtectedFetchTask;
            }

            if (GdprProtectedFetchTask == null || !Manager.IsGdprProtectedRegion)
            {
                SkipDataPrivacyWelcomeView();
                return;
            }
            
            Settings = await loadRequest;
            // Only show the progress bar image if IDFA Prepopup is enabled
            bool shouldShowProgressBar = Settings.ShowIdfaPrePopup // If enabled in manifest
                                         && InternetReachable // with internet connection
                                         && Manager.Instance.IsiOS14_5OrHigher   // Only for iOS 14.5+
                                         && AppTrackingTransparency.TrackingAuthorizationStatus == AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED; // If global setting for tracking is enabled

#if !UNITY_IOS
            shouldShowProgressBar = false;
#endif
            
            progressBar.SetActive(shouldShowProgressBar);
            LoadingPane.SetActive(false);
        }

        public void OnTailoredAdsAcceptanceChanged(bool arg0)
        {
            ModelSaver.SetTailoredAdsAccepted(arg0);
            UpdateAcceptAndPlayButton();
        }

        public void OnAnalyticsAcceptanceChanged(bool arg0)
        {
            ModelSaver.SetAnalyticsAccepted(arg0);
            UpdateAcceptAndPlayButton();
        }

        public void OnPrivacyPolicyChanged(bool arg0)
        {
            ModelSaver.SetIsAboveRequiredAge(arg0);
            ModelSaver.SetTermsAndConditionsAccepted(arg0);
            UpdateAcceptAndPlayButton();
        }

        public void OnAcceptAndPlayButtonClicked()
        {
            SceneClose();
        }

        private void UpdateAcceptAndPlayButton()
        {
            AcceptAndPlayButton.interactable = Manager.Instance.IsAnalyticsGranted() &&
                                               Manager.Instance.IsTailoredAdsGranted()
                                               && Manager.Instance.IsTermsAndConditionsAccepted();
        }

        private void SkipDataPrivacyWelcomeView()
        {
            ModelSaver.SetAllAccepted();

            SceneClose();
        }

        private void SceneClose()
        {
            ModelSaver.Save();
            GdprValuesSet = true;

            DataPrivacyUtils.LoadNextScene();
        }

        private static class ModelSaver
        {
            public static void SetTailoredAdsAccepted(bool accepted) =>
                PlayerPrefs.SetInt(Constants.PersistenceKey.TAILORED_ADS, accepted ? 1 : 0);
            public static void SetAnalyticsAccepted(bool accepted) =>
                PlayerPrefs.SetInt(Constants.PersistenceKey.ANALYTICS_TRACKING, accepted ? 1 : 0);
            public static void SetIsAboveRequiredAge(bool yes) =>
                PlayerPrefs.SetInt(Constants.PersistenceKey.ABOVE_REQUIRED_AGE, yes ? 1 : 0);
            public static void SetTermsAndConditionsAccepted(bool accepted) =>
                PlayerPrefs.SetInt(Constants.PersistenceKey.TERMS_AND_CONDITIONS, accepted ? 1 : 0);

            public static void Save() => PlayerPrefs.Save();

            public static void SetAllAccepted()
            {
                SetTailoredAdsAccepted(true);
                SetAnalyticsAccepted(true);
                SetIsAboveRequiredAge(true);
                SetTermsAndConditionsAccepted(true);
            }
        }
    }
}