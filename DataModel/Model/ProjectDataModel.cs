using System.ComponentModel.DataAnnotations;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Model;
[index: Index(nameof(Name), IsUnique = true)]
public class ProjectDataModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public ProjectDataModel()
    { }

    public ProjectDataModel(IProject project)
    {
        Id = project.Id;
        Name = project.Name;
        StartDate = project.StartDate;
        EndDate = project.EndDate;
    }
}