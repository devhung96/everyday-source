using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Project.App.Database;
using Project.App.Helpers;
using Project.Modules.Products.Entities;
using Project.Modules.Products.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Services
{
    public interface IProductService
    {
        Product Store(AddProductRequest request);
        (Product data, string Message) EditProduct(string productCode, UpdateProductRequest data);
        (Product data, string Message) DeleteProduct(string productCode);
        List<(Product data, string message)> DeleteListProduct(List<string> Id);
        object Showdata(string contentSearch);
        MenuDetail AddMenu(AddMenuRequest data);
        List<(MenuDetail, string)> DeleteMenu(List<int> MenuIds);
        List<MenuDetail> ShowMenu(string contentSearch,int? MenuId);
        (MenuDetail menudetail, string message) EditMenu(UpdateMenuRequest data, int MenuDatailId);
        Task<(object data, string message)> ImportFileProduct(ImportProductRequest request);
        ( List<AddProductRequest> data, string message) AddListProduct(DataImportRequest data);
    }
    public class ProductService : IProductService
    {
        private readonly MariaDBContext mariaDBContext;
        public ProductService( MariaDBContext mariaDB)
        {
            this.mariaDBContext = mariaDB;
        }

        public Product Store(AddProductRequest request)
        {
            Product productcheck = mariaDBContext.Products.Find(request.ProductCode);
            if (productcheck is null)
            {
                Product product = new Product(request.ProductCode, request.ProductName, request.ProductType, request.ProductUnit, request.ProductParLevel);
                mariaDBContext.Products.Add(product);
                mariaDBContext.SaveChanges();
                return product;
            }
            return null;

        }

        public List<(Product data, string message)> DeleteListProduct(List<string> Id)
        {
            List<(Product data, string message)> list = new List<(Product data, string message)>();
            foreach (string item in Id)
            {
                list.Add(DeleteProduct(item));
            }
            return list;
        }

        public object Showdata(string contentSearch)
        {
            List<Product> products = mariaDBContext.Products.Where(m => m.ProductStatus == 1 && (m.ProductCode.Contains(contentSearch) || m.ProductName.Contains(contentSearch))).OrderByDescending(m => m.ProductCreatedAt).ToList();
            foreach (var item in products)
            {
                item.IsMenu = 0;
                var menu = mariaDBContext.MenuDetails.Where(m => m.ProductCode.Equals(item.ProductCode)).FirstOrDefault();
                if (!(menu is null))
                    item.IsMenu = menu.MenuDetailId;


            }
            return products.Distinct();
        }

        public MenuDetail AddMenu(AddMenuRequest data)
        {
            Product product = mariaDBContext.Products.Where(m => m.ProductCode.Equals(data.ProductCode) && m.ProductStatus == 1).FirstOrDefault();
            int order = mariaDBContext.MenuDetails.Count()==0?1:mariaDBContext.MenuDetails.Max(m => m.MenuDetailOrder);
            MenuDetail menu = new MenuDetail { ProductCode = product.ProductCode, MenuId = data.MenuId, MenuDetailOrder = order + 1, MenuDetailParlever = product.ProductParLevel };
            mariaDBContext.MenuDetails.Add(menu);
            mariaDBContext.SaveChanges();
            return menu;

        }

        public List<(MenuDetail, string)> DeleteMenu(List<int> MenuIds)
        {
            List<(MenuDetail, string)> list = new List<(MenuDetail, string)>();
            foreach (int item in MenuIds)
            {
                MenuDetail menu = mariaDBContext.MenuDetails.Find(item);
                if (menu is null)
                {
                    list.Add((null, "Lỗi."));
                }
                else
                {
                    mariaDBContext.MenuDetails.Remove(menu);
                    mariaDBContext.SaveChanges();
                    list.Add((menu, null));
                }
            }
            return list;
        }

        public List<MenuDetail> ShowMenu(string contentSearch, int? MenuId)
        {
            List<MenuDetail> menus = mariaDBContext.MenuDetails.Where(m => m.MenuId == MenuId)
                                            .Where(m => m.ProductCode.ToLower().Contains(contentSearch.ToLower()) || m.Product.ProductName.ToLower().Contains(contentSearch.ToLower()))
                                            .Include(m=>m.Product).ToList();
            return menus;
        }
        public (MenuDetail menudetail, string message) EditMenu(UpdateMenuRequest data, int MenuDatailId)
        {
            MenuDetail menu_Detail = mariaDBContext.MenuDetails.Find(MenuDatailId);
            if (menu_Detail is null)
                return (menu_Detail, "MenuDetailId không tồn tại");
            menu_Detail.MenuDetailParlever = data.MenuDetailParlever.HasValue ? data.MenuDetailParlever.Value : menu_Detail.MenuDetailParlever;
            menu_Detail.MenuDetailOrder = data.MenuDetailOrder.HasValue ? data.MenuDetailOrder.Value : menu_Detail.MenuDetailOrder;
            mariaDBContext.MenuDetails.Update(menu_Detail);
            mariaDBContext.SaveChanges();
            return (menu_Detail, null);
        }


        public (Product data, string Message) EditProduct(string productCode, UpdateProductRequest data)
        {
            Product product = mariaDBContext.Products.FirstOrDefault(m => m.ProductCode.Equals(productCode));
            if (product is null)
                return (null, "ProductCode không tồn tại.");
            mariaDBContext.Entry(product).CurrentValues.SetValues(data.MergeData(product));
            mariaDBContext.SaveChanges();
            return (product, null);
        }

        public (Product data, string Message) DeleteProduct(string productCode)
        {
            Product product = mariaDBContext.Products.FirstOrDefault(m => m.ProductCode.Equals(productCode));
            if (product is null)
                return (null, "ProductCode không tồn tại.");
            product.ProductStatus = 0;
            mariaDBContext.SaveChanges();
            return (product, "Thành công.");
        }

        public async Task<(object data, string message)> ImportFileProduct(ImportProductRequest request)
        {
            try
            {
                string path = "Product";
                string fileFullName = await GeneralHelper.UploadFile(request.fileUpload, path);
                string file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path, fileFullName);
                FileInfo fileInfo = new FileInfo(file);
                List<AddProductRequest> ListProduct = new List<AddProductRequest>();

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    int sheet = 0;
                    int columns = 5;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheet];
                    if (worksheet.Dimension.Columns < columns)
                        return (null, "File không đúng định dạng.");
                    for (int row = 3; row <= worksheet.Dimension.Rows; row++)
                    {

                        if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text.Trim()))
                        {
                            if (row == 3)
                                return (null, "File không có dữ liệu.");
                            break;
                        }
                        AddProductRequest importRequest = new AddProductRequest();
                        importRequest.ProductCode = worksheet.Cells[row, 1].Text.Trim();
                        if (string.IsNullOrEmpty(importRequest.ProductCode))
                        {
                            return (null, "Dữ liệu không đúng.");
                        }
                        importRequest.ProductName = worksheet.Cells[row, 2].Text.Trim();
                        importRequest.ProductUnit = worksheet.Cells[row, 3].Text.Trim();
                        importRequest.ProductType = worksheet.Cells[row, 4].Text.Trim();
                        string parlever = worksheet.Cells[row, 5].Text.Trim();
                        importRequest.ProductParLevel = (String.IsNullOrEmpty(parlever)) ?  0 : int.Parse(parlever);

                        ListProduct.Add(importRequest);

                    }

                   ListProduct= XuLiDataRequest(ListProduct);
                }
                GeneralHelper.DeleteFile(file);
                return (ListProduct, "Import thành công.");
            }
            catch
            {
                return (null, "File không đúng định dạng.");
            }
        }

        List<AddProductRequest> XuLiDataRequest(List<AddProductRequest> ListProduct)
        {
            foreach (var item in ListProduct)
            {
                Product product = mariaDBContext.Products.Where(m => m.ProductCode.Equals(item.ProductCode)).FirstOrDefault();
                if (!(product is null))
                {

                    if (product.ProductStatus == 0)
                    {
                        item.Status = 4;
                    }
                    else { item.Status = 2; }// status =2 cũ
                }
                else
                {
                    item.Status = 1;// status =1 mới
                }
            }
            return ListProduct;
        }

        public (List<AddProductRequest> data, string message) AddListProduct(DataImportRequest data)
        {
            foreach (var item in data.addProducts)
            {
                Product product = new Product(item.ProductCode, item.ProductName, item.ProductType, item.ProductUnit, item.ProductParLevel);
                if (item.Status == 1)
                {
                    mariaDBContext.Add(product);
                    mariaDBContext.SaveChanges();
                }
                if (item.Status == 2 || (item.Status == 4))
                {
                    var kt = mariaDBContext.Products.Find(item.ProductCode);
                    if (kt is null)
                    {
                        item.Status = 3;
                    }
                    else
                    {
                        mariaDBContext.Entry(kt).CurrentValues.SetValues(product);
                        mariaDBContext.SaveChanges();
                    }

                }
                if (item.Status != 1 && item.Status != 2 && item.Status != 4)
                { item.Status = 3; }
                mariaDBContext.SaveChanges();

            }
            string message = "Thêm mới thành công: " + data.addProducts.Count(m => m.Status == 1) + ". Cập nhật thành công: " + data.addProducts.Count(m => m.Status == 2) + ". Lỗi: " + data.addProducts.Count(m => m.Status == 3) + ". Làm mới: " + data.addProducts.Count(m => m.Status == 4) + ".";
            return (data.addProducts, message);
        }

    }
}
