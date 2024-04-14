using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private CommonViewService viewService;

        #region COMMANDS
        public DelegateCommand OpenMyProjectsPageCommand { get; private set; }
        public DelegateCommand OpenMyDesksPageCommand { get; private set; }
        public DelegateCommand OpenMyTasksPageCommand { get; private set; }
        public DelegateCommand OpenMyInfoPageCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }

        public DelegateCommand OpenUsersManagementCommand;

        #endregion
        public MainWindowViewModel(AuthToken token, UserModel currentUser, Window currentWindow = null)
        {
            viewService = new CommonViewService();

            Token = token;
            CurrentUser = currentUser;
            this.currentWindow = currentWindow;

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

            OpenMyInfoPage();
        }

        #region PROPERTIES

        private readonly string _userProjectsButtonName = "My projects";
        private readonly string _userDesksButtonName = "My desks";
        private readonly string _userTasksButtonName = "My tasks";
        private readonly string _userInfoButtonName = "My info";
        private readonly string _logoutButtonName = "Logout";

        private readonly string _manageUsersButtonName = "Users";

        private Window currentWindow;


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

        private string selectedPageName;

        public string SelectedPageName
        {
            get => selectedPageName;

            set
            {
                selectedPageName = value;
                RaisePropertyChanged(nameof(selectedPageName));
            }
        }

        private Page selectedPage;

        public Page SelectedPage
        {
            get => selectedPage;

            set
            {
                selectedPage = value;
                RaisePropertyChanged(nameof(selectedPage));
            }
        }

        #endregion

        #region METHODS
        private void OpenMyProjectsPage()
        {
            var page = new ProjectsPage();
            OpenPage(page, _userProjectsButtonName, new ProjectsPageViewModel(Token));
        }
        private void OpenMyDesksPage()
        {
            SelectedPageName = _userDesksButtonName;
            viewService.ShowMessage(_userDesksButtonName);
        }
        private void OpenMyTasksPage()
        {
            var page = new UserTasksPage();
            OpenPage(page, _userTasksButtonName, new UserTasksPageViewModel(Token));
        }

        private void OpenMyInfoPage()
        {
            var page = new UserInfoPage();
            OpenPage(page, _userInfoButtonName, this);
        }

        private void Logout()
        {
            var question = MessageBox.Show("Are you sure?", "Logout", MessageBoxButton.YesNo);

            if (question == MessageBoxResult.Yes && currentWindow != null)
            {
                Login login = new Login();
                login.Show();
                currentWindow.Close();
            }
        }

        private void OpenUsersManagement()
        {
            SelectedPageName = _manageUsersButtonName;
            viewService.ShowMessage(_manageUsersButtonName);
        }
        #endregion

        private void OpenPage(Page page, string pageName, BindableBase viewModel)
        {
            SelectedPageName = pageName;
            SelectedPage = page;
            SelectedPage.DataContext = viewModel;
        }
    }
}
