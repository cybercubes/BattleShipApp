using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages_Boats
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Boat> Boat { get;set; } = null!;

        public async Task OnGetAsync()
        {
            Boat = await _context.Boats
                .Include(b => b.GameOption).ToListAsync();
        }
    }
}
