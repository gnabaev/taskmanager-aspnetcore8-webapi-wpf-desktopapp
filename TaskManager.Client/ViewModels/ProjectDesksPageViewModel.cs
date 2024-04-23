using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectDesksPageViewModel : BindableBase
    {
        private CommonViewService viewService;
        private DesksRequestService desksRequestService;
        private UsersRequestService usersRequestService;
        private DesksViewService desksViewService;
        private AuthToken token;
        private ProjectModel project;
        private MainWindowViewModel mainWindowViewModel;


        #region COMMANDS
        public DelegateCommand OpenNewDeskCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
        public DelegateCommand DeleteDeskCommand { get; private set; }
        public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
        public DelegateCommand AddNewColumnItemCommand { get; private set; }
        public DelegateCommand<object> DeleteColumnItemCommand { get; private set; }
        public DelegateCommand<object> OpenDeskTasksPageCommand { get; private set; }

        #endregion

        public ProjectDesksPageViewModel(AuthToken token, ProjectModel project, MainWindowViewModel mainWindowVM)
        {
            this.token = token;
            this.project = project;
            mainWindowViewModel = mainWindowVM;
            viewService = new CommonViewService();
            desksRequestService = new DesksRequestService();
            usersRequestService = new UsersRequestService();
            desksViewService = new DesksViewService(this.token, desksRequestService);

            UpdatePage();

            OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
            OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDesk);
            CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
            DeleteDeskCommand = new DelegateCommand(DeleteDesk);
            SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            DeleteColumnItemCommand = new DelegateCommand<object>(DeleteColumnItem);
            OpenDeskTasksPageCommand = new DelegateCommand<object>(OpenDeskTasksPage);
        }

        #region PROPERTIES

        public UserModel CurrentUser
        {
            get => usersRequestService.GetCurrentUser(token);
        }

        private List<ModelClient<DeskModel>> projectDesks = new List<ModelClient<DeskModel>>();

        public List<ModelClient<DeskModel>> ProjectDesks
        {
            get => projectDesks;

            set
            {
                projectDesks = value;
                RaisePropertyChanged(nameof(projectDesks));
            }
        }

        private ModelClientAction typeActionWithDesk;

        public ModelClientAction TypeActionWithDesk
        {
            get => typeActionWithDesk;

            set
            {
                typeActionWithDesk = value;
                RaisePropertyChanged(nameof(typeActionWithDesk));
            }
        }

        private ModelClient<DeskModel> selectedDesk;

        public ModelClient<DeskModel> SelectedDesk
        {
            get => selectedDesk;

            set
            {
                selectedDesk = value;
                RaisePropertyChanged(nameof(selectedDesk));
            }
        }

        private ObservableCollection<ColumnBindingHelper> columnsForNewDesk = new ObservableCollection<ColumnBindingHelper>()
        {
            new ColumnBindingHelper("New"),
            new ColumnBindingHelper("In progress"),
            new ColumnBindingHelper("Completed")
        };

        public ObservableCollection<ColumnBindingHelper> ColumnsForNewDesk
        {
            get => columnsForNewDesk;

            set
            {
                columnsForNewDesk = value;
                RaisePropertyChanged(nameof(columnsForNewDesk));
            }
        }

        #endregion

        #region METHODS

        private void OpenNewDesk()
        {
            SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
            TypeActionWithDesk = ModelClientAction.Create;
            var window = new CreateOrUpdateDeskWindow();
            viewService.OpenWindow(window, this);
        }

        private void OpenUpdateDesk(object deskId)
        {
            SelectedDesk = desksViewService.GetDeskClientById(deskId);

            if (CurrentUser.Id != SelectedDesk.Model.AdminId)
            {
                viewService.ShowMessage("You do not have administrator rights in this project.");
                return;
            }

            TypeActionWithDesk = ModelClientAction.Update;
            ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelper>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelper(c)));
            desksViewService.OpenViewDeskInfo(deskId, this);
        }

        private void CreateOrUpdateDesk()
        {
            if (typeActionWithDesk == ModelClientAction.Create)
            {
                CreateDesk();
            }
            if (typeActionWithDesk == ModelClientAction.Update)
            {
                UpdateDesk();
            }
            UpdatePage();
        }

        private void CreateDesk()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            SelectedDesk.Model.ProjectId = project.Id;
            var resulAction = desksRequestService.CreateDesk(token, SelectedDesk.Model);
            viewService.ShowActionResult(resulAction, "Desk is created.");
        }

        private void UpdateDesk()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            desksViewService.UpdateDesk(SelectedDesk.Model);
        }

        private void DeleteDesk()
        {
            desksViewService.DeleteDesk(SelectedDesk.Model.Id);
            UpdatePage();
        }

        public void SelectPhotoForDesk()
        {
            SelectedDesk = desksViewService.SelectPhotoForDesk(SelectedDesk);
        }

        private void AddNewColumnItem()
        {
            ColumnsForNewDesk.Add(new ColumnBindingHelper("Column"));
        }

        private void DeleteColumnItem(object item)
        {
            var itemtoDelete = item as ColumnBindingHelper;
            ColumnsForNewDesk.Remove(itemtoDelete);
        }

        private void UpdatePage()
        {
            SelectedDesk = null;
            ProjectDesks = desksViewService.GetProjectDesks(project.Id);
            viewService.CurrentOpenedWindow?.Close();
        }

        private void OpenDeskTasksPage(object deskId)
        {
            SelectedDesk = desksViewService.GetDeskClientById(deskId);

            var page = new DeskTasksPage();
            var context = new DeskTasksPageViewModel(token, SelectedDesk.Model, page);
            mainWindowViewModel.OpenPage(page, $"Tasks of {SelectedDesk.Model.Name}", context);
        }
        #endregion
    }
}
