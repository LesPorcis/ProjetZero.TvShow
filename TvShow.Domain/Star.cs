using System.ComponentModel.DataAnnotations;

namespace TvShow.Domain;

public class Star
{
    [Required]
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public IReadOnlyCollection<TvShow> TvShow { get; set; } = new  List<TvShow>();
}