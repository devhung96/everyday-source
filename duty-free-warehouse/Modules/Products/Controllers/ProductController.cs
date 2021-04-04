using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Products.Entities;
using Project.Modules.Products.Requests;
using Project.Modules.Products.Services;

namespace Project.Modules.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        [HttpPost("showProduct")]
        public IActionResult ShowProduct([FromBody]ShowProductRequest request)
        {
            object result = productService.Showdata(request.ContentSearch);
            return ResponseOk(result);
        }
        [HttpPost("showMenu")]
        public IActionResult ShowMenu([FromBody]ShowProductRequest request, int MenuId)
        {
            if (MenuId == 0)
                MenuId = 1;
            List<MenuDetail> result = productService.ShowMenu(request.ContentSearch,MenuId);
            return ResponseOk(result);
        }
        [HttpPost("addProduct")]
        public IActionResult AddProduct([FromBody]AddProductRequest data)
        {
            var result = productService.Store(data);
            if (result is null)
                return ResponseBadRequest("Sản phẩm đã tồn tại");
            return ResponseOk(result,"Thêm sản phẩm vào danh sách thành công.");
        }
        [HttpPost("addMenu")]
        public IActionResult AddMenu([FromBody]AddMenuRequest data)
        {
            var result = productService.AddMenu(data);
            return ResponseOk(result, "Thêm sản phẩm vào menu thành công.");
        }
        [HttpPut("DeleteProduct")]
        public IActionResult DeleteProduct([FromBody] DeleteProductRequest request)
        {
            var result = productService.DeleteListProduct(request.ProductCode);
            int count = result.Count(m => m.message.Equals("Thành công."));
            if (count == 0)
            {
                return ResponseBadRequest("Xóa không thành công.");
            }

            return ResponseOk(result, "Xóa thành công sản phẩm ra khỏi danh sách.");
        }
        [HttpPut("DeleteMenu")]
        public IActionResult DeleteMenu([FromBody] DeleteMenusRequest request)
        {
            var result = productService.DeleteMenu(request.MenuIds);
            return ResponseOk(result, "Xóa thành công sản phẩm ra khỏi menu.");
        }
        [HttpPut("UpdateProduct/{productCode}")]
        public IActionResult UpdateProduct([FromBody] UpdateProductRequest data, string productCode)
        {
            var result = productService.EditProduct(productCode, data);
            if(result.Message is null)
            {
                return ResponseOk(result.data, "Chỉnh sửa thành công.");
            }
            return ResponseBadRequest(result.Message);
        }
        [HttpPut("UpdateMenuDetail/{menuDetailId}")]
        public IActionResult UpdateMenu([FromBody]UpdateMenuRequest updateMenuRequest, int menuDetailId)
        {
            var (menudetail, message) = productService.EditMenu(updateMenuRequest, menuDetailId);
            if (String.IsNullOrEmpty(message))
                return ResponseOk(menudetail);
            return ResponseBadRequest(message);
        }
        [HttpDelete("DeleteProduct/{productCode}")]
        public IActionResult DeleteItemProduct(string productCode)
        {
            (Product data, string Message) = productService.DeleteProduct(productCode);
            if (Message.Equals("Thành công."))
                return ResponseOk(data, "Xóa thành công 1 sản phẩm");
            return ResponseBadRequest(Message);
        }
        [HttpPost("import")]
        public IActionResult ImportProduct([FromForm] ImportProductRequest data)
        {
            var result = productService.ImportFileProduct(data);
            if (result.Result.data is null)
                return ResponseBadRequest(result.Result.message);
            return ResponseOk(result.Result.data, result.Result.message);

        }
        [HttpPost("importdata")]
        public IActionResult DataImport([FromBody] DataImportRequest request)
        {
            (List<AddProductRequest> data, string message) = productService.AddListProduct(request);
            return ResponseOk(data, message);
        }
    }
}