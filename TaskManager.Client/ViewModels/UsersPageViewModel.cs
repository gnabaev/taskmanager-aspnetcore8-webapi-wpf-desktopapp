using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class UsersPageViewModel : BindableBase
    {
        private AuthToken token;
        private UsersRequestService usersRequestService;
        private CommonViewService viewService;
        private ExcelService excelService;

        #region COMMANDS
        public DelegateCommand OpenNewUserCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateUserCommand { get; private set; }
        public DelegateCommand CreateOrUpdateUserCommand { get; private set; }
        public DelegateCommand<object> DeleteUserCommand { get; private set; }
        public DelegateCommand OpenSelectUsersFromExcelCommand { get; private set; }
        public DelegateCommand GetUsersFromExcelCommand {get; private set; }
        public DelegateCommand AddUsersFromExcelCommand { get; private set; }
        #endregion

        public UsersPageViewModel(AuthToken token)
        {
            this.token = token;
            usersRequestService = new UsersRequestService();
            viewService = new CommonViewService();
            excelService = new ExcelService();

            OpenNewUserCommand = new DelegateCommand(OpenNewUser);
            OpenUpdateUserCommand = new DelegateCommand<object>(OpenUpdateUser);
            CreateOrUpdateUserCommand = new DelegateCommand(CreateOrUpdateUser);
            DeleteUserCommand = new DelegateCommand<object>(DeleteUser);
            OpenSelectUsersFromExcelCommand = new DelegateCommand(OpenSelectUsersFromExcel);
            GetUsersFromExcelCommand = new DelegateCommand(GetUsersFromExcel);
            AddUsersFromExcelCommand = new DelegateCommand (AddUsersFromExcel);
            Users = usersRequestService.GetUsers(this.token);
        }

        #region PROPERTIES

        private List<UserModel> users = new List<UserModel>();

        public List<UserModel> Users
        {
            get => users;

            set
            {
                users = value;
                RaisePropertyChanged(nameof(users));
            }

        }

        private UserModel selectedUser;

        public UserModel SelectedUser
        {
            get => selectedUser;

            set
            {
                selectedUser = value;
                RaisePropertyChanged(nameof(selectedUser));
            }
        }

        private List<UserModel> usersFromExcel;

        public List<UserModel> UsersFromExcel
        {
            get => usersFromExcel;

            set
            {
                usersFromExcel = value;
                RaisePropertyChanged(nameof(usersFromExcel));
            }
        }

        private List<UserModel> selectedUsersFromExcel = new List<UserModel>();

        public List<UserModel> SelectedUsersFromExcel
        {
            get => selectedUsersFromExcel;

            set 
            { 
                selectedUsersFromExcel = value;
                RaisePropertyChanged(nameof(selectedUsersFromExcel));
            }
        }

        private ModelClientAction typeActionWithUser;

        public ModelClientAction TypeActionWithUser
        {
            get => typeActionWithUser;

            set
            {
                typeActionWithUser = value;
                RaisePropertyChanged(nameof(typeActionWithUser));
            }
        }

        #endregion

        #region METHODS
        private void OpenNewUser()
        {
            TypeActionWithUser = ModelClientAction.Create;
            SelectedUser = new UserModel();
            var window = new CreateOrUpdateUserWindow();
            viewService.OpenWindow(window, this);
        }

        private void OpenUpdateUser(object user)
        {
            if (user != null)
            {
                TypeActionWithUser = ModelClientAction.Update;
                SelectedUser = usersRequestService.GetUser(token, ((UserModel)user).Id);

                var window = new CreateOrUpdateUserWindow();
                viewService.OpenWindow(window, this);
            }
        }

        private void CreateOrUpdateUser()
        {
            if (typeActionWithUser == ModelClientAction.Create)
            {
                usersRequestService.CreateUser(token, SelectedUser);
            }
            if (typeActionWithUser == ModelClientAction.Update)
            {
                usersRequestService.UpdateUser(token, SelectedUser);
            }
            UpdatePage();
        }

        private void UpdatePage()
        {
            viewService.CurrentOpenedWindow?.Close();
            SelectedUser = null;
            SelectedUsersFromExcel = new List<UserModel>();
            Users = usersRequestService.GetUsers(token);
        }

        private void DeleteUser(object user)
        {
            if (user != null)
            {
                SelectedUser = usersRequestService.GetUser(token, ((UserModel)user).Id);
                usersRequestService.DeleteUser(token, SelectedUser.Id);
                UpdatePage();
            }
        }

        private void OpenSelectUsersFromExcel()
        {
            var window = new UsersFromExcelWindow();
            viewService.OpenWindow(window, this);
        }

        private void GetUsersFromExcel()
        {
            string excelDialogFilterPattern = "Excel Files(.xls)|*.xls| Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm";
            string path = viewService.GetFileFromDialog(excelDialogFilterPattern);
            if (!string.IsNullOrEmpty(path))
            {
                UsersFromExcel = excelService.GetUsersFromExcel(path);
            }
        }

        private void AddUsersFromExcel()
        {
            if (SelectedUsersFromExcel != null && SelectedUsersFromExcel.Count > 0)
            {
                var result = usersRequestService.CreateUsers(token, SelectedUsersFromExcel);
                viewService.ShowActionResult(result, "Users are created.");
                UpdatePage();
            }
        }

        #endregion
    }
}
