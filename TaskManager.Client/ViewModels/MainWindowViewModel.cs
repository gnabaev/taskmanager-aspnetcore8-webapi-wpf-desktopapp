using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region COMMANDS
        public DelegateCommand OpenMyProjectsPageCommand;
        public DelegateCommand OpenMyDesksPageCommand;
        public DelegateCommand OpenMyTasksPageCommand;
        public DelegateCommand OpenMyInfoPageCommand;
        public DelegateCommand LogoutCommand;

        public DelegateCommand OpenUsersManagementCommand;

        #endregion
        public MainWindowViewModel(AuthToken token, UserModel currentUser)
        {
            Token = token;
            CurrentUser = currentUser;

            OpenMyProjectsPageCommand = new DelegateCommand(OpenMyProjectsPage);
            NavigationButtons.Add(_userProjectsButtonName, OpenMyProjectsPageCommand);

            OpenMyDesksPageCommand = new DelegateCommand(OpenMyDesksPage);
            NavigationButtons.Add(_userDesksButtonName, OpenMyDesksPageCommand);

            OpenMyTasksPageCommand = new DelegateCommand(OpenMyTasksPage); 
            NavigationButtons.Add(_userTasksButtonName, OpenMyTasksPageCommand);

            OpenMyInfoPageCommand = new DelegateCommand(OpenMyInfoPage);
            NavigationButtons.Add(_userInfoButtonName, OpenMyInfoPageCommand);

            if (currentUser.Status == UserStatus.Admin)
            {
                OpenUsersManagementCommand = new DelegateCommand(OpenUsersManagement);
                NavigationButtons.Add(_manageUsersButtonName, OpenUsersManagementCommand);
            }

            LogoutCommand = new DelegateCommand(Logout);
            NavigationButtons.Add(_logoutButtonName, LogoutCommand);
        }

        #region PROPERTIES

        private readonly string _userProjectsButtonName = "My projects";
        private readonly string _userDesksButtonName = "My desks";
        private readonly string _userTasksButtonName = "My tasks";
        private readonly string _userInfoButtonName = "My info";
        private readonly string _logoutButtonName = "Logout";

        private readonly string _manageUsersButtonName = "Users";

        private UserModel currentUser;

        public UserModel CurrentUser
        {
            get => currentUser;

            private set
            {
                currentUser = value;
                RaisePropertyChanged(nameof(currentUser));
            }
        }

        private AuthToken token;

        public AuthToken Token
        {
            get => token;

            private set
            {
                token = value;
                RaisePropertyChanged(nameof(token));
            }
        }

        private Dictionary<string, DelegateCommand> navigationButtons = new Dictionary<string, DelegateCommand>();

        public Dictionary<string, DelegateCommand> NavigationButtons
        {
            get => navigationButtons;

            set
            {
                navigationButtons = value;
                RaisePropertyChanged(nameof(navigationButtons));
            }
        }

        #endregion

        #region METHODS
        private void OpenMyProjectsPage()
        {
            ShowMessage(_userProjectsButtonName);
        }
        private void OpenMyDesksPage()
        {
            ShowMessage(_userDesksButtonName);
        }
        private void OpenMyTasksPage()
        {
            ShowMessage(_userTasksButtonName);
        }

        private void OpenMyInfoPage()
        {
            ShowMessage(_userInfoButtonName);
        }

        private void Logout()
        {
            ShowMessage(_logoutButtonName);
        }

        private void OpenUsersManagement()
        {

        }
        #endregion

        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
