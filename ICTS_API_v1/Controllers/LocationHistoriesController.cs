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
    /// Controller class for location histories.
    /// Controller and its action method handles incoming browser requests,
    /// retrieves necessary model data and returns appropriate responses.
    /// </summary>
    [Route("locationhistories")]
    [ApiController]
    public class LocationHistoriesController : Controller
    {
        //DBContext
        private readonly ICTS_Context _context;

        /// <summary>
        /// Constructor for LocationHistoriesController
        /// </summary>
        /// <param name="context">DB context</param>
        /// <returns>LocationHistoriesController object</returns>
        public LocationHistoriesController(ICTS_Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a location history and inserts into database
        /// </summary>
        /// <param name="locationHistoryDTO">Data transfer object for creating a location history</param>
        /// <returns>Action result containing data transfer object for location history details of created location history</returns>
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
                    //if exception is caught write to console and return error message
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
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Selects all location histories in the database
        /// </summary>
        /// <returns>Action result containing list of data transfer objects for location history details of all location histories</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationHistoryDetailsDTO>>> GetAllLocationHistories()
        {
            return await _context.LocationHistories
                .Include(lh => lh.Site)
                .Select(x => LocationHistoryToLocationHistoryDetailsDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// Selects a location history from database matching given record id
        /// </summary>
        /// <param name="RecordId">Record id of the location history</param>
        /// <returns>Action result containing data transfer object for location history details of selected location history</returns>
        [Route("{RecordId}")]
        [HttpGet]
        public async Task<ActionResult<LocationHistoryDetailsDTO>> GetLocationHistoryByRecordId(int RecordId)
        {
            //find location history
            var locationHistory = await _context.LocationHistories
                .Include(lh => lh.Site)
                .FirstOrDefaultAsync(lh => lh.RecordId == RecordId);

            //if location history not found return error 
            if (locationHistory == null)
            {
                return NotFound();
            }

            return LocationHistoryToLocationHistoryDetailsDTO(locationHistory);
        }

        /// <summary>
        /// Selects a location history from database matching given cart id
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <returns>Action result containing list of data transfer objects for location history details of selected location history</returns>
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

        /// <summary>
        /// Deletes the location history that matches given record id 
        /// </summary>
        /// <param name="RecordId">Record id of the location history</param>
        /// <returns>IAction result with corresponding status code</returns>
        [Route("{RecordId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveLocationFromHistory(int RecordId)
        {
            //find location history
            var locationHistory = await _context.LocationHistories.FindAsync(RecordId);

            //if location history nor found return error
            if (locationHistory == null)
            {
                return NotFound();
            }

            //delete location history
            _context.LocationHistories.Remove(locationHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Private method for creating a LocationHistoryDetailsDTO from a LocationHistory
        /// </summary>
        /// <param name="locationHistory">LocationHistory to be used</param>
        /// <returns>Data transfer object for location history details</returns>
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
