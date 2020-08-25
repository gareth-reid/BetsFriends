using System;
using System.Collections.Generic;

namespace BF_API.Data.Engine
{
    public interface IEngine<T>
    {
        //bool Execute(List<T> events);
        T Clone(T newItem, T existingItem);
        T GetFromApiId(object apiId);
    }
}
