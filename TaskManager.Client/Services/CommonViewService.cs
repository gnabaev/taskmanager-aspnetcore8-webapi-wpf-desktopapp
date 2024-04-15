using Microsoft.Win32;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class CommonViewService
    {
        private string imageDialogFilterPattern = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

        public Window CurrentOpenedWindow { get; private set; }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowActionResult(System.Net.HttpStatusCode statusCode, string message)
        {
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                ShowMessage(statusCode.ToString() + $"\n{message}");
            }
            else
            {
                ShowMessage(statusCode.ToString() + $"\nError!");
            }
        }

        public void OpenWindow(Window window, BindableBase viewModel)
        {
            CurrentOpenedWindow = window;
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        public string GetFileFromDialog(string filter)
        {
            string filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                filePath = openFileDialog.FileName;
            }

            return filePath;
        }

        public void SetPhotoForObject(CommonModel model)
        {
            string photoPath = GetFileFromDialog(imageDialogFilterPattern);

            if (string.IsNullOrEmpty(photoPath) == false)
            {
                var photoBytes = File.ReadAllBytes(photoPath);
                model.Photo = photoBytes;
            }
        }
    }
}
