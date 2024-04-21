using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels
{
    public class UserDesksPageViewModel : BindableBase
    {
        private AuthToken token;
        private CommonViewService commonViewService;
        private DesksRequestService desksRequestService;
        private DesksViewService desksViewService;

        #region COMMANDS
        public DelegateCommand OpenUpdateDeskCommand { get; private set; }
        public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
        public DelegateCommand DeleteDeskCommand { get; private set; }
        public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
        public DelegateCommand AddNewColumnItemCommand { get; private set; }
        public DelegateCommand<object> DeleteColumnItemCommand { get; private set; }

        #endregion
        public UserDesksPageViewModel(AuthToken token)
        {
            this.token = token;
            commonViewService = new CommonViewService();
            desksRequestService = new DesksRequestService();
            desksViewService = new DesksViewService(this.token, desksRequestService);

            OpenUpdateDeskCommand = new DelegateCommand(OpenUpdateDesk);
            CreateOrUpdateDeskCommand = new DelegateCommand(UpdateDesk);
            DeleteDeskCommand = new DelegateCommand(DeleteDesk);
            SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
            AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
            DeleteColumnItemCommand = new DelegateCommand<object>(DeleteColumnItem);
            ContextMenuCommands.Add("Update", OpenUpdateDeskCommand);
            ContextMenuCommands.Add("Delete", DeleteDeskCommand);
            UpdatePage();
        }

        #region PROPERTIES
        private List<ModelClient<DeskModel>> desks = new List<ModelClient<DeskModel>>();

        public List<ModelClient<DeskModel>> Desks
        {
            get => desks;

            set
            {
                desks = value;
                RaisePropertyChanged(nameof(desks));
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

        private Dictionary<string, DelegateCommand> contextMenuCommands = new Dictionary<string, DelegateCommand>();

        public Dictionary<string, DelegateCommand> ContextMenuCommands
        {
            get => contextMenuCommands;

            set
            {
                contextMenuCommands = value;
                RaisePropertyChanged(nameof(contextMenuCommands));
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
        private void OpenUpdateDesk()
        {
            SelectedDesk = desksViewService.GetDeskClientById(SelectedDesk.Model.Id);
            ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelper>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelper(c)));
            desksViewService.OpenViewDeskInfo(SelectedDesk.Model.Id, this);
        }

        private void UpdateDesk()
        {
            SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
            desksViewService.UpdateDesk(SelectedDesk.Model);
            UpdatePage();
        }

        private void SelectPhotoForDesk()
        {
           SelectedDesk = desksViewService.SelectPhotoForDesk(SelectedDesk);
        }

        private void DeleteDesk()
        {
            desksViewService.DeleteDesk(SelectedDesk.Model.Id);
            UpdatePage();
        }

        private void UpdatePage()
        {
            SelectedDesk = null;
            Desks = desksViewService.GetUserDesks();
            commonViewService.CurrentOpenedWindow?.Close();
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

        #endregion
    }
}
