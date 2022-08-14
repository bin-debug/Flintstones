namespace FlintstonesBackOffice.Services
{
    public class AppState
    {
        public string Username { get; private set; }

        public event Action OnChange;

        public void SetUsername(string username)
        {
            Username = username;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
