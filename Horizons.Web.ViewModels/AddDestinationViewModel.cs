using System.ComponentModel.DataAnnotations;
using static Horizons.GCommon.ValidationConstatnts.Destination;


namespace Horizons.Web.ViewModels
{
    public class AddDestinationViewModel
    {
        [Required]
        [MinLength(NameMinLenght)]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLenght)]
        [MaxLength(DescriptionMaxLenght)]
        public string Description { get; set; } = null!;

        public string PublishedOn { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public int TerrainId { get; set; }

        public IEnumerable<TerrainViewModel>? Terrains { get; set; }

    }
}
