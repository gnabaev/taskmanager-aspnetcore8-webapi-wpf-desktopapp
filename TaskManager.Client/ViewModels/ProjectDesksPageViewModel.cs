using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class ProjectDesksPageViewModel : BindableBase
    {
        private CommonViewService viewService;
        private DesksRequestService desksRequestService;
        private AuthToken token;
        private ProjectModel project;

        #region COMMANDS

        #endregion

        public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
        {
            this.token = token;
            this.project = project;
            desksRequestService = new DesksRequestService();
            ProjectDesks = GetDesks(this.project.Id);
        }

        #region PROPERTIES

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
        #endregion
    }
}
