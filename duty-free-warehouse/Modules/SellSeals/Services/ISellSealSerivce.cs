using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Exchangerates.Entities;
using Project.Modules.Exchangerates.Services;
using Project.Modules.FileUploads.Service;
using Project.Modules.Products.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Response;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SellSeals
{
    public interface ISellSealSerivce
    {
        (object data, string message) UpdateSellData(List<SellData> sellDatas);
        (object data, string message) GetInvoicesWithSealNo(string sealNo);
        (object data, string message) RemoveInvoice(long invoiceId);
        (object data, string message) AddNewInvoice(string sealNo, Invoice invoice);
        (object data, string message) UpdateInvoice(long invoiceId, Invoice invoice);


    }

    public class SellSealSerivce : ISellSealSerivce
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public readonly IExchangerateService exchangerateService;

        public SellSealSerivce(IConfiguration _config, MariaDBContext _dBContext, IExchangerateService _exchangerateService)
        {
            config = _config;
            dBContext = _dBContext;
            exchangerateService = _exchangerateService;
        }

        private (List<SealDetail> sealDetails, InvoiceDetail invoiceDetail) CountQuantity(List<SealDetail> sealDetails, InvoiceDetail invoiceDetail)
        {
            foreach (var item in sealDetails)
            {
                if (invoiceDetail.SoldNumber == 0)
                {
                    break;
                }

                if (item.QuantitySell == item.QuantityExport)
                {
                    continue;
                }

                item.QuantitySell += invoiceDetail.SoldNumber;
                if (item.QuantitySell >= item.QuantityExport)
                {
                    invoiceDetail.SoldNumber = item.QuantitySell - item.QuantityExport;
                    item.QuantitySell = item.QuantityExport;
                    item.QuantityInventory = 0;
                    item.QuantityReal = item.QuantityInventory;
                }
                else
                {
                    invoiceDetail.SoldNumber = 0;
                    item.QuantityInventory -= item.QuantitySell;
                    item.QuantityReal = item.QuantityInventory;
                }
            }

            return (sealDetails, invoiceDetail);
        }

        public (object data, string message) AddNewInvoice(string sealNo, Invoice invoice)
        {
            List<Exchangerate> exchangerates = exchangerateService.GetAll();
            Seal seal = dBContext.Seals.Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, Products.Entities.Product>(x => x.Product)
                .Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, DeClaration>(x => x.DeClaration).FirstOrDefault(x => x.SealNumber.Equals(sealNo));
            if (seal == null)
                return (null, "Số seal không tồn tại.");

            if (seal.Status == (int)StatusSeals.SELL)
                return (null, $"Mã số seal đã được chốt với mã chuyến bay {seal.FlightNumber} và ngày bay {seal.FlightDate.ToString("dd/MM/yyyy")}");

            if (seal.Status == (int)StatusSeals.NEW)
                return (null, $"Mã số seal chưa được xuất kho.");

            Sell sell = new Sell
            {
                InvoiceNo = invoice.InvoiceNo,
                CustomerName = invoice.CustomerName,
                SeatNumber = invoice.SeatNumber,
                PassportNumber = invoice.PassportNumber,
                Nationality = invoice.Nationality,
                FlightDate = seal.FlightDate,
                FlightNo = seal.FlightNumber,
                FlightNumberDetail = invoice.FlightNumberDetail,
                TypeInvoice = invoice.TypeInvoice,
                
            };
            foreach (var item in invoice.SellDetail)
            {
                int slBan = item.SoldNumber;
                var detailsSeal = seal.SealDetails.Where(x => x.ProductCode.Equals(item.ProductCode)).OrderBy(x => x.DeClaration.DeClaDateRe).ToList();
                (List<SealDetail> sealDetails, InvoiceDetail invoiceDetail) = CountQuantity(detailsSeal, item);

                if (invoiceDetail.SoldNumber != 0)
                {
                    return (null, $"Số lượng bán của sản phẩm {invoiceDetail.ProductCode} lớn hơn số lượng xuất kho.");
                }
                dBContext.SellDetails.Add(new SellDetail()
                {
                    SellID = sell.SellID,
                    ProductCode = invoiceDetail.ProductCode,
                    SoldNumber = slBan,
                    Currency = invoiceDetail.Currency,
                    Price = invoiceDetail.Price
                });
            }
            dBContext.Sells.Add(sell);
            dBContext.SaveChanges();
            var result = new
            {
                invoiceId = sell.SellID,
                invoiceNo = sell.InvoiceNo,
                customerName = sell.CustomerName,
                passportNumber = sell.PassportNumber,
                nationality = sell.Nationality,
                seatNumber = sell.SeatNumber,
                totalSold = sell.SellDetails.Sum(v => v.SoldNumber),
                total = TotalExchangerate(exchangerates, sell.SellDetails),
                typeInvoice = sell.TypeInvoice,
                FlightNumberDetail = sell.FlightNumberDetail,
                sellDetail = sell.SellDetails.Select(v => new
                {
                    productCode = v.ProductCode,
                    productName = dBContext.Products.FirstOrDefault(x=>x.ProductCode.Equals(v.ProductCode)).ProductName,
                    soldNumber = v.SoldNumber,
                    price = v.Price.HasValue ? v.Price.Value.ToString() : String.Empty,
                    currency = String.IsNullOrEmpty(v.Currency) ? String.Empty : v.Currency
                }).ToList()
            };
            return (result, "Thêm hóa đơn thành công.");
        }

        public (object data, string message) GetInvoicesWithSealNo(string sealNo)
        {
            List<Exchangerate> exchangerates = exchangerateService.GetAll();
            Seal seal = dBContext.Seals.FirstOrDefault(x => x.SealNumber.Equals(sealNo));
            if (seal == null)
                return (null, "Số seal không tồn tại.");
            List<Sell> sells = dBContext.Sells
                .Where(x => seal.FlightNumber.Contains(x.FlightNo) && x.FlightDate.Date == seal.FlightDate)
                .Include(x => x.SellDetails)
                .ThenInclude<Sell, SellDetail, Product>(x => x.Product)
                .ToList();
            foreach (Sell sell in sells)
            {
                sell.ToTalSell = TotalExchangerate(exchangerates,sell.SellDetails);
            }
            var result = sells
                .Select(x => new InvoiceResponse(
                    x, 
                    dBContext.Citypairs.FirstOrDefault(y => y.Schedule.Contains(x.FlightNumberDetail)))
                ).OrderBy(x => x.InvoiceNo);
            
            return (new { sealInfo = seal, result }, "Thành công");
        }
        public List<ToTalSell> TotalExchangerate(List<Exchangerate> exchangerates, List<SellDetail> sellDetails)
        {
            List<ToTalSell> toTals = new List<ToTalSell>();
            foreach (Exchangerate exchangerate in exchangerates)
            {
                
                   ToTalSell toTal = new ToTalSell()
                    {
                        TotalExchangerate = TotalSellDetail(sellDetails,exchangerate),
                        exchangerateCode = exchangerate.ExchangerateCode
                    };
                
                toTals.Add(toTal);
            }
            return toTals;
        }
        public double TotalSellDetail(List<SellDetail> sellDetails, Exchangerate exchangerate)
        {
            foreach (SellDetail sellDetail in sellDetails)
            {
                if(sellDetail.Currency.Equals("USD"))
                {
                    sellDetail.TotalSellDetail = ((double)sellDetail.Price * (double)sellDetail.SoldNumber) * 23000;
                }
                else
                {
                    sellDetail.TotalSellDetail = (double)sellDetail.Price * (double)sellDetail.SoldNumber;
                }
            }
            return sellDetails.Sum(x=>x.TotalSellDetail);
        }

        private void UpdateQuantitySeal(List<SealDetail> detailsSeal, int soLuongBan, int tongBan)
        {
            if (soLuongBan == tongBan)
            {
                foreach (var item2 in detailsSeal)
                {
                    item2.QuantitySell = 0;
                    item2.QuantityInventory = item2.QuantityExport - item2.QuantitySell;
                    item2.QuantityReal = item2.QuantityInventory;
                }
            }
            else
            {
                foreach (var item2 in detailsSeal)
                {
                    if (tongBan == 0)
                        break;
                    if (item2.QuantitySell == 0)
                        continue;
                    item2.QuantitySell -= item2.QuantitySell > soLuongBan ? soLuongBan : item2.QuantitySell;
                    item2.QuantityInventory = item2.QuantityExport - item2.QuantitySell;
                    item2.QuantityReal = item2.QuantityInventory;
                    tongBan -= item2.QuantitySell;
                }
            }
            dBContext.SaveChanges();
        }

        public (object data, string message) RemoveInvoice(long invoiceId)
        {
            Sell sell = dBContext.Sells.Include(x => x.SellDetails).FirstOrDefault(x => x.SellID == invoiceId);
            if (sell == null)
                return (null, "ID Invoice không tồn tại.");
            Seal seal = dBContext.Seals
                .Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, Products.Entities.Product>(x => x.Product)
                .Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, DeClaration>(x => x.DeClaration)
                .FirstOrDefault(x => (x.FlightNumber == sell.FlightNo || x.FlightNumber.Contains(sell.FlightNo)) && x.FlightDate.Date == sell.FlightDate.Date);
            if (seal.Status == (int)StatusSeals.SELL)
                return (null, $"Mã số seal đã được chốt với mã chuyến bay {seal.FlightNumber} và ngày bay {seal.FlightDate.ToString("dd/MM/yyyy")}");
            if (seal.Status == (int)StatusSeals.NEW)
                return (null, $"Mã số seal chưa được xuất kho.");

            foreach (var item in sell.SellDetails)
            {
                var detailsSeal = seal.SealDetails.Where(x => x.ProductCode.Equals(item.ProductCode)).OrderByDescending(x => x.DeClaration.DeClaDateRe).ToList();
                int soLuongBan = item.SoldNumber;
                int tongBan = detailsSeal.Sum(v => v.QuantitySell);
                if (soLuongBan > tongBan)
                {
                    return (null, $"Số lượng bán của {item.ProductCode} bằng 0 trên seal {seal.SealNumber}.");
                }
                else
                {
                    UpdateQuantitySeal(detailsSeal, soLuongBan, tongBan);
                }
            }
            dBContext.SellDetails.RemoveRange(sell.SellDetails);
            dBContext.Sells.Remove(sell);
            dBContext.SaveChanges();
            return ("Thành công", "Xóa hóa đơn thành công");
        }

        public (object data, string message) UpdateInvoice(long invoiceId, Invoice invoice)
        {
            List<Exchangerate> exchangerates = exchangerateService.GetAll();
            Sell sell = dBContext.Sells.Include(x => x.SellDetails).FirstOrDefault(x => x.SellID == invoiceId);
            if (sell == null)
                return (null, "ID Invoice không tồn tại.");
            Seal seal = dBContext.Seals
                .Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, Products.Entities.Product>(x => x.Product)
                .Include(x => x.SealDetails).ThenInclude<Seal, SealDetail, DeClaration>(x => x.DeClaration)
                .FirstOrDefault(x => (x.FlightNumber == sell.FlightNo || x.FlightNumber.Contains(sell.FlightNo)) && x.FlightDate.Date == sell.FlightDate.Date);
            if (seal == null)
                return (null, "Seal không tồn tại");
            
            sell.TypeInvoice = invoice.TypeInvoice;
            sell.ToTalSell = TotalExchangerate(exchangerates, sell.SellDetails);
            sell.FlightNumberDetail = invoice.FlightNumberDetail;
            (object data, string message) = (null, null); // an sửa
            using (var transaction = dBContext.Database.BeginTransaction())
            {
                (data, message) = RemoveInvoice(sell.SellID);
                if (data == null)
                {
                    transaction.Rollback();
                    return (null, message);
                }
                (data, message) = AddNewInvoice(seal.SealNumber, invoice);
                if (data == null)
                {
                    transaction.Rollback();
                    return (null, message);
                }
                transaction.Commit();
            }
            return (data, "Cập nhật hóa đơn thành công");
        }

        private List<SealDetail> UpdateQuantitySell(List<SealDetail> detailsSeal, int sl)
        {
            foreach (var dt in detailsSeal)
            {
                if (sl <= 0)
                {
                    dt.QuantitySell += 0;
                }
                else
                {
                    dt.QuantitySell += (sl > dt.QuantityExport ? dt.QuantityExport : sl);
                }

                dt.QuantityInventory = dt.QuantityExport - dt.QuantitySell;
                dt.QuantityReal = dt.QuantityInventory;
                sl -= dt.QuantitySell;
            }
            return detailsSeal;
        }

        private (bool, string) UpdateSingleSellData(SellData item, List<SellData> sellDatas, Dictionary<string, object> valuePairs, List<Sell> sells, Seal seal)
        {
            if (seal.Status == (int)StatusSeals.SELL)
            {
                return (false, $"Mã số seal đã được chốt với mã chuyến bay {item.FlightNo} và ngày bay {item.FlightDate.ToString("dd/MM/yyyy")}");
            }

            if (seal.Status == (int)StatusSeals.NEW)
            {
                return (false, $"Mã số seal chưa được xuất kho.");
            }

            var detailsSeal = seal.SealDetails.Where(x => x.ProductCode.Equals(item.JDECode)).OrderBy(x => x.DeClaration.DeClaDateRe).ToList();

            if (detailsSeal.Count == 0)
            {
                return (false, $"Sản phẩm {item.JDECode} không tồn tại với mã chuyến bay {item.FlightNo} và ngày bay {item.FlightDate.ToString("dd/MM/yyyy")}");
            }

            var productInFlight = dBContext.SealDetails
                .Include(x => x.Seal)
                .Where(x => x.Seal.FlightNumber.Contains(item.FlightNo) && x.Seal.FlightDate == item.FlightDate && x.ProductCode.Equals(item.JDECode));

            int numberTotalExport = productInFlight.Sum(x => x.QuantityExport);

            int numberTotalSell = sellDatas.Where(x => x.FlightNo.Equals(item.FlightNo) && x.FlightDate == item.FlightDate && x.JDECode.Equals(item.JDECode))
                .Sum(x => x.Number);

            if (numberTotalSell > numberTotalExport)
            {
                return (false, $"Tổng số sản phẩm bán: {item.JDECode} ({numberTotalSell}), trên chuyến bay: {item.FlightNo} - {item.FlightDate.ToString("dd/MM/yyyy")} vượt quá tổng số lượng xuất ({numberTotalExport}).");
            }

            int sl = item.Number;

            detailsSeal = UpdateQuantitySell(detailsSeal, sl);
            if (!valuePairs.ContainsKey(seal.SealNumber))
            {
                var datas = sellDatas.Where(x => x.FlightDate == item.FlightDate && x.FlightNo == item.FlightNo).Select(v => v.JDECode.Trim()).ToList();
                var detailsSealRemain = seal.SealDetails.Where(x => !datas.Contains(x.ProductCode)).OrderBy(x => x.DeClaration.DeClaDateRe).ToList();
                foreach (var v3 in detailsSealRemain)
                {
                    v3.QuantitySell = 0;
                    v3.QuantityInventory = v3.QuantityExport;
                    v3.QuantityReal = v3.QuantityInventory;
                }
                valuePairs.Add(seal.SealNumber, "data");
            }

            Sell sell = sells
                .FirstOrDefault(x => x.FlightNo == item.FlightNo && x.FlightDate == item.FlightDate && x.InvoiceNo == item.Invoice);

            if (sell == null)
            {
                sell = new Sell()
                {
                    CustomerName = item.Customer,
                    FlightDate = item.FlightDate,
                    FlightNo = seal.FlightNumber,
                    InvoiceNo = item.Invoice,
                    SeatNumber = item.SeatNumber,
                    PassportNumber = item.Passport,
                    Nationality = item.Nationality,
                    FlightNumberDetail = item.FlightNo
                };
                sells.Add(sell);
                dBContext.Sells.Add(sell);
            }
            dBContext.SellDetails.Add(new SellDetail()
            {
                SellID = sell.SellID,
                ProductCode = item.JDECode,
                SoldNumber = item.Number,
                Currency = item.Currency,
                Price = item.Price
            });
            return (true, null);
        }
            

        public (object data, string message) UpdateSellData(List<SellData> sellDatas)
        {
            Dictionary<string, object> valuePairs = new Dictionary<string, object>();
            List<Sell> sells = dBContext.Sells.Where(x => sellDatas.Count(y => y.FlightDate == x.FlightDate && y.FlightNo == x.FlightNo && y.Invoice == x.InvoiceNo) != 0).ToList();
            List<SellDetail> sellDetails = dBContext.SellDetails.Where(x => sells.Select(y => y.SellID).Contains(x.SellID)).ToList();
            dBContext.SellDetails.RemoveRange(sellDetails);
            var sss = dBContext.SealDetails.Include(x => x.Seal).Where(x => sellDatas.Any(v => v.FlightDate == x.Seal.FlightDate && v.FlightNo == x.Seal.FlightNumber && v.JDECode == x.ProductCode)).ToList();
            sss.ForEach(x => x.QuantitySell = 0);

            foreach (var item in sellDatas)
            {
                if (item.Number == 0)
                {
                    continue;
                }

                Seal seal = dBContext.Seals
                    .Include(x => x.SealDetails)
                    .ThenInclude(x => x.Product)
                    .Include(x => x.SealDetails)
                    .ThenInclude(x => x.DeClaration)
                    .FirstOrDefault(x => (x.FlightNumber == item.FlightNo || x.FlightNumber.Contains(item.FlightNo)) && x.FlightDate.Date == item.FlightDate.Date);
                if (seal == null)
                {
                    return (false, $"Mã số seal không tồn với mã chuyến bay {item.FlightNo} và ngày bay {item.FlightDate.ToString("dd/MM/yyyy")}");
                }

                (bool check, string message) = UpdateSingleSellData(item, sellDatas, valuePairs, sells, seal);
                if (!check)
                {
                    return (null, message);
                }
            }
            dBContext.SaveChanges();
            return ("Thành công", "Thêm dữ liệu bán thành công");
        }
    }


    public class Invoice
    {
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string PassportNumber { get; set; }
        public string FlightNumberDetail { get; set; }
        public int? TypeInvoice { get; set; }
        public string Nationality { get; set; }
        public string SeatNumber { get; set; }
        public int TotalSold { get; set; }
        public string FlightNo { get; set; }
        public InvoiceDetail[] SellDetail { get; set; }
    }

    public class InvoiceDetail
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int SoldNumber { get; set; }
        public double? Price { get; set; }
        public string Currency { get; set; }
    }


}
