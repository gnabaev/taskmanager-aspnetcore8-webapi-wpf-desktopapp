﻿using System.Windows.Media.Imaging;
using TaskManager.Client.Models.Extensions;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models
{
    public class TaskClient
    {
        public TaskModel Model { get; private set; }

        public UserModel Creator {  get; set; }

        public UserModel Executor { get; set; }

        public BitmapImage Image
        {
            get
            {
                return Model.LoadImage();
            }
        }

        public bool HasFile
        {
            get => Model?.File != null;
        }

        public TaskClient(TaskModel model)
        {
            Model = model;
        }
    }
}
