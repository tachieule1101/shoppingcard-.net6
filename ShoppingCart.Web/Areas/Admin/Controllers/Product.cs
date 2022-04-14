using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCart.DataAccess.DT0;
using ShoppingCart.DataAccess.Repositories;

namespace Shoppingcart.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _hostingEnviroment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnviroment = hostingEnvironment;
        }

        #region APICALL
        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = products });
        }
        #endregion
        public IActionResult Index()
        {
            //ProductVM productVM = new ProductVM();
            //productVM.Products = _unitOfWork.Product.GetAll();
            return View();
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (vm.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(vm);
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM vm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = String.Empty;
                if (file != null)
                {
                    string uploadDir = Path.Combine(_hostingEnviroment.WebRootPath, "ProductImage");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                   
                    if (vm.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(_hostingEnviroment.WebRootPath,
                            vm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }
                if (vm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                    TempData["success"] = "Product Created Done!";
                }
                else
                {
                    _unitOfWork.Product.Update(vm.Product);
                    TempData["success"] = "Product Update Done!";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new {success = false,message="Error in Fetching Date"});
            }
            else
            {
                var oldImagePath = Path.Combine(_hostingEnviroment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                TempData["success"] = "Category Deleted Done!";
                return Json(new {success = true, message = "Product delete"});
            }    
        }
    }
}