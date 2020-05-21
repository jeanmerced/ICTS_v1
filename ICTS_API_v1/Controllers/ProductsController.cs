using ICTS_API_v1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ProductsController(ICTS_Context context)
        {
            _context = context;
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

        [HttpPost]
        public async Task<ActionResult<ProductDetailsDTO>> AddProductToCart(ProductDTO productDTO)
        {
            var product = new Product
            {
                LotId = productDTO.LotId,
                ProductName = productDTO.ProductName,
                ExpirationDate = productDTO.ExpirationDate,
                Quantity = productDTO.Quantity,
                VirtualSiteName = productDTO.VirtualSiteName,
                CartId = productDTO.CartId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductById),
                new { productId = product.ProductId },
                ProductsToProductDetailsDTO(product));
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
