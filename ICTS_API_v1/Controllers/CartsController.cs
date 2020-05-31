using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICTS_API_v1.Controllers
{
    /// <summary>
    /// Controller class for carts.
    /// Controller and its action method handles incoming browser requests,
    /// retrieves necessary model data and returns appropriate responses.
    /// </summary>
    [Route("carts")]
    [ApiController]
    public class CartsController : Controller
    {
        //DBContext
        private readonly ICTS_Context _context; 

        /// <summary>
        /// Constructor for CartsController
        /// </summary>
        /// <param name="context">DB context</param>
        /// <returns>CartsController object</returns>
        public CartsController(ICTS_Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a cart and inserts into database
        /// </summary>
        /// <param name="cartDTO">Data transfer object for creating a cart</param>
        /// <returns>Action result containing data transfer object for cart details of created cart</returns>
        [HttpPost]
        public async Task<ActionResult<CartDetailsDTO>> AddCart(CartDTO cartDTO)
        {
            try
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
            catch (InvalidOperationException e)
            {
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Selects all carts in the database
        /// </summary>
        /// <returns>Action result containing list of data transfer objects for cart details of all carts</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetailsDTO>>> GetAllCarts()
        {
            return await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Select(x => CartToCartDetailsDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// Selects a cart from database matching given cart id
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <returns>Action result containing data transfer object for cart details of selected cart</returns>
        [Route("{CartId}")]
        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetCartByCartId(int CartId)
        {
            //find cart
            var cart = await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .FirstOrDefaultAsync(c => c.CartId == CartId);

            //if cart not found return error
            if (cart == null)
            {
                return NotFound();
            }

            return CartToCartDetailsDTO(cart);
        }

        /// <summary>
        /// Selects the cart from database containing a product with given lot id
        /// </summary>
        /// <param name="LotId">Lot id of the product</param>
        /// <returns>Action result containing data transfer object for cart details of selected cart</returns>
        [Route("lotid/{LotId}")]
        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetCartByProductLotId(string LotId)
        {
            //find cart
            var cart = await _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Site)
                .Where(c => c.Products.Any(p => p.LotId == LotId))
                .FirstOrDefaultAsync();

            //if cart not found return error
            if (cart == null)
            {
                return NotFound();
            }

            return CartToCartDetailsDTO(cart);
        }

        /// <summary>
        /// Selects all carts from database containing products with given product name
        /// </summary>
        /// <param name="ProductName">Product name of the product</param>
        /// <returns>Action result containing list of data transfer objects for cart details of selected carts</returns>
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

        /// <summary>
        /// Selects all carts from database containing products with given expiratio date
        /// </summary>
        /// <param name="ExpirationDate">Expiration date of the product</param>
        /// <returns>Action result containing list of data transfer objects for cart details of selected carts</returns>
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

        /// <summary>
        /// Updates the cart name and/or tag address of the cart matching given cart id 
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <param name="cartUpdateDTO">Data transfer object for updating a cart</param>
        /// <returns>IAction result with corresponding status code</returns>
        [HttpPut("{CartId}")]
        public async Task<IActionResult> UpdateCart(int CartId, CartUpdateDTO cartUpdateDTO)
        {
            try
            {
                //check if cart id matched id requested
                if (cartUpdateDTO.CartId != null)
                {
                    if (CartId != cartUpdateDTO.CartId)
                    {
                        return BadRequest(new { ApiProblem = "Entity Id does not match requested Id." });
                    }
                }

                var cartIdExists = _context.Carts.Any(c => c.CartId == cartUpdateDTO.CartId);
                var cartNameExists = _context.Carts.Any(c => c.CartName == cartUpdateDTO.CartName);
                var tagAddressExists = _context.Carts.Any(c => c.TagAddress == cartUpdateDTO.TagAddress);

                //check if cartid does not exist
                if (!cartIdExists && cartUpdateDTO.CartId != null)
                {
                    //add error message
                    ModelState.AddModelError("CartId", "No cart found with given cart id.");
                }

                //check if CartName already exists
                if (cartNameExists)
                {
                    //check if the cart is another cart
                    var theCart = _context.Carts.Where(c => c.CartName == cartUpdateDTO.CartName).FirstOrDefault();
                    if (theCart.CartId != cartUpdateDTO.CartId)
                    {
                        //add error message
                        ModelState.AddModelError("CartName", "CartName already exists.");
                    }
                }

                //check if TagAddress already exists
                if (tagAddressExists)
                {
                    //check if the cart is another cart
                    var theCart = _context.Carts.Where(c => c.TagAddress == cartUpdateDTO.TagAddress).FirstOrDefault();
                    if (theCart.CartId != cartUpdateDTO.CartId)
                    {
                        //add error message
                        ModelState.AddModelError("TagAddress", "TagAddress already exists.");
                    }
                }

                //if model is not valid return error messages 
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                }

                //find cart
                var cart = await _context.Carts.FindAsync(CartId);

                //if site not found return error 
                if (cart == null)
                {
                    return NotFound();
                }

                //update cart
                cart.CartName = cartUpdateDTO.CartName;
                cart.TagAddress = cartUpdateDTO.TagAddress;

                //put cart
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!CartExists(CartId))
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (NullReferenceException e)
            {
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Deletes the cart that matches given cart id 
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <returns>IAction result with corresponding status code</returns>
        [Route("{CartId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCart(int CartId)
        {
            //find cart
            var cart = await _context.Carts.FindAsync(CartId);

            //if cart not found return error
            if (cart == null)
            {
                return NotFound();
            }

            //delete cart
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Private method for determining if cart with given cart id exists 
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <returns>Boolean value of the expression</returns>
        private bool CartExists(int CartId) =>
             _context.Carts.Any(c => c.CartId == CartId);

        /// <summary>
        /// Private method for creating a CartDetailsDTO from a Cart
        /// </summary>
        /// <param name="cart">Cart to be used</param>
        /// <returns>Data transfer object for cart details</returns>
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
