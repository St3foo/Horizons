using System.ComponentModel.DataAnnotations;

namespace Horizons.Web.ViewModels
{
    public class DeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Publisher { get; set; }

        [Required]
        public string PublisherId { get; set; } = null!;
    }
}
