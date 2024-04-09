using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class LoginViewModel: BindableBase
    {
        UsersRequestService usersRequestService;

        #region COMMANDS
        public DelegateCommand<object> GetUserFromDbCommand { get; private set; }

        #endregion

        public LoginViewModel()
        {
            usersRequestService = new UsersRequestService();

            GetUserFromDbCommand = new DelegateCommand<object>(GetUserFromDb);
        }

        #region PROPERTIES
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
        #endregion

        #region METHODS
        private void GetUserFromDb(object parameter)
        {
            var passwordBox = parameter as PasswordBox;

            UserPassword = passwordBox.Password;

            AuthToken = usersRequestService.GetToken(UserLogin, UserPassword);

            if (authToken != null)
            {
                CurrentUser = usersRequestService.GetCurrentUser(authToken);

                if (CurrentUser != null)
                {
                    MessageBox.Show(CurrentUser.FirstName);
                }
            }

        }

        #endregion
    }
}
