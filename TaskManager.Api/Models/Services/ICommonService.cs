﻿namespace TaskManager.Api.Models.Services
{
    public interface ICommonService<T>
    {
        bool Create(T model);

        bool Update(int id, T model);

        bool Delete(int id);

        T Get(int id);
    }
}
