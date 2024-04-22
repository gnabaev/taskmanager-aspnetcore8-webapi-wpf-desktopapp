using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class DeskTasksPageViewModel : BindableBase
    {
        private AuthToken token;
        private DeskModel deskModel;
        private UsersRequestService usersRequestService;
        private TasksRequestService tasksRequestService;
        private CommonViewService viewService;

        public DeskTasksPageViewModel(AuthToken token, DeskModel deskModel)
        {
            this.token = token;
            this.deskModel = deskModel;
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            tasksRequestService = new TasksRequestService();

            TasksByColumns = GetTasksByColumns(deskModel.Id);
        }

        #region PROPERTIES
        private Dictionary<string, List<TaskClient>> tasksByColumns = new Dictionary<string, List<TaskClient>>();

        public Dictionary<string, List<TaskClient>> TasksByColumns
        {
            get => tasksByColumns;

            set
            {
                tasksByColumns = value;
                RaisePropertyChanged(nameof(tasksByColumns));
            }
        }
        #endregion 

        #region METHODS
        private Dictionary<string, List<TaskClient>> GetTasksByColumns(int deskId)
        {
            var tasksByColumns = new Dictionary<string, List<TaskClient>>();

            var tasks = tasksRequestService.GetDeskTasks(this.token, deskId);

            foreach (string column in deskModel.Columns)
            {
                tasksByColumns.Add(column, tasks.Where(t => t.Column == column).Select(t => new TaskClient(t)).ToList());
            }

            return tasksByColumns;
        }

        #endregion
    }
}
