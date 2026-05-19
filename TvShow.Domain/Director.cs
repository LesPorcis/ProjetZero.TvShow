using System.ComponentModel.DataAnnotations;

namespace TvShow.Domain;

public class Director
{
    [Required]
    public int Id { get; set; }
    /// <summary>
    /// TEsts
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public IReadOnlyCollection<TvShow> TvShow { get; set; } = new  List<TvShow>();
}