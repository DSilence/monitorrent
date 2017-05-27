// Helpers/Settings.cs

using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Caliburn.Micro;

namespace MonitorrentMobile.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public class Settings : PropertyChangedBase
    {
        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        #region Setting Constants

        private const string ServerUrlKey = "server_url";
        private static readonly string ServerUrlDefault = null;
        private const string TokenKey = "token";
        private const string TokenDefault = "";

        static Settings settings;
        public static Settings Current
        {
            get { return settings ?? (settings = new Settings()); }
        }

        #endregion

        public Uri ServerUrl
        {
            get
            {
                var value = AppSettings.GetValueOrDefault(ServerUrlKey, ServerUrlDefault);
                return string.IsNullOrEmpty(value) ? null : new Uri(value);
            }
            set => AppSettings.AddOrUpdateValue(ServerUrlKey, value.ToString());
        }

        public string Token
        {
            get => AppSettings.GetValueOrDefault(TokenKey, TokenDefault);
            set => AppSettings.AddOrUpdateValue(TokenKey, value);
        }

        public bool IsLoginFailed { get; set; }
    }
}