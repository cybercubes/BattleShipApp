using System.Threading.Tasks;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.GamePlay
{
    public class Index : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;

        public Index(DAL.AppDbContext context,ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public GameOption GameOption { get; set; } = new GameOption();

        public async Task OnGetAsync()
        {
            GameOption = await _context.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData).FirstOrDefaultAsync() ?? new GameOption();

        }
    }
}