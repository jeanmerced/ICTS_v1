using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationHistory>>> GetAllLocationHistories()
        {
            return await _context.LocationHistories.ToListAsync();
        }

        [Route("{RecordId}")]
        [HttpGet]
        public async Task<ActionResult<LocationHistory>> GetLocationHistoryByRecordId(int RecordId)
        {
            var locationHistory = await _context.LocationHistories.FirstOrDefaultAsync(lh => lh.RecordId == RecordId);

            if (locationHistory == null)
            {
                return NotFound();
            }

            return locationHistory;
        }

        [Route("cartid/{CartId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationHistory>>> GetLocationHistoriesByCartId(int CartId)
        {
            return await _context.LocationHistories.Where(lh => lh.CartId == CartId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<LocationHistory>> AddLocationToHistory(LocationHistoryDTO locationHistoryDTO)
        {
            var locationHistory = new LocationHistory
            {
                CartId = locationHistoryDTO.CartId,
                SiteId = locationHistoryDTO.SiteId,
                RecordDate = DateTime.Now
            };

            _context.LocationHistories.Add(locationHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLocationHistoryByRecordId),
                new { recordId = locationHistory.RecordId },
                locationHistory);
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

        //TODO:UpdateLocationHistories*********************************************************************************
        //[Route("location-histories")]
        //[HttpPut]
        //public void UpdateLocationHistories()
        //{

        //}
    }
}
