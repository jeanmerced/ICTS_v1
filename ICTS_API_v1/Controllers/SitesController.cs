using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
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
        public async Task<ActionResult<SiteDetailsDTO>> AddSite(SiteDTO siteDTO)
        {
            try
            {
                var siteNameExists = _context.Sites.Any(s => s.SiteName == siteDTO.SiteName);

                //check if SiteName already exists
                if (siteNameExists)
                {
                    //add error message
                    ModelState.AddModelError("SiteName", "SiteName already exists.");
                }

                //try to parse given ref coordinates to npgsql box
                var coords = new NpgsqlBox();
                if (siteDTO.RefCoordinates != null)
                {
                    try
                    {
                        coords = NpgsqlBox.Parse(siteDTO.RefCoordinates);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("{0} Exception caught.", e);
                        //add error message
                        ModelState.AddModelError("RefCoordinates", "Invalid input: RefCoordinates must be specified using the following syntax \'((x1,y1),(x2,y2))\' where (x1,y1) and (x2,y2) are any two opposite corners.");
                        return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                    }
                }

                if (ModelState.IsValid)
                {
                    var refCoordinatesExist = _context.Sites.Any(s => s.RefCoordinates.Equals(coords));

                    //check if RefCoordinates already exists
                    if (refCoordinatesExist)
                    {
                        //add error message
                        ModelState.AddModelError("RefCoordinates", "RefCoordinates already exist.");
                        return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                    }

                    var site = new Site
                    {
                        SiteName = siteDTO.SiteName,
                        RefCoordinates = coords
                    };

                    _context.Sites.Add(site);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(
                        nameof(GetSiteById),
                        new { siteId = site.SiteId },
                        SiteToSiteDetailsDTO(site));
                }
                return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteDetailsDTO>>> GetAllSites()
        {
            return await _context.Sites
                .Select(x => SiteToSiteDetailsDTO(x))
                .ToListAsync();
        }

        [Route("{SiteId}")]
        [HttpGet]
        public async Task<ActionResult<SiteDetailsDTO>> GetSiteById(int SiteId)
        {
            var site = await _context.Sites.FindAsync(SiteId);

            if (site == null)
            {
                return NotFound();
            }

            return SiteToSiteDetailsDTO(site);
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

        [Route("{SiteId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSite(int SiteId, SiteUpdateDTO siteUpdateDTO)
        {
            try
            {
                //check site id matches id requested
                if (siteUpdateDTO.SiteId != null)
                {
                    if (SiteId != siteUpdateDTO.SiteId)
                    {
                        return BadRequest(new { ApiProblem = "Entity Id does not match requested Id." });
                    }
                }

                var siteIdExists = _context.Sites.Any(s => s.SiteId == siteUpdateDTO.SiteId);
                var siteNameExists = _context.Sites.Any(s => s.SiteName == siteUpdateDTO.SiteName);

                //check if siteid does not exist
                if (!siteIdExists && siteUpdateDTO.SiteId != null)
                {
                    //add error message
                    ModelState.AddModelError("SiteId", "No site found with given site id.");
                }

                //check if CartName already exists
                if (siteNameExists)
                {
                    //check if the cart is another cart
                    var theSite = _context.Sites.Where(s => s.SiteName == siteUpdateDTO.SiteName).FirstOrDefault();
                    if (theSite.SiteId != siteUpdateDTO.SiteId)
                    {
                        //add error message
                        ModelState.AddModelError("SiteName", "SiteName already exists.");
                    }
                }

                //try to parse given ref coordinates to npgsql box
                var coords = new NpgsqlBox();
                if (siteUpdateDTO.RefCoordinates != null)
                {
                    try
                    {
                        coords = NpgsqlBox.Parse(siteUpdateDTO.RefCoordinates);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("{0} Exception caught.", e);
                        //add error message
                        ModelState.AddModelError("RefCoordinates", "Invalid input: RefCoordinates must be specified using the following syntax \'((x1,y1),(x2,y2))\' where (x1,y1) and (x2,y2) are any two opposite corners.");
                        return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                    }
                }
                
                if (ModelState.IsValid)
                {
                    var refCoordinatesExist = _context.Sites.Any(s => s.RefCoordinates.Equals(coords));

                    //check if RefCoordinates already exists
                    if (refCoordinatesExist)
                    {
                        var theSite = _context.Sites.Where(s => s.RefCoordinates == coords).FirstOrDefault();
                        if (theSite.SiteId != siteUpdateDTO.SiteId)
                        {
                            //add error message
                            ModelState.AddModelError("RefCoordinates", "RefCoordinates already exist.");
                            return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                        }
                    }

                    var site = await _context.Sites.FindAsync(SiteId);
                    if (site == null)
                    {
                        return NotFound();
                    }

                    site.SiteName = siteUpdateDTO.SiteName;
                    site.RefCoordinates = coords;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) when (!SiteExists(SiteId))
                    {
                        return NotFound();
                    }

                    return NoContent();
                }
                return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        private bool SiteExists(int SiteId) =>
             _context.Sites.Any(s => s.SiteId == SiteId);

        private static SiteDetailsDTO SiteToSiteDetailsDTO(Site site) =>
            new SiteDetailsDTO
            {
                SiteId = site.SiteId,                
                SiteName = site.SiteName,
                RefCoordinates = site.RefCoordinates
            };
    }
}
