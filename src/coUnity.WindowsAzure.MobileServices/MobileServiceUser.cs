namespace coUnity.WindowsAzure.MobileServices
{
    public class MobileServiceUser
    {
        public MobileServiceUser() {}

        internal MobileServiceUser(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}
