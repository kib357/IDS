using System;
using System.ComponentModel;

namespace IDservice.Model
{
    public interface IEditableItem : IEditableObject
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
