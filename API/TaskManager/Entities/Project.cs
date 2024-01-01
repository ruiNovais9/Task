using System.ComponentModel.DataAnnotations;

namespace TaskManager.Entities;

public class Project
{
    [Key]
    public long Id { get; set; }
    public required string Name { get; set; }
    public long DeveloperId { get; set; }
    public DateTime? DeadLine { get; set; }
    public long TimeSpend { get; set; }
    /// <summary>
    /// If true the project is Completed, otherwise is Active
    /// </summary>
    public bool ProjectIsCompleted { get; set; }
}

public class Developer
{
    public Developer()
    {
        Name = string.Empty;
    }
    public long Id { get; set; }
    public string Name { get; set; }
}