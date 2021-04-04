using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Declarations.Requests;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Inventories.Entites;
using Project.Modules.Inventories.Services;
using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Services
{
    public interface IDeClarationDetailServices
    {
        (object data, string message) StoreImport(StoreDetail request);
        (List<DeClarationDetail> data, string message) StoreExport(StoreExportDetail request);

        (object data, string message) StoreAndSaveInventoryImport(StoreDetail request);
        (object data, string message) StoreAndSaveInventoryExport(StoreExportDetail request);
        (object data, string message) ShowProductFollowImport(string declarationNumberImport);
        object ShowProductInInventory(string DeclarationNumber, string ProductCode);
    }
    public class DeClarationDetailServices : IDeClarationDetailServices
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IInventoryService inventoryService;
        public DeClarationDetailServices(MariaDBContext mariaDBContext, IInventoryService inventoryService)
        {
            this.mariaDBContext = mariaDBContext;
            this.inventoryService = inventoryService;
        }
        public (object data, string message) StoreImport(StoreDetail request)
        {
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumber));
            if (deClaration is null)
                return (null, "Số tờ khai không đúng.");
            if (deClaration.DeClaStatus == DeClaStatus.confirm)
                return (null, "Tờ khai đã được xác nhận.");
            request.DeClaNumber = deClaration.DeClaNumber;
            List<DeClarationDetail> imports = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(request.DeClaNumber)).ToList();
            mariaDBContext.DeClarationDetails.RemoveRange(imports);
            List<object> errors = new List<object>();
            foreach (var item in request.Details)
            {
                if (item.DeClaDetailQuantity < 0)
                    errors.Add(item);
                else
                {
                    DeClarationDetail clarationDetail = new DeClarationDetail()
                    {
                        DeClaNumber = request.DeClaNumber,
                        ProductCode = item.ProductCode,
                        DeClaDetailQuantity = item.DeClaDetailQuantity,
                        DeClaDetailInvoicePrice = item.DeClaDetailInvoicePrice,
                        DeClaDetailInvoiceValue = item.DeClaDetailInvoiceValue.HasValue ? item.DeClaDetailInvoiceValue.Value : 0,
                        DeClaDetailProductNumber = item.DeClaDetailProductNumber,
                        DeClaDetailOwnCode = item.DeClaDetailOwnCode,
                        DeClaReconfirmCode = item.DeClaReconfirmCode
                    }; 
                    mariaDBContext.DeClarationDetails.Add(clarationDetail);
                }
            }
            if (errors.Count > 0)
                return (errors, "Số lượng không được phép âm.");
            mariaDBContext.SaveChanges();
            return ("Thành công", null);
        }

        public (object data, string message) StoreAndSaveInventoryImport(StoreDetail request)
        {
            DeClaration checkdeClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumber));
            if(checkdeClaration is null)
                return (null, "Số tờ khai không đúng.");
            if (checkdeClaration.DeClaStatus == DeClaStatus.confirm)
                return (null, "Tờ khai đã được xác nhận.");
            List<DeClarationDetail> imports = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(request.DeClaNumber)).ToList();
            mariaDBContext.DeClarationDetails.RemoveRange(imports);

            JObject jObject = JObject.Parse(checkdeClaration.DeClaContent);
            jObject["dateadded"] = request.DateAdded;
            jObject["importnumber"] = request.ImportNumber;
            jObject["supplier"] = request.Supplier;
            jObject["deliver"] = request.Deliver;
            checkdeClaration.DeClaContent = JsonConvert.SerializeObject(jObject);
            foreach (var item in request.Details)
            {
                DeClarationDetail clarationDetail = new DeClarationDetail()
                {
                    DeClaNumber = request.DeClaNumber,
                    ProductCode = item.ProductCode,
                    DeClaDetailQuantity = item.DeClaDetailQuantity,
                    DeClaDetailInvoicePrice = item.DeClaDetailInvoicePrice,
                    DeClaDetailInvoiceValue = item.DeClaDetailInvoiceValue.HasValue ? item.DeClaDetailInvoiceValue.Value : 0,
                    DeClaDetailProductNumber = item.DeClaDetailProductNumber,
                    DeClaDetailOwnCode = item.DeClaDetailOwnCode,
                    DeClaReconfirmCode = item.DeClaReconfirmCode
                };
                mariaDBContext.DeClarationDetails.Add(clarationDetail);
                
            }
            mariaDBContext.SaveChanges();
            List<DeClarationDetail> declarations = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Where(x => x.DeClaration.DeClaStatus == DeClaStatus.unconfimred && x.DeClaNumber == request.DeClaNumber && x.DeClaration.DeClaType == 1).ToList();
            var result = inventoryService.AddList(declarations);
            if (result.result.Count == 0)
                return (null, "Nhập kho lỗi.");
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumber));
            deClaration.DeClaStatus = DeClaStatus.confirm;
            mariaDBContext.SaveChanges();
            return ("Thành công", null);
        }

        public (List<DeClarationDetail> data, string message) StoreExport(StoreExportDetail request)
        {
            List<DeClarationDetail> errors = new List<DeClarationDetail>();
            List<DeClarationDetail> exports = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(request.DeClaNumberExport)).ToList();
            mariaDBContext.DeClarationDetails.RemoveRange(exports);
            List<DeClarationDetail> productImports = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(request.DeClaNumberImport)).AsNoTracking().ToList();
            foreach (var item in request.Details)
            {
                List<DeClarationDetail> deClarationDetails = productImports.Where(x => x.ProductCode == item.ProductCode).ToList();
                int tong = deClarationDetails.Sum(x => x.DeClaDetailQuantity);
                if(item.DeClaDetailQuantity > tong || item.DeClaDetailQuantity < 0)
                {
                    return (new List<DeClarationDetail>(), $"Số lượng của sản phẩm {item.ProductCode} không hợp lệ.");
                }
                if (item.DeClaDetailQuantity == tong)
                {
                    foreach (var item2 in deClarationDetails)
                    {
                        DeClarationDetail clarationDetail = new DeClarationDetail()
                        {
                            DeClaNumber = request.DeClaNumberExport,
                            ProductCode = item2.ProductCode,
                            DeClaDetailQuantity = item.DeClaDetailQuantity,
                            DeClaDetailInvoicePrice = item2.DeClaDetailInvoicePrice,
                            DeClaDetailInvoiceValue = item2.DeClaDetailInvoiceValue,
                            DeClaDetailProductNumber = item2.DeClaDetailProductNumber,
                            DeClaReconfirmCode = item2.DeClaReconfirmCode,
                            DeClaDetailOwnCode = item2.DeClaDetailOwnCode
                        };
                        mariaDBContext.DeClarationDetails.Add(clarationDetail);
                        item.DeClaDetailQuantity -= item2.DeClaDetailQuantity;
                    }
                    continue;
                }

                if (item.DeClaDetailQuantity < tong)
                {
                    foreach (var item2 in deClarationDetails)
                    {
                        if (item.DeClaDetailQuantity > 0)
                        {
                            if(item2.DeClaDetailQuantity >= item.DeClaDetailQuantity)
                            {
                                DeClarationDetail clarationDetail = new DeClarationDetail()
                                {
                                    DeClaNumber = request.DeClaNumberExport,
                                    ProductCode = item2.ProductCode,
                                    DeClaDetailQuantity = item.DeClaDetailQuantity,
                                    DeClaDetailInvoicePrice = item2.DeClaDetailInvoicePrice,
                                    DeClaDetailInvoiceValue = item2.DeClaDetailInvoiceValue,
                                    DeClaDetailProductNumber = item2.DeClaDetailProductNumber,
                                    DeClaReconfirmCode = item2.DeClaReconfirmCode,
                                    DeClaDetailOwnCode = item2.DeClaDetailOwnCode
                                };
                                mariaDBContext.DeClarationDetails.Add(clarationDetail);
                                item2.DeClaDetailQuantity -= item.DeClaDetailQuantity;
                                item.DeClaDetailQuantity = 0;
                                break;
                            }
                            else
                            {
                                DeClarationDetail clarationDetail = new DeClarationDetail()
                                {
                                    DeClaNumber = request.DeClaNumberExport,
                                    ProductCode = item2.ProductCode,
                                    DeClaDetailQuantity = item.DeClaDetailQuantity - item2.DeClaDetailQuantity,
                                    DeClaDetailInvoicePrice = item2.DeClaDetailInvoicePrice,
                                    DeClaDetailInvoiceValue = item2.DeClaDetailInvoiceValue,
                                    DeClaDetailProductNumber = item2.DeClaDetailProductNumber,
                                    DeClaReconfirmCode = item2.DeClaReconfirmCode,
                                    DeClaDetailOwnCode = item2.DeClaDetailOwnCode
                                };
                                mariaDBContext.DeClarationDetails.Add(clarationDetail);
                                item.DeClaDetailQuantity -= item2.DeClaDetailQuantity;
                            }
                            
                        }
                    }
                    continue;
                }
            }
            mariaDBContext.SaveChanges();
            return (errors, null);
        }

        public (object data, string message) StoreAndSaveInventoryExport(StoreExportDetail request)
        {
            DeClaration checkdeClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumberExport));
            if (checkdeClaration.DeClaStatus == DeClaStatus.confirm)
                return (null, "Tờ khai đã được xác nhận.");
            List<DeClarationDetail> exports = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(request.DeClaNumberExport)).ToList();
            mariaDBContext.DeClarationDetails.RemoveRange(exports);

            JObject jObject = JObject.Parse(checkdeClaration.DeClaContent);
            jObject["dateexported"] = request.DateExported;
            jObject["exportnumber"] = request.ExportNumber;
            jObject["requestname"] = request.RequestName;
            jObject["rebill"] = request.ReBill;
            checkdeClaration.DeClaContent = JsonConvert.SerializeObject(jObject);
            foreach (var item in request.Details)
            {
                DeClarationDetail clarationDetail = new DeClarationDetail()
                {
                    DeClaNumber = request.DeClaNumberExport,
                    ProductCode = item.ProductCode,
                    DeClaDetailQuantity = item.DeClaDetailQuantity,
                    DeClaDetailInvoicePrice = item.DeClaDetailInvoicePrice,
                    DeClaDetailInvoiceValue = item.DeClaDetailInvoiceValue.HasValue ? item.DeClaDetailInvoiceValue.Value : 0,
                    DeClaDetailProductNumber = item.DeClaDetailProductNumber,
                    DeClaReconfirmCode = item.DeClaReconfirmCode,
                    DeClaDetailOwnCode = item.DeClaDetailOwnCode
                };
                mariaDBContext.DeClarationDetails.Add(clarationDetail);
            }
            mariaDBContext.SaveChanges();
            List<DeClarationDetail> declarations = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Where(x => x.DeClaration.DeClaStatus == DeClaStatus.unconfimred && x.DeClaNumber == request.DeClaNumberExport && x.DeClaration.DeClaType == 2).ToList();
            var result = inventoryService.RemmoveList(declarations, checkdeClaration.DeClaParentNumber);
            if (result.errors.Count > 0)
                return (null, $"Số lượng của sản phẩm {result.errors.FirstOrDefault().ProductCode} không hợp lệ.");
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumberExport));
            deClaration.DeClaStatus = DeClaStatus.confirm;
            mariaDBContext.SaveChanges();
            return ("Thành công", null);
        }

        public (object data, string message) ShowProductFollowImport(string declarationNumberImport)
        {
            List<DeClarationDetail> deClarationDetails = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(declarationNumberImport)).ToList();
            if (deClarationDetails.Count <= 0)
                return (null, "Số tờ khai nhập không đúng.");
            else
            {
                var products = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(declarationNumberImport)).Include(x => x.Product).GroupBy(x=> x.ProductCode).ToList();
                List<object> list = new List<object>();
                foreach (var item in products)
                {
                    list.Add(item.FirstOrDefault());
                }
                return (list, null);
            }
        }

        public object ShowProductInInventory(string DeclarationNumber, string ProductCode)
        {
            var result = inventoryService.GetInventory(DeclarationNumber, ProductCode);
            SupportResponseShowProductInInventory resultt = new SupportResponseShowProductInInventory
            {
                inId = result.inventorie?.InId,
                deNumber = result.inventorie?.DeNumber,
                inQuantity = result.inventorie?.InQuantity,
                productCode = result.inventorie?.ProductCode,
                deClaDetailProductNumber = mariaDBContext.DeClarationDetails.FirstOrDefault(x => x.DeClaNumber.Equals(result.inventorie.DeNumber) && x.ProductCode.Equals(result.inventorie.ProductCode))?.DeClaDetailProductNumber,
            };
            return resultt;
        }
        public class SupportResponseShowProductInInventory
        {
            public int? inId { get; set; }
            public string deNumber { get; set; }
            public int? inQuantity { get; set; }
            public string productCode { get; set; }
            public string settlementDate { get; set; }
            public Product product { get; set; }
            public string deClaDetailProductNumber { get; set; }
        }
    }

}
