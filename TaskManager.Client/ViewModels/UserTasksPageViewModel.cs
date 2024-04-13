using Prism.Mvvm;
using TaskManager.Api.Models;
using TaskManager.Client.Models;
using TaskManager.Client.Services;

namespace TaskManager.Client.ViewModels
{
    public class UserTasksPageViewModel : BindableBase
    {
        private AuthToken token;
        private TasksRequestService tasksRequestService;
        private UsersRequestService usersRequestService;

        public UserTasksPageViewModel(AuthToken token)
        {
            this.token = token;
            tasksRequestService = new TasksRequestService();
            usersRequestService = new UsersRequestService();
        }

        public List<TaskClient> Tasks
        {
            get => tasksRequestService.GetUserTasks(token).Select(
                task => 
                {
                    var taskClient = new TaskClient(task);
                    
                    if (task.CreatorId != null)
                    {
                        int creatorId = (int)task.CreatorId;
                        taskClient.Creator = usersRequestService.GetUser(token, creatorId);
                    }

                    if (task.ExecutorId != null)
                    {
                        int executorId = (int)task.ExecutorId;
                        taskClient.Executor = usersRequestService.GetUser(token, executorId);
                    }

                    return taskClient;

                }).ToList();
        }
    }
}
