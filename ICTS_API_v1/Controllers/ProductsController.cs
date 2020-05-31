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
    /// <summary>
    /// Controller class for products.
    /// Controller and its action method handles incoming browser requests,
    /// retrieves necessary model data and returns appropriate responses.
    /// </summary>
    [Route("products")]
    [ApiController]
    public class ProductsController : Controller
    {
        //DBContext
        private readonly ICTS_Context _context;

        //Service
        private readonly MPROCProductsService _productsService;

        /// <summary>
        /// Constructor for ProductsController
        /// </summary>
        /// <param name="context">DB context</param>
        /// <returns>ProductsController object</returns>
        public ProductsController(ICTS_Context context, MPROCProductsService productsService)
        {
            _context = context;
            _productsService = productsService;
        }

        /// <summary>
        /// Creates a product and inserts into database
        /// </summary>
        /// <param name="productDTO">Data transfer object for creating a product</param>
        /// <returns>Action result containing data transfer object for product details of created product</returns>
        [HttpPost]
        public async Task<ActionResult<ProductDetailsDTO>> AddProductToCart(ProductDTO productDTO)
        {
            try
            {
                var cartIdExists = _context.Carts.Any(c => c.CartId == productDTO.CartId);
                var productInCart = _context.Products.Any(p => p.LotId == productDTO.LotId);

                //check if cartid exists
                if (!cartIdExists && productDTO.CartId != null)
                {
                    //add error message
                    ModelState.AddModelError("CartId", "No cart found with given cart id.");
                }

                //check if product is already associated with another cart
                if (productInCart)
                {
                    //add error message
                    ModelState.AddModelError("LotId", "Product already added to a cart.");
                }

                //if model is not valid return error messages
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                }

                //find product with given lotid from MPROC's data
                var mPROCProduct = GetMPROCProductByLotId(productDTO.LotId);

                //if product does not exist in MPROC's data return error message
                if (mPROCProduct == null)
                {
                    //add error message
                    ModelState.AddModelError("LotId", "No product found with given lot id.");
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));

                }

                //tries to parse expiration date to DateTime. if exception, expDate = null
                DateTime? expDate;
                try
                {
                    expDate = DateTime.Parse(mPROCProduct.USEBEFOREDATE);
                }
                catch (FormatException e)
                {
                    //if exception is caught write to console and expdate is null
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

                //return the created product details
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { productId = product.ProductId },
                    ProductsToProductDetailsDTO(product));
            }
            catch (InvalidOperationException e)
            {
                //if exception is caught write to console and return error message
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(new { ApiProblem = "Invalid JSON format sent." });
            }
        }

        /// <summary>
        /// Selects a product from database matching given product id
        /// </summary>
        /// <param name="ProductId">Product id of the product</param>
        /// <returns>Action result containing data transfer object for product details of selected product</returns>
        [Route("{ProductId}")]
        [HttpGet]
        public async Task<ActionResult<ProductDetailsDTO>> GetProductById(int ProductId)
        {
            //find product
            var product = await _context.Products
                .Include(p => p.Cart)
                    .ThenInclude(c => c.Site)
                .Where(p => p.ProductId == ProductId)
                .FirstOrDefaultAsync();

            //if product not found return error
            if (product == null)
            {
                return NotFound();
            }

            return ProductsToProductDetailsDTO(product);
        }

        /// <summary>
        /// Selects all product from database contained in cart matching given cart id
        /// </summary>
        /// <param name="CartId">Cart id of the cart</param>
        /// <returns>Action result containing list of data transfer objects for product details of selected products</returns>
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

        /// <summary>
        /// Updates the virtual site name of the product matching given product id 
        /// </summary>
        /// <param name="ProductId">Product id of the product</param>
        /// <param name="productUpdateDTO">Data transfer object for updating a product</param>
        /// <returns>IAction result with corresponding status code</returns>
        [Route("{ProductId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int ProductId, ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                //check product id matches requested id
                if (productUpdateDTO.ProductId != null)
                {
                    if (ProductId != productUpdateDTO.ProductId)
                    {
                        return BadRequest(new { ApiProblem = "Entity Id does not match requested Id." });
                    }
                }

                var productIdExists = _context.Products.Any(p => p.ProductId == productUpdateDTO.ProductId);

                //check if product id does not exist
                if (!productIdExists && productUpdateDTO.ProductId != null)
                {
                    //add error message
                    ModelState.AddModelError("ProductId", "No product found with given product id.");
                }

                //if model is not valid return error messages 
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
                }

                //find product
                var product = await _context.Products.FindAsync(ProductId);

                //if product not found return error
                if (product == null)
                {
                    return NotFound();
                }

                //update product
                product.VirtualSiteName = productUpdateDTO.VirtualSiteName;

                //put product
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!ProductExists(ProductId))
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
        /// Deletes the product that matches given product id 
        /// </summary>
        /// <param name="ProductId">Product id of the product</param>
        /// <returns>IAction result with corresponding status code</returns>
        [Route("{ProductId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveProductFromCart(int ProductId)
        {
            //find product
            var product = await _context.Products.FindAsync(ProductId);

            //if product not found return error
            if (product == null)
            {
                return NotFound();
            }

            //delete product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Private method for requesting MPROC product from MPROCProducts data given lot id
        /// </summary>
        /// <param name="LotId">Lot id of the product</param>
        /// <returns>MPROC product model containing necessary properties to create a product</returns>
        private MPROCProduct GetMPROCProductByLotId(string LotId)
        {
            //get MPROC products
            var products = _productsService.GetMPROCProducts();

            //find product matching given lot id
            foreach (var product in products)
            {
                if (product.LOTID == LotId)
                    return product;
            }
            return null;
        }

        /// <summary>
        /// Private method for determining if product with given product id exists 
        /// </summary>
        /// <param name="ProductId">Product id of the product</param>
        /// <returns>Boolean value of the expression</returns>
        private bool ProductExists(int ProductId) =>
             _context.Products.Any(p => p.ProductId == ProductId);


        /// <summary>
        /// Private method for creating a ProductDetailsDTO from a Product
        /// </summary>
        /// <param name="product">Product to be used</param>
        /// <returns>Data transfer object for product details</returns>
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
