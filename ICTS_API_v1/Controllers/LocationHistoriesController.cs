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
    [Route("locationhistories")]
    [ApiController]
    public class LocationHistoriesController : Controller
    {
        private readonly ICTS_Context _context;

        public LocationHistoriesController(ICTS_Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LocationHistoryDetailsDTO>> AddLocationToHistory(LocationHistoryDTO locationHistoryDTO)
        {
            try
            {
                var cartIdExists = _context.Carts.Any(c => c.CartId == locationHistoryDTO.CartId);
                var siteIdExists = _context.Sites.Any(s => s.SiteId == locationHistoryDTO.SiteId);

                //check if cartid exists
                if (!cartIdExists && locationHistoryDTO.CartId != null)
                {
                    //add error message
                    ModelState.AddModelError("CartId", "No cart found with given cart id.");
                }

                //check if siteid exists
                if (!siteIdExists && locationHistoryDTO.SiteId != null)
                {
                    //add error message
                    ModelState.AddModelError("SiteId", "No site found with given site id.");
                }

                //if model is not valid return error messages
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                }

                //tries to parse cart coordinates to NpgsqlPoint. if exception, return bad request
                var coords = new NpgsqlPoint(); ;
                try
                {
                    coords = NpgsqlPoint.Parse(locationHistoryDTO.CartCoordinates);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                    //add error message
                    ModelState.AddModelError("CartCoordinates", "Invalid input: CartCoordinates must be specified using the following syntax \'(x,y)\' where x and y are the respective coordinates, as floating-point numbers.");
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                }

                //create location history
                var locationHistory = new LocationHistory
                {
                    CartId = locationHistoryDTO.CartId,
                    SiteId = locationHistoryDTO.SiteId,
                    CartCoordinates = coords,
                    RecordDate = DateTime.Now
                };

                //insert location history
                _context.LocationHistories.Add(locationHistory);
                await _context.SaveChangesAsync();

                //rerturn the new location history details
                return CreatedAtAction(
                    nameof(GetLocationHistoryByRecordId),
                    new { recordId = locationHistory.RecordId },
                    LocationHistoryToLocationHistoryDetailsDTO(locationHistory));
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationHistoryDetailsDTO>>> GetAllLocationHistories()
        {
            return await _context.LocationHistories
                .Include(lh => lh.Site)
                .Select(x => LocationHistoryToLocationHistoryDetailsDTO(x))
                .ToListAsync();
        }

        [Route("{RecordId}")]
        [HttpGet]
        public async Task<ActionResult<LocationHistoryDetailsDTO>> GetLocationHistoryByRecordId(int RecordId)
        {
            var locationHistory = await _context.LocationHistories
                .Include(lh => lh.Site)
                .FirstOrDefaultAsync(lh => lh.RecordId == RecordId);

            if (locationHistory == null)
            {
                return NotFound();
            }

            return LocationHistoryToLocationHistoryDetailsDTO(locationHistory);
        }

        [Route("cartid/{CartId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationHistoryDetailsDTO>>> GetLocationHistoriesByCartId(int CartId)
        {
            return await _context.LocationHistories
                .Include(lh => lh.Site)
                .Where(lh => lh.CartId == CartId)
                .Select(x => LocationHistoryToLocationHistoryDetailsDTO(x))
                .ToListAsync();
        }

        [Route("{RecordId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveLocationFromHistory(int RecordId)
        {
            var locationHistory = await _context.LocationHistories.FindAsync(RecordId);

            if (locationHistory == null)
            {
                return NotFound();
            }

            _context.LocationHistories.Remove(locationHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static LocationHistoryDetailsDTO LocationHistoryToLocationHistoryDetailsDTO(LocationHistory locationHistory) =>
            new LocationHistoryDetailsDTO
            {
                RecordId = locationHistory.RecordId,
                CartId = locationHistory.CartId,
                CartCoordinates = locationHistory.CartCoordinates,
                RecordDate = locationHistory.RecordDate,
                Site = locationHistory.Site
            };
    }
}