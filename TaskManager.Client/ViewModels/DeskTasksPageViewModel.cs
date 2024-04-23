using Prism.Mvvm;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
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
        private CommonViewService viewService;
        private DeskTasksPage page;

        public DeskTasksPageViewModel(AuthToken token, DeskModel deskModel, DeskTasksPage page)
        {
            this.token = token;
            this.deskModel = deskModel;
            this.page = page;
            viewService = new CommonViewService();
            usersRequestService = new UsersRequestService();
            tasksRequestService = new TasksRequestService();

            TasksByColumns = GetTasksByColumns(deskModel.Id);
            page.TasksGrid.Children.Add(CreateTasksGrid());
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
