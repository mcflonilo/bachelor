using System;

namespace UltraBend.Common
{
    public interface IModelId
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}