using Prism.Commands;
using Prism.Mvvm;
using System.Net.Http.Headers;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
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

        #endregion

        public ProjectsPageViewModel(AuthToken token)
        {
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            projectsRequestService = new ProjectsRequestService();

            this.token = token;
            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
            ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
        }


        #region PROPERTIES
        //private List<ModelClient<ProjectModel>> userProjects = new List<ModelClient<ProjectModel>>();

        public List<ModelClient<ProjectModel>> UserProjects
        {
            get => projectsRequestService.GetProjects(token).Select(p => new ModelClient<ProjectModel>(p)).ToList();
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
            viewService.ShowMessage(nameof(OpenNewProject));
        }

        private void OpenUpdateProject(object projectId)
        {
           SelectedProject = GetProjectClienById(projectId);
        }

        private void ShowProjectInfo(object projectId)
        {
           SelectedProject = GetProjectClienById(projectId);
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

        #endregion
    }
}
