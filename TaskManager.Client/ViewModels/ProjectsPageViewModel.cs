using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectsPageViewModel : BindableBase
    {
        private AuthToken token;
        private UsersRequestService usersRequestService;
        private ProjectsRequestService projectsRequestService;
        private CommonViewService viewService;
        private MainWindowViewModel mainWindowViewModel;

        #region COMMANDS
        public DelegateCommand OpenNewProjectCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateProjectCommand { get; private set; }
        public DelegateCommand<object> ShowProjectInfoCommand { get; private set; }
        public DelegateCommand CreateOrUpdateProjectCommand { get; private set; }
        public DelegateCommand DeleteProjectCommand { get; private set; }
        public DelegateCommand SelectPhotoForProjectCommand { get; private set; }
        public DelegateCommand OpenUsersToProjectCommand {  get; private set; }
        public DelegateCommand AddUsersToProjectCommand { get; private set; }
        public DelegateCommand OpenProjectDesksPageCommand { get; private set; }
        #endregion

        public ProjectsPageViewModel(AuthToken token, MainWindowViewModel mainWindowVM)
        {
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            projectsRequestService = new ProjectsRequestService();
            mainWindowViewModel = mainWindowVM;

            this.token = token;
            UpdatePage();

            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
            ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
            CreateOrUpdateProjectCommand = new DelegateCommand(CreateOrUpdateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject);
            SelectPhotoForProjectCommand = new DelegateCommand(SelectPhotoForProject);
            OpenUsersToProjectCommand = new DelegateCommand(OpenUsersToProject);
            AddUsersToProjectCommand = new DelegateCommand(AddUsersToProject);
            OpenProjectDesksPageCommand = new DelegateCommand(OpenProjectDesksPage);
        }

        #region PROPERTIES

        public UserModel CurrentUser
        {
            get => usersRequestService.GetCurrentUser(token);
        }

        private ModelClientAction typeActionWithProject;

        public ModelClientAction TypeActionWithProject
        {
            get => typeActionWithProject;

            set
            {
                typeActionWithProject = value;
                RaisePropertyChanged(nameof(typeActionWithProject));
            }
        }

        private List<ModelClient<ProjectModel>> userProjects;

        public List<ModelClient<ProjectModel>> UserProjects
        {
            get => userProjects;
            set
            {
                userProjects = value;
                RaisePropertyChanged(nameof(userProjects));
            }
        }

        private ModelClient<ProjectModel> selectedProject;

        public ModelClient<ProjectModel> SelectedProject
        {
            get => selectedProject;

            set
            {
                selectedProject = value;
                RaisePropertyChanged(nameof(selectedProject));
                if (SelectedProject?.Model.UserIds != null && SelectedProject?.Model.UserIds.Count > 0)
                {
                    ProjectUsers = SelectedProject.Model.UserIds.Select(userId => usersRequestService.GetUser(token, userId)).ToList();
                }
                else
                {
                    ProjectUsers = new List<UserModel>();
                }
            }
        }

        private List<UserModel> projectUsers = new List<UserModel>();

        public List<UserModel> ProjectUsers
        {
            get => projectUsers;

            set
            {
                projectUsers = value;
                RaisePropertyChanged(nameof(projectUsers));
            }
        }

        public List<UserModel> NewUsersForSelectedProject
        {
            get => usersRequestService.GetUsers(token).Where(user => ProjectUsers.Any(u => u.Id == user.Id) == false).ToList();
        }

        private List<UserModel> selectedUsersForProject = new List<UserModel>();

        public List<UserModel> SelectedUsersForProject
        {
            get => selectedUsersForProject;

            set
            {
                selectedUsersForProject = value;
                RaisePropertyChanged(nameof(selectedUsersForProject));
            }
        }

        #endregion

        #region METHODS
        private void OpenNewProject()
        {
            SelectedProject = new ModelClient<ProjectModel>(new ProjectModel());
            TypeActionWithProject = ModelClientAction.Create;
            var window = new CreateOrUpdateProjectWindow();
            viewService.OpenWindow(window, this);
        }

        private void OpenUpdateProject(object projectId)
        {
            SelectedProject = GetProjectClientById(projectId);
            TypeActionWithProject = ModelClientAction.Update;
            var window = new CreateOrUpdateProjectWindow();
            viewService.OpenWindow(window, this);
        }

        private void ShowProjectInfo(object projectId)
        {
           SelectedProject = GetProjectClientById(projectId);
        }

        private void CreateOrUpdateProject()
        {
            if (typeActionWithProject == ModelClientAction.Create)
            {
                CreateProject();
            }
            if (typeActionWithProject == ModelClientAction.Update)
            {
                UpdateProject();
            }
            UpdatePage();
        }

        private ModelClient<ProjectModel> GetProjectClientById(object projectId)
        {
            try
            {
                int id = (int)projectId;
                ProjectModel project = projectsRequestService.GetProject(token, id);
                return new ModelClient<ProjectModel>(project);

            }
            catch (FormatException)
            {
                return new ModelClient<ProjectModel>(null);
            }
        }

        private void CreateProject()
        {
            var resulAction = projectsRequestService.CreateProject(token, SelectedProject.Model);
            viewService.ShowActionResult(resulAction, "Project is created.");
        }

        private void UpdateProject()
        {
            var resulAction = projectsRequestService.UpdateProject(token, SelectedProject.Model);
            viewService.ShowActionResult(resulAction, "Project is updated.");
        }

        private void DeleteProject()
        {
            var resulAction = projectsRequestService.DeleteProject(token, SelectedProject.Model.Id);
            viewService.ShowActionResult(resulAction, "Project is deleted.");
            UpdatePage();
        }

        private List<ModelClient<ProjectModel>> GetProjectsToClient()
        {
            viewService.CurrentOpenedWindow?.Close();
            return projectsRequestService.GetProjects(token).Select(p => new ModelClient<ProjectModel>(p)).ToList();
        }

        private void SelectPhotoForProject()
        {
            viewService.SetPhotoForObject(SelectedProject.Model);
            SelectedProject = new ModelClient<ProjectModel>(SelectedProject.Model);
        }

        private void OpenUsersToProject()
        {
            var window = new AddUsersToProjectWindow();
            viewService.OpenWindow(window, this);
        }

        private void AddUsersToProject()
        {
            if (selectedUsersForProject == null || selectedUsersForProject?.Count == 0)
            {
                viewService.ShowMessage("Select users.");
                return;
            }

            var resultAction = projectsRequestService.AddUsersToProject(token, selectedProject.Model.Id, SelectedUsersForProject.Select(u => u.Id).ToList());
            viewService.ShowActionResult(resultAction, "New users are added to project.");
            UpdatePage();
        }

        private void UpdatePage()
        {
            UserProjects = GetProjectsToClient();
            SelectedProject = null;
            SelectedUsersForProject = new List<UserModel>();
        }

        private void OpenProjectDesksPage()
        {
            if (SelectedProject?.Model != null)
            {
                var page = new ProjectDesksPage();
                mainWindowViewModel.OpenPage(page, $"Desks of {SelectedProject.Model.Name}", new ProjectDesksPageViewModel(token, SelectedProject.Model));
            }
        }
        #endregion
    }
}
