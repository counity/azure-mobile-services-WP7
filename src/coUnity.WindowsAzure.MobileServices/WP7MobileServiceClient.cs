using System;
using System.IO.IsolatedStorage;

namespace coUnity.WindowsAzure.MobileServices
{
    public partial class MobileServiceClient
    {
        /// <summary>
        /// Create a new instance of MobileSericeClient and loads the application id form the isolated storage.
        /// If no application id exists it is generated and stored in the isolated storage.
        /// </summary>
        /// <param name="applicationUri"></param>
        /// <param name="applicationKey"></param>
        public MobileServiceClient(string applicationUri, string applicationKey)
            : this(new Uri(applicationUri), applicationKey)
        {
            
        }

        /// <summary>
        /// Create a new instance of MobileSericeClient and loads the application id form the isolated storage.
        /// If no application id exists it is generated and stored in the isolated storage.
        /// </summary>
        /// <param name="applicationUri"></param>
        /// <param name="applicationKey"></param>
        public MobileServiceClient(Uri applicationUri, string applicationKey)
        {
            if (applicationUri == null)
                throw new ArgumentException("applicationUri");

            ApplicationUri = applicationUri;
            ApplicationKey = applicationKey;

            // Try to get the AppInstallationId from settings
            if (IsolatedStorageSettings.ApplicationSettings.Contains(MobileServiceApplicationIdKey))
                _applicationInstallationId =
                    (string)IsolatedStorageSettings.ApplicationSettings[MobileServiceApplicationIdKey];

            // Generate a new AppInstallationId if we failed to find one
            if (string.IsNullOrEmpty(_applicationInstallationId))
            {
                _applicationInstallationId = Guid.NewGuid().ToString();
                IsolatedStorageSettings.ApplicationSettings[MobileServiceApplicationIdKey] = _applicationInstallationId;
            }
        }
    }
}
