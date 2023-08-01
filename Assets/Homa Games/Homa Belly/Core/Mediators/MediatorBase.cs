using System;
using JetBrains.Annotations;

namespace HomaGames.HomaBelly
{
    public abstract class MediatorBase
    {
        // Base methods
        public virtual void Initialize(Action onInitialized = null)
        {
            onInitialized?.Invoke();
        }
        public virtual void OnApplicationPause(bool pause)
        { }
        public virtual void ValidateIntegration()
        { }

        #region GDPR/CCPA
        /// <summary>
        /// Specifies if the user asserted being above the required age
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        public virtual void SetUserIsAboveRequiredAge(bool consent)
        { }

        /// <summary>
        /// Specifies if the user accepted privacy policy and terms and conditions
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        public virtual void SetTermsAndConditionsAcceptance(bool consent)
        { }

        /// <summary>
        /// Specifies if the user granted consent for analytics tracking
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        public virtual void SetAnalyticsTrackingConsentGranted(bool consent)
        { }

        /// <summary>
        /// Specifies if the user granted consent for showing tailored ads
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        public virtual void SetTailoredAdsConsentGranted(bool consent)
        { }

        #endregion

        /// <summary>
        /// Register all events and callbacks required for the
        /// mediation implementation
        /// </summary>
        public virtual void RegisterEvents()
        { }

        // Rewarded Video Ads
        public virtual void LoadExtraRewardedVideoAd([NotNull] string placement)
        { }
        public virtual void ShowRewardedVideoAd(string placement = null)
        { }
        public virtual bool IsRewardedVideoAdAvailable(string placement = null)
        {
            return false;
        }

        // Banners
        public virtual void LoadBanner(BannerSize size, BannerPosition position, string placement = null, UnityEngine.Color bannerBackgroundColor = default)
        { }
        public virtual void ShowBanner(string placement = null)
        { }
        public virtual void HideBanner(string placement = null)
        { }
        public virtual void DestroyBanner(string placement = null)
        { }
        public virtual int GetBannerHeight(string placement = null)
        {
            return 0;
        }
        
        // Interstitial
        public virtual void LoadExtraInterstitial([NotNull] string placement)
        { }
        public virtual void ShowInterstitial(string placement = null)
        { }
        public virtual bool IsInterstitialAvailable(string placement = null)
        {
            return false;
        }
    }
}
