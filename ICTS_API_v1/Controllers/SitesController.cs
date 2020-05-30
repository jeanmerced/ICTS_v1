using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
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

        [HttpPost]
        public async Task<ActionResult<Site>> AddSite(SiteDTO siteDTO)
        {
            var siteNameExists = _context.Sites.Any(s => s.SiteName == siteDTO.SiteName);
            var refCoordinatesExist = false;
            if (siteDTO.RefCoordinates != null)
            {
                refCoordinatesExist = _context.Sites.Any(s => s.RefCoordinates.Equals(NpgsqlBox.Parse(siteDTO.RefCoordinates)));
            }
            
            //check if SiteName already exists
            if (siteNameExists)
            {
                //add error message
                ModelState.AddModelError("SiteName", "SiteName already exists.");
            }

            //check if RefCoordinates already exists
            if (refCoordinatesExist)
            {
                //add error message
                ModelState.AddModelError("RefCoordinates", "RefCoordinates already exist.");
            }

            //if model is not valid return error messages 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
            }

            var site = new Site
            {
                SiteName = siteDTO.SiteName,
                RefCoordinates = NpgsqlBox.Parse(siteDTO.RefCoordinates)
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSiteById),
                new { siteId = site.SiteId },
                site);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Site>>> GetAllSites()
        {
            return await _context.Sites.ToListAsync();
        }

        [Route("{SiteId}")]
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
