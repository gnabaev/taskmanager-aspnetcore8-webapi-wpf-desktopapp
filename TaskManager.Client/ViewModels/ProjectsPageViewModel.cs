using Prism.Commands;
using Prism.Mvvm;
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
        public DelegateCommand OpenNewProjectCommand;
        public DelegateCommand<object> OpenUpdateProjectCommand;
        public DelegateCommand<object> ShowProjectInfoCommand;

        #endregion

        public ProjectsPageViewModel(AuthToken token)
        {
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            projectsRequestService = new ProjectsRequestService();

            this.token = token;
            OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
            OpenUpdateProjectCommand = new DelegateCommand<object>(UpdateProject);
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
                if (SelectedProject.Model.UserIds != null || SelectedProject.Model.UserIds.Count > 0)
                {
                    ProjectUsers = SelectedProject.Model.UserIds.Select(userId => usersRequestService.GetUser(token, userId)).ToList();
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

        private void UpdateProject(object param)
        {
            var selectedProject = param as ModelClient<ProjectModel>;
            SelectedProject = selectedProject;
            viewService.ShowMessage(nameof(UpdateProject));
        }

        private void ShowProjectInfo(object param)
        {
            var selectedProject = param as ModelClient<ProjectModel>;
            SelectedProject = selectedProject;
            viewService.ShowMessage(nameof(ShowProjectInfo));
        }

        #endregion
    }
}
