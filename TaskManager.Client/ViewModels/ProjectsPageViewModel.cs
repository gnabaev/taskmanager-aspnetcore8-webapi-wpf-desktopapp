using Prism.Commands;
using Prism.Mvvm;
using System.Net.Http.Headers;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectsPageViewModel : BindableBase
    {
        private AuthToken token;
        private UsersRequestService usersRequestService;
        private ProjectsRequestService projectsRequestService;
        private CommonViewService viewService;

        #region COMMANDS
        public DelegateCommand OpenNewProjectCommand { get; private set; }
        public DelegateCommand<object> OpenUpdateProjectCommand { get; private set; }
        public DelegateCommand<object> ShowProjectInfoCommand { get; private set; }
        public DelegateCommand CreateOrUpdateProjectCommand { get; private set; }
        public DelegateCommand DeleteProjectCommand { get; private set; }
        public DelegateCommand SelectPhotoForProjectCommand { get; private set; }
        #endregion

        public ProjectsPageViewModel(AuthToken token)
        {
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            projectsRequestService = new ProjectsRequestService();

            this.token = token;
            UserProjects = GetProjectsToClient();

            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
            ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
            CreateOrUpdateProjectCommand = new DelegateCommand(CreateOrUpdateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject);
            SelectPhotoForProjectCommand = new DelegateCommand(SelectPhotoForProject);
        }

        #region PROPERTIES
        //private List<ModelClient<ProjectModel>> userProjects = new List<ModelClient<ProjectModel>>();

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
                if (SelectedProject.Model.UserIds != null && SelectedProject.Model.UserIds.Count > 0)
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
            SelectedProject = GetProjectClienById(projectId);
            TypeActionWithProject = ModelClientAction.Update;
            var window = new CreateOrUpdateProjectWindow();
            viewService.OpenWindow(window, this);
        }

        private void ShowProjectInfo(object projectId)
        {
           SelectedProject = GetProjectClienById(projectId);
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
            UserProjects = GetProjectsToClient();
            viewService.CurrentOpenedWindow?.Close();
        }

        private ModelClient<ProjectModel> GetProjectClienById(object projectId)
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
            UserProjects = GetProjectsToClient();
        }

        private List<ModelClient<ProjectModel>> GetProjectsToClient()
        {
            return projectsRequestService.GetProjects(token).Select(p => new ModelClient<ProjectModel>(p)).ToList();
        }

        private void SelectPhotoForProject()
        {
            viewService.SetPhotoForObject(SelectedProject.Model);
            SelectedProject = new ModelClient<ProjectModel>(SelectedProject.Model);
        }
        #endregion
    }
}
