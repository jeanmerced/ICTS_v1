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
    /// <summary>
    /// Controller class for sites.
    /// Controller and its action method handles incoming browser requests,
    /// retrieves necessary model data and returns appropriate responses.
    /// </summary>
    [Route("sites")]
    [ApiController]
    public class SitesController : Controller
    {
        //DBContext
        private readonly ICTS_Context _context;

        /// <summary>
        /// Constructor for SitesController
        /// </summary>
        /// <param name="context">DB context</param>
        /// <returns>SitesController object</returns>
        public SitesController(ICTS_Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a site and inserts into database
        /// </summary>
        /// <param name="siteDTO">Data transfer object for creating a site</param>
        /// <returns>Action result containing data transfer object for site details of created site</returns>
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
                        //if exception is caught write to console and return error message
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

                    //create site
                    var site = new Site
                    {
                        SiteName = siteDTO.SiteName,
                        RefCoordinates = coords
                    };

                    //insert site
                    _context.Sites.Add(site);
                    await _context.SaveChangesAsync();

                    //return the new site details
                    return CreatedAtAction(
                        nameof(GetSiteById),
                        new { siteId = site.SiteId },
                        SiteToSiteDetailsDTO(site));
                }
                return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
            }
            catch (InvalidOperationException e)
            {
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Selects all sites in the database
        /// </summary>
        /// <returns>Action result containing list of data transfer objects for site details of all sites</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteDetailsDTO>>> GetAllSites()
        {
            return await _context.Sites
                .Select(x => SiteToSiteDetailsDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// Selects a site from database matching given site id
        /// </summary>
        /// <param name="SiteId">site id of the site</param>
        /// <returns>Action result containing data transfer object for site details of selected site</returns>
        [Route("{SiteId}")]
        [HttpGet]
        public async Task<ActionResult<SiteDetailsDTO>> GetSiteById(int SiteId)
        {
            //find site
            var site = await _context.Sites.FindAsync(SiteId);

            //if site not found return error
            if (site == null)
            {
                return NotFound();
            }

            return SiteToSiteDetailsDTO(site);
        }

        /// <summary>
        /// Updates the site name and/or ref  coordinates of the site matching given site id 
        /// </summary>
        /// <param name="SiteId">Site id of the site</param>
        /// <param name="siteUpdateDTO">Data transfer object for updating a site</param>
        /// <returns>IAction result with corresponding status code</returns>
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

                //check if SiteName already exists
                if (siteNameExists)
                {
                    //check if the site is another site
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
                        //if exception is caught write to console and return error message
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
                        //check if the site is another site
                        var theSite = _context.Sites.Where(s => s.RefCoordinates == coords).FirstOrDefault();
                        if (theSite.SiteId != siteUpdateDTO.SiteId)
                        {
                            //add error message
                            ModelState.AddModelError("RefCoordinates", "RefCoordinates already exist.");
                            return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                        }
                    }

                    //find site
                    var site = await _context.Sites.FindAsync(SiteId);

                    //if site not found return error 
                    if (site == null)
                    {
                        return NotFound();
                    }

                    //update site
                    site.SiteName = siteUpdateDTO.SiteName;
                    site.RefCoordinates = coords;

                    //put site
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
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Deletes the site that matches given site id 
        /// </summary>
        /// <param name="SiteId">Site id of the cart</param>
        /// <returns>IAction result with corresponding status code</returns>
        [Route("{SiteId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveSite(int SiteId)
        {
            //find site
            var site = await _context.Sites.FindAsync(SiteId);

            //if site not found return error
            if (site == null)
            {
                return NotFound();
            }

            //delete site
            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Private method for determining if site with given site id exists 
        /// </summary>
        /// <param name="SiteId">Site id of the site</param>
        /// <returns>Boolean value of the expression</returns>
        private bool SiteExists(int SiteId) =>
             _context.Sites.Any(s => s.SiteId == SiteId);

        /// <summary>
        /// Private method for creating a SiteDetailsDTO from a Site
        /// </summary>
        /// <param name="site">Site to be used</param>
        /// <returns>Data transfer object for site details</returns>
        private static SiteDetailsDTO SiteToSiteDetailsDTO(Site site) =>
            new SiteDetailsDTO
            {
                SiteId = site.SiteId,                
                SiteName = site.SiteName,
                RefCoordinates = site.RefCoordinates
            };
    }
}
