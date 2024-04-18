using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectDesksPageViewModel : BindableBase
    {
        private CommonViewService viewService;
        private DesksRequestService desksRequestService;
        private UsersRequestService usersRequestService;
        private AuthToken token;
        private ProjectModel project;

        #region COMMANDS
        public DelegateCommand OpenNewDeskCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
        public DelegateCommand DeleteDeskCommand { get; private set; }
        public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
        public DelegateCommand AddNewColumnItemCommand { get; private set; }
        public DelegateCommand<object> DeleteColumnItemCommand { get; private set; }

        #endregion

        public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
        {
            this.token = token;
            this.project = project;
            viewService = new CommonViewService();
            desksRequestService = new DesksRequestService();
            usersRequestService = new UsersRequestService();

            UpdatePage();

            OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
            OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDesk);
            CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
            DeleteDeskCommand = new DelegateCommand(DeleteDesk);
            SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            DeleteColumnItemCommand = new DelegateCommand<object>(DeleteColumnItem);
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

        private List<ModelClient<DeskModel>> GetDesks(int projectId)
        {
            var result = new List<ModelClient<DeskModel>>();
            var desks = desksRequestService.GetProjectDesks(token, project.Id);
            if (desks != null)
            {
                result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();

            }

            return result;
        }

        private void OpenNewDesk()
        {
            SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
            TypeActionWithDesk = ModelClientAction.Create;
            var window = new CreateOrUpdateDeskWindow();
            viewService.OpenWindow(window, this);
        }

        private void OpenUpdateDesk(object DeskId)
        {
            SelectedDesk = GetDeskClientById(DeskId);
            TypeActionWithDesk = ModelClientAction.Update;
            var window = new CreateOrUpdateDeskWindow();
            viewService.OpenWindow(window, this);
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
            var resulAction = desksRequestService.UpdateDesk(token, SelectedDesk.Model);
            viewService.ShowActionResult(resulAction, "Desk is updated.");
        }

        private void DeleteDesk()
        {
            var resulAction = desksRequestService.DeleteDesk(token, SelectedDesk.Model.Id);
            viewService.ShowActionResult(resulAction, "Desk is deleted.");
            UpdatePage();
        }

        private void SelectPhotoForDesk()
        {
            viewService.SetPhotoForObject(SelectedDesk.Model);
            SelectedDesk = new ModelClient<DeskModel>(SelectedDesk.Model);
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
            ProjectDesks = GetDesks(project.Id);
            viewService.CurrentOpenedWindow?.Close();
        }

        private ModelClient<DeskModel> GetDeskClientById(object deskId)
        {
            try
            {
                int id = (int)deskId;
                DeskModel desk = desksRequestService.GetDesk(token, id);
                return new ModelClient<DeskModel>(desk);
            }
            catch (FormatException)
            {
                return new ModelClient<DeskModel>(null);
            }
        }
        #endregion
    }
}
