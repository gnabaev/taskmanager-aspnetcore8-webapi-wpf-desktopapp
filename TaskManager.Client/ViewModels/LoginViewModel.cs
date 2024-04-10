using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        UsersRequestService usersRequestService;

        #region COMMANDS
        public DelegateCommand<object> GetUserFromDbCommand { get; private set; }

        public DelegateCommand<object> LoginFromCacheCommand { get; private set; }

        #endregion

        public LoginViewModel()
        {
            usersRequestService = new UsersRequestService();
            CurrentUserCache = GetUserCache();

            GetUserFromDbCommand = new DelegateCommand<object>(GetUserFromDb);
            LoginFromCacheCommand = new DelegateCommand<object>(LoginFromCache);
        }

        #region PROPERTIES

        private string cachePath = Path.GetTempPath() + "usertaskmanager.txt";

        private Window currentWindow;

        public string UserLogin { get; set; }

        public string UserPassword { get; set; }

        private UserModel currentUser;

        public UserModel CurrentUser
        {
            get => currentUser;

            set
            {
                currentUser = value;
                RaisePropertyChanged(nameof(currentUser));
            }
        }

        private AuthToken authToken;

        public AuthToken AuthToken
        {
            get => authToken;

            set
            {
                authToken = value;
                RaisePropertyChanged(nameof(authToken));
            }
        }

        private UserCache currentUserCache;

        public UserCache CurrentUserCache
        {
            get => currentUserCache;

            set
            {
                currentUserCache = value;
                RaisePropertyChanged(nameof(currentUserCache));
            }
        }

        #endregion

        #region METHODS
        private void GetUserFromDb(object parameter)
        {
            var passwordBox = parameter as PasswordBox;

            bool isNewUser = false;

            currentWindow = Window.GetWindow(passwordBox);

            if (UserLogin != CurrentUserCache?.Login || UserPassword != CurrentUserCache?.Password)
            {
                isNewUser = true;
            }

            UserPassword = passwordBox.Password;

            AuthToken = usersRequestService.GetToken(UserLogin, UserPassword);

            if (authToken == null)
            {
                return;
            }

            CurrentUser = usersRequestService.GetCurrentUser(authToken);

            if (CurrentUser != null)
            {
                if (isNewUser)
                {
                    var saveUserCache = MessageBox.Show("Сохранить логин и пароль?", "Сохранение данных", MessageBoxButton.YesNo);

                    if (saveUserCache == MessageBoxResult.Yes)
                    {
                        UserCache newUserCache = new UserCache()
                        {
                            Login = UserLogin,
                            Password = UserPassword,
                        };

                        CreateUserCache(newUserCache);
                    }
                }

                OpenMainWindow();
            }
        }

        private void CreateUserCache(UserCache userCache)
        {
            string jsonUserCache = JsonConvert.SerializeObject(userCache);

            using (StreamWriter sw = new StreamWriter(cachePath, false, Encoding.Default))
            {
                sw.Write(jsonUserCache);
                MessageBox.Show("Успех!");
            }
        }

        private UserCache GetUserCache()
        {
            bool isCacheExist = File.Exists(cachePath);

            if (isCacheExist && File.ReadAllText(cachePath).Length > 0)
            {
                return JsonConvert.DeserializeObject<UserCache>(File.ReadAllText(cachePath));
            }

            return null;
        }

        private void LoginFromCache(object window)
        {
            currentWindow = window as Window;
            UserLogin = currentUserCache.Login;
            UserPassword = currentUserCache.Password;
            AuthToken = usersRequestService.GetToken(UserLogin, UserPassword);

            CurrentUser = usersRequestService.GetCurrentUser(AuthToken);
            if (CurrentUser != null)
            {
                OpenMainWindow();
            }
        }

        private void OpenMainWindow()
        {
            MainWindow window = new MainWindow();
            window.Show();
            currentWindow.Close();
        }

        #endregion
    }
}
