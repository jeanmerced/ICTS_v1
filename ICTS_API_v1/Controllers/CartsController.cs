using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICTS_API_v1.Controllers
{
    [Route("carts")]
    [ApiController]
    public class CartsController : Controller
    {
        private readonly ICTS_Context _context;

        public CartsController(ICTS_Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CartDetailsDTO>> AddCart(CartDTO cartDTO)
        {
            var cartNameExists = _context.Carts.Any(c => c.CartName == cartDTO.CartName);
            var tagAddressExists = _context.Carts.Any(c => c.TagAddress == cartDTO.TagAddress);

            //check if CartName already exists
            if (cartNameExists)
            {
                //add error message
                ModelState.AddModelError("CartName", "CartName already exists.");
            }

            //check if TagAddress already exists
            if (tagAddressExists)
            {
                //add error message
                ModelState.AddModelError("TagAddress", "TagAddress already exists.");
            }

            //if model is not valid return error messages 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
            }

            //create cart
            var cart = new Cart
            {
                CartName = cartDTO.CartName,
                TagAddress = cartDTO.TagAddress
            };

            //insert cart
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            //return the new cart details
            return CreatedAtAction(
                nameof(GetCartByCartId),
                new { cartId = cart.CartId },
                CartToCartDetailsDTO(cart));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetailsDTO>>> GetAllCarts()
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Select(x => CartToCartDetailsDTO(x))
                .ToListAsync();
        }

        [Route("{CartId}")]
        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetCartByCartId(int CartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .FirstOrDefaultAsync(c => c.CartId == CartId);

            if (cart == null)
            {
                return NotFound();
            }

            return CartToCartDetailsDTO(cart);
        }

        [Route("lotid/{LotId}")]
        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetCartByProductLotId(string LotId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Where(c => c.Products.Any(p => p.LotId == LotId))
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound();
            }

            return CartToCartDetailsDTO(cart);
        }

        [Route("productname/{ProductName}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetailsDTO>>> GetCartsByProductName(string ProductName)
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Where(c => c.Products.Any(p => p.ProductName == ProductName))
                .Select(x => CartToCartDetailsDTO(x))
                .ToListAsync();
        }

        [Route("expirationdate/{ExpirationDate}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetailsDTO>>> GetCartsByProductExpirationDate(DateTime ExpirationDate)
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Where(c => c.Products.Any(p => p.ExpirationDate == ExpirationDate))
                .Select(x => CartToCartDetailsDTO(x))
                .ToListAsync();
        }

        //TODO: Update CartName and TagAddress
        //[HttpPut("{CartId}")]
        //public async Task<IActionResult> UpdateCart(int CartId, CartLocationDTO cartLocationDTO)
        //{
        //    if (CartId != cartLocationDTO.CartId)
        //    {
        //        return BadRequest();
        //    }

        //    var cart = await _context.Carts.FindAsync(CartId);
        //    if (cart == null)
        //    {
        //        return NotFound();
        //    }

        //    cart.LastUpdated = DateTime.Now;
        //    cart.SiteId = cartLocationDTO.SiteId;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException) when (!CartExists(CartId))
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}

        [Route("{CartId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCart(int CartId)
        {
            var cart = await _context.Carts.FindAsync(CartId);

            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int CartId) =>
             _context.Carts.Any(c => c.CartId == CartId);

        private static CartDetailsDTO CartToCartDetailsDTO(Cart cart) =>
            new CartDetailsDTO
            {
                CartId = cart.CartId,
                CartName = cart.CartName,
                TagAddress = cart.TagAddress,
                LastUpdated = cart.LastUpdated,
                Site = cart.Site,
                Products = cart.Products
            };
    }
}
