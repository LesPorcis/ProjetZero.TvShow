using System.ComponentModel.DataAnnotations;

namespace TvShow.Domain;

public class Genre
{
    [Required]
    public int Id { get; set; }
    
    // Forcer l'unicité du nom
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Test PR
    /// </summary>
    public IReadOnlyCollection<TvShow> TvShow { get; set; } = new  List<TvShow>();
}