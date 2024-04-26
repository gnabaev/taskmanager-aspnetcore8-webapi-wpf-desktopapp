using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Components;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class DeskTasksPageViewModel : BindableBase
    {
        private AuthToken token;
        private DeskModel deskModel;
        private UsersRequestService usersRequestService;
        private TasksRequestService tasksRequestService;
        private ProjectsRequestService projectsRequestService;
        private CommonViewService viewService;
        private DeskTasksPage page;

        #region COMMANDS
        public DelegateCommand OpenNewTaskCommand { get; private set; }
        public DelegateCommand OpenUpdateTaskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateTaskCommand { get; private set; }
        public DelegateCommand DeleteTaskCommand { get; private set; }
        #endregion

        public DeskTasksPageViewModel(AuthToken token, DeskModel deskModel, DeskTasksPage page)
        {
            this.token = token;
            this.deskModel = deskModel;
            this.page = page;
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            tasksRequestService = new TasksRequestService();
            projectsRequestService = new ProjectsRequestService();

            TasksByColumns = GetTasksByColumns(deskModel.Id);
            page.TasksGrid.Children.Add(CreateTasksGrid());

            OpenNewTaskCommand = new DelegateCommand(OpenNewTask);
            OpenUpdateTaskCommand = new DelegateCommand(OpenUpdateTask);
            CreateOrUpdateTaskCommand = new DelegateCommand(CreateOrUpdateTask);
            DeleteTaskCommand = new DelegateCommand(DeleteTask);
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

        private ModelClient<TaskModel> selectedTask;

        public ModelClient<TaskModel> SelectedTask
        {
            get => selectedTask;

            set
            {
                selectedTask = value;
                RaisePropertyChanged(nameof(selectedTask));
            }
        }

        private ModelClientAction typeActionWithTask;

        public ModelClientAction TypeActionWithTask
        {
            get => typeActionWithTask;

            set
            {
                typeActionWithTask = value;
                RaisePropertyChanged(nameof(typeActionWithTask));
            }
        }

        private UserModel selectedTaskExecutor;

        public UserModel SelectedTaskExecutor
        {
            get => selectedTaskExecutor;

            set
            {
                selectedTaskExecutor = value;
                RaisePropertyChanged(nameof(selectedTaskExecutor));
            }
        }

        private ProjectModel project
        {
            get => projectsRequestService.GetProject(token, deskModel.ProjectId);
        }

        public List<UserModel> ProjectUsers
        {
            get => project.UserIds.Select(userId => usersRequestService.GetUser(token, userId)).ToList();
        }

        #endregion

        #region METHODS

        private void OpenNewTask()
        {
            SelectedTask = new ModelClient<TaskModel>(new TaskModel());
            TypeActionWithTask = ModelClientAction.Create;
            var window = new CreateOrUpdateTaskWindow();
            viewService.OpenWindow(window, this);
        }

        private void OpenUpdateTask()
        {
            TypeActionWithTask = ModelClientAction.Update;
            var window = new CreateOrUpdateTaskWindow();
            viewService.OpenWindow(window, this);
        }

        private void CreateOrUpdateTask()
        {
            if (typeActionWithTask == ModelClientAction.Create)
            {
                CreateTask();
            }
            if (typeActionWithTask == ModelClientAction.Update)
            {
                UpdateTask();
            }
            UpdatePage();
        }

        private void CreateTask()
        {
            SelectedTask.Model.DeskId = deskModel.Id;
            SelectedTask.Model.ExecutorId = SelectedTaskExecutor.Id;
            SelectedTask.Model.Column = deskModel.Columns.FirstOrDefault();
            var resulAction = tasksRequestService.CreateTask(token, SelectedTask.Model);
            viewService.ShowActionResult(resulAction, "Task is created.");
        }

        private void UpdateTask()
        {
            tasksRequestService.UpdateTask(token, SelectedTask.Model);
        }

        private void DeleteTask()
        {
            tasksRequestService.DeleteTask(token, SelectedTask.Model.Id);
            UpdatePage();
        }

        private void UpdatePage()
        {
            SelectedTask = null;
            TasksByColumns = GetTasksByColumns(deskModel.Id);
            page.TasksGrid.Children.Add(CreateTasksGrid());
            viewService.CurrentOpenedWindow?.Close();
        }

        private Dictionary<string, List<TaskClient>> GetTasksByColumns(int deskId)
        {
            var tasksByColumns = new Dictionary<string, List<TaskClient>>();

            var tasks = tasksRequestService.GetDeskTasks(this.token, deskId);

            foreach (string column in deskModel.Columns)
            {
                tasksByColumns.Add(column, tasks.Where(t => t.Column == column).Select(t =>
                {
                    var taskClient = new TaskClient(t);
                    taskClient.Creator = usersRequestService.GetCurrentUser(token);
                    if (t.ExecutorId != null)
                    {
                        taskClient.Executor = usersRequestService.GetUser(token, (int)t.ExecutorId);
                    }
                    return taskClient;
                }).ToList());
            }

            return tasksByColumns;
        }

        private Grid CreateTasksGrid()
        {
            var resource = new ResourceDictionary();
            resource.Source = new Uri("./Resources/Styles/MainStyle.xaml", UriKind.Relative);
            var grid = new Grid();
            var row0 = new RowDefinition();
            row0.Height = new GridLength(30);
            var row1 = new RowDefinition();

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);

            int columnsCount = 0;

            foreach (var column in TasksByColumns)
            {
                var col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);

                TextBlock header = new TextBlock();
                header.Text = column.Key;
                header.Style = resource["headerTextBlock"] as Style;

                Grid.SetRow(header, 0);
                Grid.SetColumn(header, columnsCount);

                grid.Children.Add(header);

                ItemsControl columnsControl = new ItemsControl();
                Grid.SetRow(columnsControl, 1);
                Grid.SetColumn(columnsControl, columnsCount);

                var taskViews = new List<TaskControl>();

                foreach (var task in column.Value)
                {
                    var taskView = new TaskControl(task);
                    taskViews.Add(taskView);
                }
                columnsControl.ItemsSource = taskViews;
                grid.Children.Add(columnsControl);

                columnsCount++;
            }

            return grid;
        }



        #endregion
    }
}
