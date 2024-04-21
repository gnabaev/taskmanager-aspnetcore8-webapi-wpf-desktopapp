using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class DesksViewService
    {
        private AuthToken token;
        private DesksRequestService desksRequestService;
        private CommonViewService commonViewService;

        public DesksViewService(AuthToken token, DesksRequestService desksRequestService) 
        { 
            this.token = token;
            this.desksRequestService = desksRequestService;
            commonViewService = new CommonViewService();
        }

        public ModelClient<DeskModel> GetDeskClientById(object deskId)
        {
            try
            {
                int id = (int)deskId;
                DeskModel desk = desksRequestService.GetDesk(token, id);
                return new ModelClient<DeskModel>(desk);
            }
            catch (FormatException)
            {
                return new ModelClient<DeskModel>(null);
            }
        }

        public List<ModelClient<DeskModel>> GetProjectDesks(int projectId)
        {
            var result = new List<ModelClient<DeskModel>>();
            var desks = desksRequestService.GetProjectDesks(token, projectId);
            if (desks != null)
            {
                result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();

            }

            return result;
        }

        public List<ModelClient<DeskModel>> GetUserDesks()
        {
            var result = new List<ModelClient<DeskModel>>();
            var desks = desksRequestService.GetUserDesks(token);
            if (desks != null)
            {
                result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
            }

            return result;
        }

        public void OpenViewDeskInfo(object deskId, BindableBase context)
        {
            var window = new CreateOrUpdateDeskWindow();
            commonViewService.OpenWindow(window, context);
        }

        public void UpdateDesk(DeskModel desk)
        {
            var resulAction = desksRequestService.UpdateDesk(token, desk);
            commonViewService.ShowActionResult(resulAction, "Desk is updated.");
        }

        public void DeleteDesk(int deskId)
        {
            var resulAction = desksRequestService.DeleteDesk(token, deskId);
            commonViewService.ShowActionResult(resulAction, "Desk is deleted.");
        }

        public ModelClient<DeskModel> SelectPhotoForDesk(ModelClient<DeskModel> selectedDesk)
        {
            if (selectedDesk?.Model != null)
            {
                commonViewService.SetPhotoForObject(selectedDesk.Model);
                selectedDesk = new ModelClient<DeskModel>(selectedDesk.Model);
            }

            return selectedDesk;
        }
    }
}
