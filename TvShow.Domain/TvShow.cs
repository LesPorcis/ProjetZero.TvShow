using System.ComponentModel.DataAnnotations;

namespace TvShow.Domain;

public class TvShow
{
    [Required]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? ReleasedAt { get; set; }
    public int Seasons { get; set; }
    public int Episodes { get; set; }
    
    public IReadOnlyCollection<Director> Directors { get; set; } = new List<Director>();
    public IReadOnlyCollection<Writer> Writers { get; set; } = new List<Writer>();
    public IReadOnlyCollection<Star> Stars { get; set; } = new List<Star>();
    public IReadOnlyCollection<Genre> Genres { get; set; } = new List<Genre>();
}