//using ICTS_API.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ICTS_API.Controllers
//{
//    public class LocationHistoriesController : Controller
//    {
//        private readonly MyWebApiContext _context;

//        public LocationHistoriesController(MyWebApiContext context)
//        {
//            _context = context;
//        }

//        [Route("location-histories/cartid/{CartID}")]
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<LocationHistory>>> GetLocationHistoryByCartID(int CartID)
//        {
//            return await _context.LocationHistories.Where(lh => lh.CartID == CartID).ToListAsync();
//        }

//        [Route("location-histories")]
//        [HttpPost]
//        public async Task<ActionResult<LocationHistory>> AddLocationToHistory(LocationHistory locationHistory)
//        {
//            _context.LocationHistories.Add(locationHistory);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetLocationHistoryByCartID), new { cartID = locationHistory.CartID }, locationHistory);
//        }

//        [Route("location-histories/{RecordID}")]
//        [HttpDelete]
//        public async Task<IActionResult> RemoveLocationFromHistory(int RecordID)
//        {
//            var locationHistory = await _context.LocationHistories.FindAsync(RecordID);

//            if (locationHistory == null)
//            {
//                return NotFound();
//            }

//            _context.LocationHistories.Remove(locationHistory);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        //TODO:UpdateLocationHistories*********************************************************************************
//        //[Route("location-histories")]
//        //[HttpPut]
//        //public void UpdateLocationHistories()
//        //{

//        //}
//    }
//}
