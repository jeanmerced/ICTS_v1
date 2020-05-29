using ICTS_API_v1.Models;
using ICTS_API_v1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICTS_API_v1.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ICTS_Context _context;
        private readonly MPROCProductsService _productsService;

        public ProductsController(ICTS_Context context, MPROCProductsService productsService)
        {
            _context = context;
            _productsService = productsService;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailsDTO>> AddProductToCart(ProductDTO productDTO)
        {
            var cartIdExists = _context.Carts.Any(c => c.CartId == productDTO.CartId);
            var productInCart = _context.Products.Any(p => p.LotId == productDTO.LotId);

            //check if cartid exists
            if (!cartIdExists && productDTO.CartId != null)
            {
                //add error message
                ModelState.AddModelError("CartId", "Cart with CartId=" + productDTO.CartId + " does not exist.");
            }

            //check if product is already associated with another cart
            if (productInCart)
            {
                //add error message
                ModelState.AddModelError("LotId", "Product with LotId=" + productDTO.LotId + " is already associated with another cart.");
            }

            //if model is not valid return error messages
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()) });
            }

            //looks for product with given lotid from MPROC's data
            var mPROCProduct = GetMPROCProductByLotId(productDTO.LotId);

            //if product does not exist in MPROC's data return error message
            if (mPROCProduct == null)
            {
                //add error message
                ModelState.AddModelError("LotId", "Product with LotId=" + productDTO.LotId + " does not exist.");
                return BadRequest(new { errors = ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()) });

            }

            //tries to parse expiration date to DateTime. if exception, expDate = null
            DateTime? expDate;
            try
            {
                expDate = DateTime.Parse(mPROCProduct.USEBEFOREDATE);
            }
            catch (FormatException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                expDate = null;
            }

            //create product
            var product = new Product
            {
                LotId = mPROCProduct.LOTID,
                ProductName = mPROCProduct.PRODUCTNAME,
                ExpirationDate = expDate,
                Quantity = mPROCProduct.COMPONENTQTY,
                VirtualSiteName = mPROCProduct.STEPNAME,
                CartId = productDTO.CartId
            };

            //insert product
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //return the new product details
            return CreatedAtAction(
                nameof(GetProductById),
                new { productId = product.ProductId },
                ProductsToProductDetailsDTO(product));
        }

        [Route("cartid/{CartId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailsDTO>>> GetProductsByCartId(int CartId)
        {
            return await _context.Products
                .Include(p => p.Cart)
                    .ThenInclude(c => c.Site)
                .Where(p => p.CartId == CartId)
                .Select(x => ProductsToProductDetailsDTO(x))
                .ToListAsync();
        }

        [Route("{ProductId}")]
        [HttpGet]
        public async Task<ActionResult<ProductDetailsDTO>> GetProductById(int ProductId)
        {
            var product = await _context.Products
                .Include(p => p.Cart)
                    .ThenInclude(c => c.Site)
                .Where(p => p.ProductId == ProductId)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return ProductsToProductDetailsDTO(product);
        }

        
        [Route("{ProductId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveProductFromCart(int ProductId)
        {
            var product = await _context.Products.FindAsync(ProductId);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // TODO:UpdateProduct
        //[Route("products")]
        //[HttpPut]
        //public async Task<ActionResult<Product>> UpdateProduct(int CartID, [FromBody]string TagAddress)
        //{
        //    return null;
        //}

        private MPROCProduct GetMPROCProductByLotId(string LotId)
        {
            var products = _productsService.GetMPROCProducts();
            foreach (var product in products)
            {
                if (product.LOTID == LotId)
                    return product;
            }
            return null;
        }

        private static ProductDetailsDTO ProductsToProductDetailsDTO(Product product) =>
            new ProductDetailsDTO
            {
                ProductId = product.ProductId,
                LotId = product.LotId,
                ProductName = product.ProductName,
                ExpirationDate = product.ExpirationDate,
                Quantity = product.Quantity,
                VirtualSiteName = product.VirtualSiteName,
                CartId = product.CartId,
                Cart = product.Cart
            };
    }
}