using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICTS_API_v1.Controllers
{
    [Route("sites")]
    [ApiController]
    public class SitesController : Controller
    {
        private readonly ICTS_Context _context;

        public SitesController(ICTS_Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Site>>> GetAllSites()
        {
            return await _context.Sites.ToListAsync();
        }

        [Route("{SiteID}")]
        [HttpGet]
        public async Task<ActionResult<Site>> GetSiteById(int SiteId)
        {
            var site = await _context.Sites.FindAsync(SiteId);

            if (site == null)
            {
                return NotFound();
            }

            return site;
        }

        [HttpPost]
        public async Task<ActionResult<Site>> AddSite(SiteDTO siteDTO)
        {
            var site = new Site
            {
                SiteName = siteDTO.SiteName
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSiteById),
                new { siteId = site.SiteId },
                site);
        }

        [Route("{SiteId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveSite(int SiteId)
        {
            var site = await _context.Sites.FindAsync(SiteId);

            if (site == null)
            {
                return NotFound();
            }

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{SiteId}")]
        public async Task<IActionResult> UpdateSite(int SiteId, Site site)
        {
            if (SiteId != site.SiteId)
            {
                return BadRequest();
            }

            _context.Entry(site).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SiteExists(SiteId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool SiteExists(int SiteId) =>
             _context.Sites.Any(s => s.SiteId == SiteId);
    }
}
