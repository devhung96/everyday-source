using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Inventories.Entites;
using Project.Modules.Products.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Services
{
    public interface ISealImportService
    {
        (List<SealImportRequest>, string) Import(List<SealImportRequest> requests);
        (int, string) ChangeQuantityExport(int quantity, string sealNumber, string productCode);
        object DetailSeal(string sealNumber);
        (object, string) CheckXuatKho(string sealNumber);
    }
    public class SealImportService : ISealImportService
    {
        public readonly MariaDBContext dBContext;
        public SealImportService(MariaDBContext DbContext)
        {
            dBContext = DbContext;
        }

        private (SealProduct, string) CheckTonKho (SealImportRequest request, MenuDetail menuDetail, List<Inventory> TonKho)
        {
            List<DeClarationDetail> deClarationDetails = dBContext.DeClarationDetails
                        .Include(x => x.DeClaration)
                        .Where(x => x.DeClaration.DeClaType == 1 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && x.ProductCode == menuDetail.ProductCode && x.DeClaration.DeClaDateReData <= request.FlightDate)
                        .OrderBy(x => x.DeClaration.DeClaDateReData)
                        .ToList();

            if (deClarationDetails.Count == 0)
            {
                return (null, $"Sản phẩm {menuDetail.Product?.ProductName} không tồn tại trong tờ khai");
            }

            int soluongPar = menuDetail.MenuDetailParlever;
            int tongTonKho = 0;
            foreach (DeClarationDetail item2 in deClarationDetails)
            {
                if (soluongPar == 0)
                {
                    break;
                }
                Inventory inventory = TonKho.FirstOrDefault(x => x.ProductCode == item2.ProductCode && x.DeNumber == item2.DeClaNumber);
                tongTonKho += inventory.InQuantity;
            }

            SealProduct sealProduct = new SealProduct
            {
                ProductCode = menuDetail.ProductCode,
                SealNumber = request.SealNumber,
                QuantityExport = tongTonKho < soluongPar ? tongTonKho : soluongPar,
                CreatedAt = DateTime.Now
            };

            return (sealProduct, "Success");
        }

        public (List<SealImportRequest>, string) Import(List<SealImportRequest> requests)
        {
            List<MenuDetail> menus = dBContext.MenuDetails.Include(x => x.Product).ToList();
            List<Inventory> TonKho = dBContext.Inventories.Where(x=>menus.Any(y=>y.ProductCode == x.ProductCode)).AsNoTracking().ToList();
            List<Citypair> cityPairs = dBContext.Citypairs.ToList();

            foreach (SealImportRequest item in requests)
            {
                Seal sealCheck = dBContext.Seals.FirstOrDefault(x => x.SealNumber.Equals(item.SealNumber));
                Seal sealCheck1 = dBContext.Seals.FirstOrDefault(x => x.FlightNumber.Equals(item.FlightNumber) && x.FlightDate.Equals(item.FlightDate));
                if (sealCheck != null)
                {
                    return (null, $"Số Seal {item.SealNumber} đã tồn tại trong hệ thống");
                }
                if (sealCheck1 != null)
                {
                    return (null, $"Mã chuyến bay {item.FlightNumber} đã tồn tại trong hệ thống ngày {item.FlightDate.ToString("dd/MM/yyyy")}");
                }

                // Check FlightNumber
                string[] flightArr = item.FlightNumber.Replace(" ", "").Split(",");
                Citypair citypair = null;
                foreach (var flight in flightArr)
                {
                    citypair = cityPairs.FirstOrDefault(x => x.Schedule.Contains(flight));
                    if (citypair is null)
                    {
                        return (null, $"Mã chuyến bay {flight} không tồn tại trong hệ thống");
                    }    
                }
                // End Check FlightNumber
                Seal seal = new Seal
                {
                    SealNumber = item.SealNumber,
                    FlightNumber = item.FlightNumber,
                    FlightDate = item.FlightDate,
                    AcReg = item.AcReg,
                    CityPairId = citypair.Id,
                    Route = citypair.Route,
                    Return = item.SealNumberReturn,
                };
                dBContext.Seals.Add(seal);
            }
            
            List<SealProduct> sealProducts = new List<SealProduct>();
            foreach (SealImportRequest item in requests)
            {
                foreach (MenuDetail itemMenu in menus)
                {
                    (SealProduct sealProduct, string message) = CheckTonKho(item, itemMenu, TonKho);
                    if(sealProduct is null)
                    {
                        return (null, message);
                    }

                    sealProducts.Add(sealProduct);
                }
            }
            dBContext.SealProducts.AddRange(sealProducts);
            dBContext.SaveChanges();
            return (requests, "Nhập thành công");
        }

        private (bool, string, string deClaNumber) CheckHetTonKho(SealProduct seaProduct, Seal seal, List<Inventory> TonKho)
        {
            List<DeClarationDetail> deClarationDetails = dBContext.DeClarationDetails
                    .Include(x => x.DeClaration)
                    .Where(x => x.DeClaration.DeClaType == 1 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && x.ProductCode == seaProduct.ProductCode)
                    .OrderBy(x => x.DeClaration.DeClaDateReData)
                    .ToList();

            if (deClarationDetails.Count == 0)
            {
                return (false, $"Sản phẩm {seaProduct.product?.ProductName} không tồn tại trong tờ khai", null);
            }

            int soluongPar = seaProduct.QuantityExport; // đổi thành số lượng xuất trong Seal Product

            bool flagHetTonKho = false;

            deClarationDetails = deClarationDetails.Where(x => x.DeClaration.DeClaDateReData <= seal.FlightDate).ToList();

            foreach (DeClarationDetail item2 in deClarationDetails)
            {
                if (soluongPar == 0)
                {
                    break;
                }
                Inventory inventory = TonKho.FirstOrDefault(x => x.ProductCode == item2.ProductCode && x.DeNumber == item2.DeClaNumber);
                int tonKho = inventory.InQuantity;

                if (tonKho == 0)
                {
                    continue;
                }
                flagHetTonKho = true;

                if (tonKho >= soluongPar)
                {
                    SealDetail sealDetail = new SealDetail
                    {
                        SealNumber = seaProduct.SealNumber,
                        ProductCode = seaProduct.ProductCode,
                        DeClaNumber = item2.DeClaNumber,
                        QuantityInventory = soluongPar, // mặc định tồn kho bằng số lượng xuất
                        QuantityReal = soluongPar, // như trên
                        QuantitySell = 0,
                        QuantityExport = soluongPar
                    };
                    dBContext.SealDetails.Add(sealDetail);
                    inventory.InQuantity -= sealDetail.QuantityExport;
                    break;
                }
                else
                {
                    SealDetail sealDetail = new SealDetail
                    {
                        SealNumber = seaProduct.SealNumber,
                        ProductCode = seaProduct.ProductCode,
                        DeClaNumber = item2.DeClaNumber,
                        QuantityInventory = tonKho,
                        QuantityReal = tonKho,
                        QuantitySell = 0,
                        QuantityExport = tonKho
                    };
                    dBContext.SealDetails.Add(sealDetail);
                    soluongPar -= sealDetail.QuantityExport;
                    inventory.InQuantity = 0;
                }
            }
            return (flagHetTonKho, null, deClarationDetails.FirstOrDefault()?.DeClaNumber);
        }

        public (object, string) CheckXuatKho(string sealNumber)
        {
            List<SealProduct> sealProducts = dBContext.SealProducts.Include(x => x.product).Where(x => x.SealNumber.Equals(sealNumber)).ToList();
            List<Inventory> TonKho = dBContext.Inventories.Where(x => sealProducts.Any(y => y.ProductCode == x.ProductCode)).AsNoTracking().ToList();
            Seal seal = dBContext.Seals.FirstOrDefault(x => x.SealNumber.Equals(sealNumber));
            foreach (SealProduct item in sealProducts)
            {
                (bool flagHetTonKho, string message, string deClaNumber) = CheckHetTonKho(item, seal, TonKho);
                if(message != null)
                {
                    return (null, message);
                }
                    
                if (!flagHetTonKho)
                {
                    SealDetail sealDetail = new SealDetail
                    {
                        SealNumber = item.SealNumber,
                        ProductCode = item.ProductCode,
                        DeClaNumber = deClaNumber,
                        QuantityInventory = 0,
                        QuantityReal = 0,
                        QuantitySell = 0,
                        QuantityExport = 0
                    };
                    dBContext.SealDetails.Add(sealDetail);
                }
            }
            dBContext.SaveChanges();
            return (new { }, "Nhập thành công");
        }

        public (int, string) ChangeQuantityExport(int quantity, string sealNumber, string productCode)
        {
            List<Inventory> TonKho = dBContext.Inventories.Where(x => productCode == x.ProductCode).AsNoTracking().ToList();
            int tongTonKho = 0;
            Seal seal = dBContext.Seals.FirstOrDefault(x => x.SealNumber.Equals(sealNumber));
            if(seal is null)
            {
                return (0, "Số Seal không hợp lệ");
            }
                
            List<DeClarationDetail> deClarationDetails = dBContext.DeClarationDetails
                        .Include(x => x.DeClaration)
                        .Where(x => x.DeClaration.DeClaType == 1 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && x.ProductCode.Equals(productCode))
                        .OrderBy(x => x.DeClaration.DeClaDateReData)
                        .ToList();

            if (deClarationDetails.Count == 0)
            {
                return (0, $"Sản phẩm không tồn tại trong tờ khai");
            }

            deClarationDetails = deClarationDetails.Where(x => x.DeClaration.DeClaDateReData <= seal.FlightDate).ToList(); // check ngày tờ khai < ngày bay
                
            foreach (DeClarationDetail item2 in deClarationDetails)
            {
                Inventory inventory = TonKho.FirstOrDefault(x => x.ProductCode == item2.ProductCode && x.DeNumber == item2.DeClaNumber);
                tongTonKho += inventory.InQuantity;
            }
            if(tongTonKho < quantity)
            {
                return (0, $"Hạn Mức Tối Đa Là {tongTonKho} Của Sản Phẩm Này");
            }
            SealProduct sealProduct = dBContext.SealProducts
                .FirstOrDefault(x => x.SealNumber.Equals(sealNumber) && x.ProductCode.Equals(productCode));
            if(sealProduct is null)
            {
                return (0, "Seal Product Not Found");
            }
            sealProduct.QuantityExport = quantity;
            dBContext.SealProducts.Update(sealProduct);
            dBContext.SaveChanges();
            return (1, "Update Quantity Success");
        }
        public object DetailSeal(string sealNumber)
        {
            Seal seal = dBContext.Seals
                .Include(x => x.SealProduct)
                .ThenInclude(x => x.product)
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => x.SealNumber.Equals(sealNumber));

            return new SealDetailResponse(seal, dBContext.Citypairs.ToList());
        }
    }
}
