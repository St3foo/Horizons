using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizons.Web.ViewModels
{
    public class FavoritesViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Terrain { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}
