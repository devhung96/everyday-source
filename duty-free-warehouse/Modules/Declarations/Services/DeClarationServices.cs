using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Project.App.Database;
using Project.Modules.Declarations.Entites;
using Project.Modules.Declarations.Models;
using Project.Modules.Declarations.Requests;
using Project.Modules.Declarations.Validatations;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Inventories.Entites;
using Project.Modules.Products.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Sells.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Project.Modules.Declarations.Services
{
    public interface IDeClarationServices
    {
        (object data, string message) Store(DeclarationStore request, int userID);
        object ShowAll();
        (object data, string message) ShowID(string declarationNumber);
        object Filter(SortDeclaration request);
        object Report(Report request);
        object ListReportImport(Report request);
        object TrackLiquidity(TrackLiquidityRequest rq);
        (bool flag, string message) ConfirmSettlement(string DeclarationNumber, string DispatchNumber);
        (bool flag, string message) DispatchExtend(string DeclarationNumber, string DispatchNumber, DateTime RenewalDate, DateTime? dispatchDate);
        object WarehouseReceipt(WarehouseReceiptRequest request);
        object ExportFilter(FilterExportDeclaration filterExport, int userId);
        object DeliveryReceipt(WarehouseReceiptRequest request);
        object ExportDeclarationTypeX4(FilterExportDeclaration filterExport, int TypeDeclaration = 1);
        object ExportDeclarationTypeX2(FilterExportDeclaration filterExport, int TypeDeclaration = 2);
        object ExtensionDispatchReport(WarehouseReceiptRequest request);
        object ExportDeclarationTypeX5(FilterExportDeclaration filterExport);
    }
    public class DeClarationServices : IDeClarationServices
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IConfiguration configuration;
        public DeClarationServices(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.mariaDBContext = mariaDBContext;
        }

        public (object data, string message) Store(DeclarationStore request, int userID)
        {
            if (mariaDBContext.Users.Find(userID) is null)
            {
                return (null, "UserNotFound");
            }
            if (request.Type == 2 && (request.Content["temporarynumber"] == null || String.IsNullOrEmpty(request.Content["temporarynumber"].ToString())))
            {
                return (null, "Số tờ khai tạm nhập không tồn tại.");
            }
            DeClaration deClaration = new DeClaration()
            {
                DeClaNumber = request.DeClaNumber,
                DeClaDateReData = request.DeClaDateReData,
                DeClaType = request.Type,
                DeClaContent = request.Content != null ? JsonConvert.SerializeObject(request.Content) : null,
                DeClaStatus = DeClaStatus.unconfimred,
                DeClaParentNumber = request.DeClaParentNumber,
                UserID = userID
            };
            mariaDBContext.Declarations.Add(deClaration);
            mariaDBContext.SaveChanges();
            return (deClaration, null);
        }
        public object ShowAll()
        {
            List<DeClaration> result = mariaDBContext.Declarations.Include(x => x.DeClarationDetails).ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product).ToList();
            return result;
        }

        public (object data, string message) ShowID(string declarationNumber)
        {
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(declarationNumber));
            if (deClaration is null)
                return (null, "Tờ khai không tồn tại.");
            DeClaration result = mariaDBContext.Declarations.Where(x => x.DeClaNumber.Equals(declarationNumber)).Include(x => x.DeClarationDetails).ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product).FirstOrDefault();
            return (result, null);
        }
        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }

        public object Filter(SortDeclaration request)
        {
            List<DeClaration> declarations = mariaDBContext.Declarations.Include(x => x.DeClarationDetails).ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product).Where(x =>
            (request.DeclarationNumber == null || String.IsNullOrEmpty(request.DeclarationNumber) || (request.DeclarationNumber != null && x.DeClaNumber.Equals(request.DeclarationNumber)))
            && (x.DeClaDateReData >= request.RegisterFromData && x.DeClaDateReData <= request.RegisterToData)
            && (!request.DeadlineFromData.HasValue || !String.IsNullOrEmpty(x.Content.SelectToken("deadline").ToString()) && (DateTime.ParseExact(x.Content.SelectToken("deadline").ToString(), "dd/MM/yyyy", null) >= request.DeadlineFromData.Value) && (DateTime.ParseExact(x.Content.SelectToken("deadline").ToString(), "dd/MM/yyyy", null) <= request.DeadlineToData.Value))
            && (!request.Type.HasValue || (x.DeClaType == request.Type.Value))
            ).OrderBy(OrderValue(request.sortField, request.sortOrder)).ToList();
            ResponseTable responseTable = new ResponseTable()
            {
                results = declarations.Skip((request.page - 1) * request.results).Take(request.results).ToList(),
                info = new Info()
                {
                    page = request.page,
                    totalRecord = declarations.Count,
                    results = request.results
                }
            };
            return responseTable;
        }

        //[Backend] [Task] N2 Xuất dữ liệu phiếu kê khai hàng hóa nhập kho doanh nghiệp từ nguồn hàng tạm nhập
        public object ExportFilter(FilterExportDeclaration filterExport, int userId)
        {
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (filterExport.DateFrom != null && filterExport.DateTo != null)
            {
                dateFrom = DateTime.ParseExact(filterExport.DateFrom, "dd/MM/yyyy", null);
                dateTo = DateTime.ParseExact(filterExport.DateTo, "dd/MM/yyyy", null);
            }
            List<DeClaration> declarations = mariaDBContext.Declarations
                .Include(x => x.DeClarationDetails)
                .ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product)
                .Where(x =>
                        ((dateFrom == null && dateTo == null) || (x.DeClaDateReData >= dateFrom && x.DeClaDateReData <= dateTo))
                        && (filterExport.DeclarationNumber == null || String.IsNullOrEmpty(filterExport.DeclarationNumber) || (filterExport.DeclarationNumber != null && x.DeClaNumber.Equals(filterExport.DeclarationNumber)))
                        && x.DeClaStatus == DeClaStatus.confirm
                        && x.DeClaType == 1
                        )
                .ToList();
            List<User> users = mariaDBContext.Users.ToList();
            List<ExportReportN2> exportReportN2s = new List<ExportReportN2>();
            foreach (DeClaration deClaration in declarations)
            {
                User user = users.FirstOrDefault(x =>
                                        x.UserId == deClaration.UserID
                                        && x.UserId == userId
                                        );
                if (user is null)
                {
                    continue;
                }
                exportReportN2s.AddRange(deClaration.HandleList(user));
            }
            return exportReportN2s;
        }

        //[Backend] Xuất báo cáo chuẩn X2: Phiếu khai hàng hóa xuất từ kho doanh nghiệp sang loại hình tái xuất.t = 2
        public object ExportDeclarationTypeX2(FilterExportDeclaration filterExport, int TypeDeclaration = 2)
        {
            DateTime? AccountingFrom = null;
            DateTime? AccountingTo = null;
            if (filterExport.DateFrom != null && filterExport.DateTo != null)
            {
                AccountingFrom = DateTime.ParseExact(filterExport.DateFrom, "dd/MM/yyyy", null);
                AccountingTo = DateTime.ParseExact(filterExport.DateTo, "dd/MM/yyyy", null);
            }
            string InvoiceNumberHash = "X2" + DateTime.UtcNow.ToString("yyyyMMdd"); //( loại chuẩn + ngày tháng năm + 00001)
            List<DeClaration> declarations = mariaDBContext.Declarations
                .Include(x => x.DeClarationDetails)
                .ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product)
                .Where(x =>
                        ((AccountingFrom == null && AccountingTo == null) || (x.DeClaDateReData >= AccountingFrom && x.DeClaDateReData <= AccountingTo))
                        && (filterExport.DeclarationNumber == null || String.IsNullOrEmpty(filterExport.DeclarationNumber) || (filterExport.DeclarationNumber != null && x.DeClaNumber.Equals(filterExport.DeclarationNumber)))
                        && x.DeClaType == TypeDeclaration
                        )
                .ToList();
            List<ExportReportX2> exportReportX2s = new List<ExportReportX2>();
            foreach (DeClaration deClaration in declarations)
            {
                foreach (DeClarationDetail deClarationDetail in deClaration.DeClarationDetails)
                {
                    ExportReportX2 reportX2 = new ExportReportX2
                    {
                        TaxCode = configuration["Export:TaxCode"],
                        InvoiceNumber = InvoiceNumberHash + DateTimeOffset.UtcNow.Ticks,
                        //AccountingDate = deClaration.DeClaDateReData,
                        //19.10 Ngày hạch toán => ngày lập phiếu
                        AccountingDate = DateTime.Now,
                        DeliveryCode = configuration["Export:OutCode"],
                        Currency = "USD",
                        Explain = null,
                        DeClaDetailProductNumber = deClarationDetail.DeClaDetailProductNumber,
                        ProductCode = deClarationDetail.Product.ProductCode,
                        ProductName = deClarationDetail.Product.ProductName,
                        ProductUnit = deClarationDetail.Product.ProductUnit,
                        DeClaNumber = deClarationDetail.DeClaNumber,
                        //18.11 NGAY_TK đang để theo ngày hiện tại => để theo ngày tờ khai xuất
                        //DeClaDateRe = DateTime.Now,
                        DeClaDateRe = deClaration.DeClaDateReData,
                        DeClaDetailQuantity = deClarationDetail.DeClaDetailQuantity,
                        DeClaDetailInvoicePrice = deClarationDetail.DeClaDetailInvoicePrice,
                        DeClaDetailInvoiceValue = deClarationDetail.DeClaDetailInvoiceValue
                    };
                    exportReportX2s.Add(reportX2);
                }
            }
            return exportReportX2s;
        }

        //[Backend] Xuất báo cáo chuẩn X4: Phiếu khai hàng hóa xuất từ kho doanh nghiệp sang loại hình tái nhap.t = 1
        public object ExportDeclarationTypeX4(FilterExportDeclaration filterExport, int TypeDeclaration = 1)
        {
            DateTime? AccountingFrom = null;
            DateTime? AccountingTo = null;
            if (filterExport.DateFrom != null && filterExport.DateTo != null)
            {
                AccountingFrom = DateTime.ParseExact(filterExport.DateFrom, "dd/MM/yyyy", null);
                AccountingTo = DateTime.ParseExact(filterExport.DateTo, "dd/MM/yyyy", null);
            }
            string InvoiceNumberHash = "X4" + DateTime.UtcNow.ToString("yyyyMMdd"); //( loại chuẩn + ngày tháng năm + 00001)
            List<DeClaration> declarations = mariaDBContext.Declarations
                .Include(x => x.DeClarationDetails)
                .ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product)
                .Where(x =>
                        ((AccountingFrom == null && AccountingTo == null) || (x.DeClaDateReData >= AccountingFrom && x.DeClaDateReData <= AccountingTo))
                        && (filterExport.DeclarationNumber == null || String.IsNullOrEmpty(filterExport.DeclarationNumber) || (filterExport.DeclarationNumber != null && x.DeClaNumber.Equals(filterExport.DeclarationNumber)))
                        && x.DeClaType == TypeDeclaration
                        )
                .ToList();
            List<ExportReportX4> exportReportX4s = new List<ExportReportX4>();
            foreach (DeClaration deClaration in declarations)
            {
                foreach (DeClarationDetail deClarationDetail in deClaration.DeClarationDetails)
                {
                    ExportReportX4 reportX4 = new ExportReportX4
                    {
                        TaxCode = configuration["Export:TaxCode"],
                        InvoiceNumber = InvoiceNumberHash + DateTimeOffset.UtcNow.Ticks,
                        AccountingDate = deClaration.DeClaDateReData,
                        DeliveryCode = configuration["Export:OutCode"],
                        Currency = "USD",
                        Explain = null,
                        DeClaDetailProductNumber = deClarationDetail.DeClaDetailProductNumber,
                        ProductCode = deClarationDetail.Product.ProductCode,
                        ProductName = deClarationDetail.Product.ProductName,
                        ProductUnit = deClarationDetail.Product.ProductUnit,
                        DeClaNumber = deClarationDetail.DeClaNumber,
                        DeClaDateRe = DateTime.Now,
                        DeClaDetailQuantity = deClarationDetail.DeClaDetailQuantity,
                        DeClaDetailInvoicePrice = deClarationDetail.DeClaDetailInvoicePrice,
                        DeClaDetailInvoiceValue = deClarationDetail.DeClaDetailInvoiceValue
                    };
                    exportReportX4s.Add(reportX4);
                }
            }
            return exportReportX4s;
        }

        //[Backend] X5
        public object ExportDeclarationTypeX5(FilterExportDeclaration filterExport)
        {
            DateTime AccountingFrom = DateTime.ParseExact(filterExport.DateFrom, "dd/MM/yyyy", null);
            DateTime AccountingTo = DateTime.ParseExact(filterExport.DateTo, "dd/MM/yyyy", null);
            string InvoiceNumberHash = "X5" + DateTime.UtcNow.ToString("yyyyMMdd"); //( loại chuẩn + ngày tháng năm + 00001)
            List<DeClaration> declarations = mariaDBContext.Declarations
                .Include(x => x.DeClarationDetails)
                .ThenInclude<DeClaration, DeClarationDetail, Product>(x => x.Product)
                .Where(x =>
                        x.DeClaDateReData >= AccountingFrom && x.DeClaDateReData <= AccountingTo
                        )
                .ToList();

            List<Sell> sells = mariaDBContext
                .Sells
                .Include(x => x.SellDetails)
                .ToList();

            List<Citypair> citypairs = mariaDBContext
                .Citypairs
                .ToList();
            List<ExportReportX5> exportReportX5s = new List<ExportReportX5>();
            foreach (DeClaration deClaration in declarations)
            {
                foreach (DeClarationDetail deClarationDetail in deClaration.DeClarationDetails)
                {
                    List<Sell> sellsTemp = sells.Where(x => x.SellDetails.Any(z => z.ProductCode.Equals(deClarationDetail.ProductCode))).ToList();
                    foreach (Sell sell in sellsTemp)
                    {
                        Citypair citypairsTemp = citypairs.FirstOrDefault(x => x.CityPairSchedules.Any(z => z.flightNumber.Equals(sell.FlightNumberDetail)));
                        if (citypairsTemp is null)
                        {
                            Console.WriteLine($"FlightCode not exists in databases : {sell.FlightNumberDetail}");
                            continue;
                        }
                        string personalCode = citypairsTemp
                            .CityPairSchedules
                            .FirstOrDefault(x => x.flightNumber.Equals(sell.FlightNumberDetail))
                            .typeSchedule
                            .PersonalCodeStatic();
                        ExportReportX5 reportX5 = new ExportReportX5
                        {
                            TaxCode = configuration["Export:TaxCode"],
                            InvoiceNumber = InvoiceNumberHash + DateTimeOffset.UtcNow.Ticks,
                            DeClaDateRe = deClaration.DeClaDateRe,
                            CurrencyType = "USD",
                            Explain = null,

                            PersonalCode = personalCode,
                            CustomerName = sell.CustomerName,
                            Passport = sell.PassportNumber,
                            Nationality = sell.Nationality,
                            FlightCode = sell.FlightNo,
                            TicketFlight = "THE_LEN_TAU_BAY",

                            DeClaDetailProductNumber = deClarationDetail.DeClaDetailProductNumber,
                            ProductCode = deClarationDetail.Product.ProductCode,
                            ProductName = deClarationDetail.Product.ProductName,
                            ProductUnit = deClarationDetail.Product.ProductUnit,
                            DeClaNumber = deClarationDetail.DeClaNumber,
                            DateAdded = deClaration.Content["dateadded"] is null ? "" : deClaration.Content["dateadded"].ToString(),
                            DeClaDetailQuantity = deClarationDetail.DeClaNumber,
                            DeClaDetailInvoicePrice = deClarationDetail.DeClaDetailInvoicePrice,
                            DeClaDetailInvoiceValue = deClarationDetail.DeClaDetailInvoiceValue
                        };
                        exportReportX5s.Add(reportX5);
                    }
                }
            }
            return exportReportX5s;
        }

        //Bảng kê 05
        public object Report(Report request)
        {
            List<ReportInventoryStatistics> resultReport = new List<ReportInventoryStatistics>();
            List<ReportInventoryStatistics> exports = new List<ReportInventoryStatistics>();
            List<ReportInventoryStatistics> notexports = new List<ReportInventoryStatistics>();
            // List không chứa hàng xuất trả

            List<string> cothangcha = mariaDBContext.Declarations.Where(x => x.DeClaType == 2 && x.DeClaStatus == DeClaStatus.confirm).Select(x => x.DeClaParentNumber).ToList(); // confirm check
            var nonexportdetails = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Include(x => x.Product)
                .Where(x => x.DeClaration.DeClaType == 1 && !cothangcha.Contains(x.DeClaNumber) && x.DeClaration.DeClaStatus == DeClaStatus.confirm
                && (
                    (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.DeClaNumber.Equals(request.search)))
                    || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.ProductCode.Equals(request.search)))
                    || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.Product.ProductName.Contains(request.search)))
                    )
                && (x.DeClaration.DeClaDateReData >= request.RegisterFromData && x.DeClaration.DeClaDateReData <= request.RegisterToData)
                ).GroupBy(x => new { DeClaNumber = x.DeClaNumber, ProductCode = x.ProductCode }).ToList();
            foreach (var item in nonexportdetails)
            {
                ReportInventoryStatistics temp = new ReportInventoryStatistics
                {
                    ProductCode = item.Key.ProductCode,
                    ImportDeclarationNumber = item.Key.DeClaNumber,
                    ImportQuantity = item.Sum(x => x.DeClaDetailQuantity),
                    SellQuantity = mariaDBContext.SealDetails.Include(x => x.Seal).Where(x => x.Seal.Status == (int)StatusSeals.SELL && x.ProductCode == item.Key.ProductCode && x.DeClaNumber == item.Key.DeClaNumber).Sum(x => x.QuantitySell),
                    ImportDeclaRegister = item.FirstOrDefault().DeClaration.DeClaDateRe,
                    ProductName = item.FirstOrDefault().Product.ProductName,
                    Price = Math.Round(item.FirstOrDefault().DeClaDetailInvoicePrice, 2),
                    ProductUnit = item.FirstOrDefault().Product.ProductUnit,
                    ImportInvoiceValue = Math.Round(item.FirstOrDefault().DeClaDetailInvoiceValue, 2),
                    DestroyQuantity = mariaDBContext.DestroyDetails.Include(x => x.Destroy).Where(x => x.DeClaNumber.Equals(item.Key.DeClaNumber) && x.ProductCode.Equals(item.Key.ProductCode) && x.Destroy.DestroyStatus == Destroys.Entities.DestroyStatus.CONFIRMED).Sum(x => x.DestroyDetailQuantity),
                    DestroyCode = mariaDBContext.DestroyDetails.Include(x => x.Destroy).Where(x => x.DeClaNumber.Equals(item.Key.DeClaNumber) && x.ProductCode.Equals(item.Key.ProductCode)).Select(x => x.Destroy.DestroyCode).Distinct().ToArray(),
                };
                temp.ImportInvoiceValue = Math.Round(temp.ImportQuantity * temp.Price, 2);
                temp.SellInvoiceValue = Math.Round(temp.SellQuantity * temp.Price, 2);
                temp.DestroyInvoiceValue = Math.Round(temp.DestroyQuantity * temp.Price);
                notexports.Add(temp);
            }

            // List chứa hàng xuất trả
            var exportdetails = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Include(x => x.Product)
                .Where(x => x.DeClaration.DeClaType == 1 && cothangcha.Contains(x.DeClaNumber) && x.DeClaration.DeClaStatus == DeClaStatus.confirm
                && (((request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.DeClaNumber.Equals(request.search)))
                || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.ProductCode.Equals(request.search)))
                || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.Product.ProductName.Contains(request.search))))
                && (x.DeClaration.DeClaDateReData >= request.RegisterFromData && x.DeClaration.DeClaDateReData <= request.RegisterToData)
                ))
                .GroupBy(x => new { ProductCode = x.ProductCode, DeClaNumberImport = x.DeClaration.DeClaNumber }).ToList();
            foreach (var item in exportdetails)
            {
                string exportKey = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaParentNumber.Equals(item.Key.DeClaNumberImport) && x.DeClaStatus == DeClaStatus.confirm).DeClaNumber;
                var checkExportDeclareNumberFolowProductCode = mariaDBContext.DeClarationDetails.FirstOrDefault(x => x.DeClaNumber.Equals(exportKey) && x.ProductCode.Equals(item.Key.ProductCode));
                ReportInventoryStatistics tmp = new ReportInventoryStatistics
                {
                    ProductCode = item.Key.ProductCode,
                    ImportDeclarationNumber = item.Key.DeClaNumberImport,
                    ImportQuantity = item.Sum(x => x.DeClaDetailQuantity),
                    SellQuantity = mariaDBContext.SealDetails.Include(x => x.Seal).Where(x => x.Seal.Status == (int)StatusSeals.SELL && x.ProductCode == item.Key.ProductCode && x.DeClaNumber == item.Key.DeClaNumberImport).Sum(x => x.QuantitySell),
                    ImportDeclaRegister = item.FirstOrDefault().DeClaration.DeClaDateRe,
                    ProductName = item.FirstOrDefault().Product.ProductName,
                    Price = Math.Round(item.FirstOrDefault().DeClaDetailInvoicePrice, 2),
                    ProductUnit = item.FirstOrDefault().Product.ProductUnit,
                    ImportInvoiceValue = Math.Round(item.FirstOrDefault().DeClaDetailInvoiceValue, 2),
                    ExportDeclarationNumber = checkExportDeclareNumberFolowProductCode?.DeClaNumber,
                    ExportQuantity = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(exportKey) && x.ProductCode.Equals(item.Key.ProductCode)).Sum(x => x.DeClaDetailQuantity),
                    ExportDeclaRegister = checkExportDeclareNumberFolowProductCode != null ? mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(checkExportDeclareNumberFolowProductCode.DeClaNumber)).DeClaDateRe : null,
                    ExportInvoiceValue = Math.Round(mariaDBContext.DeClarationDetails.FirstOrDefault().DeClaDetailInvoiceValue, 2),
                    DestroyQuantity = mariaDBContext.DestroyDetails.Include(x => x.Destroy).Where(x => x.DeClaNumber.Equals(item.Key.DeClaNumberImport) && x.ProductCode.Equals(item.Key.ProductCode) && x.Destroy.DestroyStatus == Destroys.Entities.DestroyStatus.CONFIRMED).Sum(x => x.DestroyDetailQuantity),
                    DestroyCode = mariaDBContext.DestroyDetails.Include(x => x.Destroy).Where(x => x.DeClaNumber.Equals(item.Key.DeClaNumberImport) && x.ProductCode.Equals(item.Key.ProductCode)).Select(x => x.Destroy.DestroyCode).Distinct().ToArray(),
                };
                tmp.SellInvoiceValue = Math.Round(tmp.SellQuantity * tmp.Price, 2);
                tmp.ImportInvoiceValue = Math.Round(tmp.ImportQuantity * tmp.Price, 2);
                tmp.ExportInvoiceValue = Math.Round(tmp.ExportQuantity * tmp.Price, 2);
                tmp.DestroyInvoiceValue = Math.Round(tmp.DestroyQuantity * tmp.Price, 2);
                exports.Add(tmp);
            }
            resultReport.AddRange(notexports);
            resultReport.AddRange(exports);
            return SupportResponseTable(request, resultReport);
        }

        public ResponseTable SupportResponseTable(Report request, List<ReportInventoryStatistics> resultReport)
        {
            int productAmount = resultReport.Select(x => x.ProductCode).Distinct().Count();
            int declarationAmount = resultReport.Select(x => x.ImportDeclarationNumber).Distinct().Count();
            ResponseTable responseTable = new ResponseTable();


            if (request.Type == 1)
            {
                resultReport = resultReport.AsQueryable().OrderBy(OrderValue(request.sortField, request.sortOrder)).ToList();
                responseTable.results = resultReport.Skip((request.page - 1) * request.results).Take(request.results).ToList();
                responseTable.info = new Info
                {
                    page = request.page,
                    totalRecord = resultReport.Count,
                    results = request.results
                };

            }
            else
            {
                responseTable.results = resultReport;
                responseTable.info = new Info
                {
                    page = 0,
                    totalRecord = 0,
                    results = 0
                };
            }

            responseTable.total = new
            {
                totalProductAmount = productAmount,
                totalDeclarationAmount = declarationAmount,
                totalImportQuantity = resultReport.Sum(x => x.ImportQuantity),
                totalImportInvoiceValue = Math.Round(resultReport.Sum(x => x.ImportInvoiceValue), 2),
                totalSellQuantity = resultReport.Sum(x => x.SellQuantity),
                totalSellInvoiceValue = Math.Round(resultReport.Sum(x => x.SellInvoiceValue), 2),
                totalExportQuantity = resultReport.Sum(x => x.ExportQuantity),
                totalExportInvoiceValue = Math.Round(resultReport.Sum(x => x.ExportInvoiceValue), 2),
                totalDestroyQuantity = resultReport.Sum(x => x.DestroyQuantity),
                totalDestroyInvoiceValue = Math.Round(resultReport.Sum(x => x.DestroyInvoiceValue), 2)
            };
            return responseTable;
        }


        //Báo cáo nhập xuất tồn 
        public object ListReportImport(Report request)
        {
            List<ReportImport> list = new List<ReportImport>();
            var Import = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration)
                .Where(x => x.DeClaration.DeClaType == 1 && x.DeClaration.DeClaStatus == DeClaStatus.confirm
                && (
                    (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.DeClaNumber.Equals(request.search)))
                    || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.ProductCode.Equals(request.search)))
                    || (request.search == null || String.IsNullOrEmpty(request.search) || (request.search != null && x.Product.ProductName.Contains(request.search)))
                    )
                && (x.DeClaration.DeClaDateReData >= request.RegisterFromData && x.DeClaration.DeClaDateReData <= request.RegisterToData)
                ).Include(x => x.Product).ToList();
            foreach (var item in Import)
            {
                ReportImport reportImport = new ReportImport
                {
                    DeclarationNumber = item.DeClaNumber,
                    DeclaRegister = item.DeClaration.DeClaDateRe,
                    ProductCode = item.Product.ProductCode,
                    ProductName = item.Product.ProductName,
                    Quantity = item.DeClaDetailQuantity,
                    TypeCode = "Tạm Nhập",
                    Unit = item.Product.ProductUnit
                };
                list.Add(reportImport);
            }
            if (request.Type == 1)
            {
                var result = list.AsQueryable().OrderBy(OrderValue(request.sortField, request.sortOrder)).ToList();
                ResponseTable responseTable = new ResponseTable()
                {
                    results = result.Skip((request.page - 1) * request.results).Take(request.results).ToList(),
                    info = new Info()
                    {
                        page = request.page,
                        totalRecord = result.Count,
                        results = request.results
                    },
                };
                return responseTable;
            }
            else
            {
                object result = new
                {
                    results = list,
                    info = new
                    {
                        results = 0,
                        page = 0,
                        totalRecord = 0
                    }
                };
                return result;
            }
        }

        //Báo cáo theo dõi tờ khai
        public object TrackLiquidity(TrackLiquidityRequest rq)
        {
            List<TrackLiquidity> trackLiquidities = new List<TrackLiquidity>();
            var import = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Include(x => x.Product)
                .Where(x => x.DeClaration.DeClaType == 1 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && (String.IsNullOrEmpty(rq.search) || x.DeClaNumber.Contains(rq.search)))
                .GroupBy(x => x.DeClaNumber).ToList();

            foreach (var item in import)
            {
                List<ChildData> lists = new List<ChildData>();
                foreach (var item1 in item)
                {
                    ChildData checkChildData = lists.FirstOrDefault(x => x.ProductCode.Equals(item1.ProductCode));
                    if (checkChildData != null)
                    {
                        checkChildData.ImportQuantity += item1.DeClaDetailQuantity;
                    }
                    else
                    {
                        ChildData childData = new ChildData
                        {
                            ProductCode = item1.ProductCode,
                            ProductName = item1.Product.ProductName,
                            Unit = item1.Product.ProductUnit,
                            Price = item1.DeClaDetailInvoicePrice,
                            ImportQuantity = item1.DeClaDetailQuantity,
                            ExportQuantity = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Where(x => x.DeClaration.DeClaType == 2 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && x.DeClaration.DeClaParentNumber.Equals(item.Key) && x.ProductCode.Equals(item1.ProductCode)).Sum(x => x.DeClaDetailQuantity),
                            SellQuantity = mariaDBContext.SealDetails.Include(x => x.Seal).Where(x => x.Seal.Status == (int)StatusSeals.SELL && x.ProductCode == item1.ProductCode && x.DeClaNumber == item.Key).Sum(x => x.QuantitySell),
                            DestroyQuantity = mariaDBContext.DestroyDetails.Include(x => x.Destroy).Where(x => x.DeClaNumber.Equals(item.Key) && x.ProductCode.Equals(item1.ProductCode) && x.Destroy.DestroyStatus == Destroys.Entities.DestroyStatus.CONFIRMED).Sum(x => x.DestroyDetailQuantity),
                            InventoryQuantity = mariaDBContext.Inventories.FirstOrDefault(x => x.DeNumber.Equals(item.Key) && x.ProductCode.Equals(item1.ProductCode)).InQuantity + mariaDBContext.SealDetails.Where(x => x.ProductCode == item1.ProductCode && x.DeClaNumber == item.Key).Include(x => x.Seal).Where(x => x.Seal.Status == (int)StatusSeals.EXPORT).Sum(x => x.QuantityExport),
                            SettlementDate = mariaDBContext.Inventories.FirstOrDefault(x => x.DeNumber.Equals(item.Key) && x.ProductCode.Equals(item1.ProductCode)).SettlementDate
                        };
                        childData.TotalQuantity = childData.SellQuantity + childData.ExportQuantity + childData.DestroyQuantity;
                        lists.Add(childData);
                    }
                }
                var checkDate = lists.Where(x => x.SettlementDate.HasValue).ToList();
                int inventoryQuantity = lists.Sum(x => x.InventoryQuantity);
                TrackLiquidity temp = new TrackLiquidity
                {
                    DeclarationNumber = item.Key,
                    DeclaRegister = item.FirstOrDefault().DeClaration.DeClaDateRe,
                    DeClaExtendedDispatch = item.FirstOrDefault().DeClaration.DeClaExtendedDispatch,
                    DeClaSettlementDispatch = item.FirstOrDefault().DeClaration.DeClaSettlementDispatch,
                    TotalImportQuantity = lists.Sum(x => x.ImportQuantity),
                    TotalExportQuantity = lists.Sum(x => x.TotalQuantity),
                    TotalInventoryQuantity = inventoryQuantity,
                    TotalProductquantity = lists.Count,
                    DeClaDateSettlementDispatch = checkDate.Count > 0 && inventoryQuantity == 0 ? checkDate.Select(x => x.SettlementDate).Max().Value : (DateTime?)null,
                    DeClaSettlementStatus = StatusResponse(inventoryQuantity, item.FirstOrDefault().DeClaration.DeClaDateReData, item.FirstOrDefault().DeClaration.DeClaExtendedDispatch),
                    ChildData = lists,
                    // thêm mới trường này  : : devhung
                    TotalChildData = lists.Count,
                    DeClaration = JObject.Parse(item.FirstOrDefault().DeClaration.DeClaContent).SelectToken("deadline"),
                    Color = ColorResponse(item.FirstOrDefault().DeClaration.DeClaDateReData),
                    NewDate = item.FirstOrDefault().DeClaration.DeClaNewDateV2,
                    _thenOrderBy = item.FirstOrDefault().DeClaration.DeClaNewDate
                };
                trackLiquidities.Add(temp);
            }

            if (rq.Type == 1)
            {
                var result = trackLiquidities.OrderByDescending(x => x.OrderBy).ThenByDescending(x => x.ThenOrderBy).ToList();
                ResponseTable responseTable = new ResponseTable()
                {
                    results = result.Skip((rq.page - 1) * rq.results).Take(rq.results).ToList(),
                    info = new Info()
                    {
                        page = rq.page,
                        totalRecord = result.Count,
                        results = rq.results

                    },
                    // thêm mới phân này  : devhung
                    total = new
                    {
                        totalDeClaration = result.Count,
                        totalProductInDeClatation = result.Sum(x => x.TotalChildData),
                        totalImport = result.Sum(x => x.TotalImportQuantity),
                        totalExport = result.Sum(x => x.TotalExportQuantity),
                        totalInventory = result.Sum(x => x.TotalInventoryQuantity)
                    }
                };
                return responseTable;
            }
            else
            {
                object result = new
                {
                    results = trackLiquidities.OrderByDescending(x => x.OrderBy).ThenByDescending(x => x.ThenOrderBy).ToList(),
                    info = new
                    {
                        results = 0,
                        page = 0,
                        totalRecord = 0
                    }
                };
                return result;
            }

        }

        public int StatusResponse(int inventoryQuantity, DateTime DateRegis, string DeClaExtendedDispatch)
        {
            if (inventoryQuantity == 0) // đã quyết toán
                return 1;
            int daytemp = DateTime.Now.Subtract(DateRegis).Days;
            if (daytemp <= 270) // tờ khai mới
                return 0;
            if (daytemp > 270 && String.IsNullOrEmpty(DeClaExtendedDispatch) /*&& monthTemp <= 330*/ )// cần quyết toán (xử lí gấp)
                return 3;
            if (daytemp > 270 && !String.IsNullOrEmpty(DeClaExtendedDispatch))
                return 2; // cần quyết toán (đã gia hạn)
            return -1;
        }


        public int ColorResponse(DateTime DateRegis)
        {
            int daytemp = DateTime.Now.Subtract(DateRegis).Days;
            if (daytemp <= 270) // Black
                return 1;
            else
                return 2; //Red
        }
        public (bool flag, string message) ConfirmSettlement(string DeclarationNumber, string DispatchNumber)
        {
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(DeclarationNumber));
            if (deClaration is null)
                return (false, "Tờ khai không tồn tại.");
            deClaration.DeClaSettlementDispatch = DispatchNumber;
            mariaDBContext.SaveChanges();
            return (true, "Thành công");
        }
        public (bool flag, string message) DispatchExtend(string DeclarationNumber, string DispatchNumber, DateTime RenewalDate, DateTime? dispatchDate)
        {
            DeClaration deClaration = mariaDBContext.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(DeclarationNumber));
            if (deClaration is null)
                return (false, "Tờ khai không tồn tại.");
            deClaration.DeClaExtendedDispatch = DispatchNumber;
            deClaration.DeClaRenewalDate = RenewalDate;
            deClaration.DeClaExtendedDispatchDate = dispatchDate;
            deClaration.DeClaNewDate = deClaration.DeClaDateReData.AddYears(2);

            // Lưu detail những sản phẩm của tờ khai khi gia hạn tờ khai
            List<DeclarationExtension> declarationExtensions = new List<DeclarationExtension>();
            List<DeClarationDetail> deClarationDetails = mariaDBContext.DeClarationDetails.Where(x => x.DeClaNumber.Equals(DeclarationNumber)).ToList();
            foreach (var deClarationDetail in deClarationDetails)
            {
                Inventory inventory = mariaDBContext.Inventories.FirstOrDefault(x => x.DeNumber.Equals(deClarationDetail.DeClaNumber) && x.ProductCode.Equals(deClarationDetail.ProductCode));
                if (inventory != null)
                {
                    // Lưu detail gia hạn tờ khai
                    declarationExtensions.Add(new DeclarationExtension
                    {
                        DeclarationNumber = deClarationDetail.DeClaNumber,
                        HsCode = deClarationDetail.DeClaDetailProductNumber,
                        ProductCode = deClarationDetail.ProductCode,
                        QuantityInventory = inventory.InQuantity
                    });
                }
            }
            mariaDBContext.DeclarationExtensions.AddRange(declarationExtensions);
            mariaDBContext.SaveChanges();
            return (true, "Thành công");
        }
        public object WarehouseReceipt(WarehouseReceiptRequest request)
        {
            List<WarehouseReceipt> warehouseReceipts = new List<WarehouseReceipt>();
            DateTime RegisterFromData = request.DateFrom == null || String.IsNullOrEmpty(request.DateFrom) ? DateTime.MinValue : DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
            DateTime RegisterToData = request.DateTo == null || String.IsNullOrEmpty(request.DateTo) ? DateTime.MaxValue : DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            var declarations = mariaDBContext.Declarations.Where(x =>
                x.DeClaType == 1
                && x.DeClaStatus == DeClaStatus.confirm
                && (String.IsNullOrEmpty(request.DeclarationNumber) || x.DeClaNumber.Equals(request.DeclarationNumber))
                && (x.DeClaDateReData <= RegisterToData && x.DeClaDateReData >= RegisterFromData))
                .Include(x => x.DeClarationDetails)
                .ThenInclude(y => y.Product)
                .ToList();
            foreach (var deClaration in declarations)
            {
                List<WarehouseProductReceipt> productReceipts = new List<WarehouseProductReceipt>();
                foreach (var deClarationDetail in deClaration.DeClarationDetails)
                {
                    var itemExist = productReceipts.FirstOrDefault(x => x.ProductCode.Equals(deClarationDetail.ProductCode));
                    if (itemExist != null)
                    {
                        itemExist.Quantity = deClarationDetail.DeClaDetailQuantity + itemExist.Quantity;
                    }
                    else
                    {
                        WarehouseProductReceipt productReceipt = new WarehouseProductReceipt
                        {
                            ProductCode = deClarationDetail.ProductCode,
                            ProductName = deClarationDetail.Product.ProductName,
                            Quantity = deClarationDetail.DeClaDetailQuantity,
                            Unit = deClarationDetail.Product.ProductUnit
                        };
                        productReceipts.Add(productReceipt);
                    }
                }
                WarehouseReceipt warehouseReceipt = new WarehouseReceipt
                {
                    DeclarationNumber = $"{deClaration.DeClaNumber} ({deClaration.DeClaDateRe})",
                    ReceivingDate = deClaration.Content["dateadded"]?.ToString(),
                    RcvOrderNo = deClaration.Content["importnumber"]?.ToString(),
                    Supplier = deClaration.Content["supplier"]?.ToString(),
                    DeliverersName = deClaration.Content["deliver"]?.ToString(),
                    Weight = deClaration.Content["gross"]?.ToString(),
                    ProductReceipts = productReceipts
                };
                warehouseReceipts.Add(warehouseReceipt);
            }
            return warehouseReceipts;
        }

        public object DeliveryReceipt(WarehouseReceiptRequest request)
        {
            DateTime RegisterFromData = request.DateFrom == null || String.IsNullOrEmpty(request.DateFrom) ? DateTime.MinValue : DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
            DateTime RegisterToData = request.DateTo == null || String.IsNullOrEmpty(request.DateTo) ? DateTime.MaxValue : DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            List<DeliveryReceipt> deliveryReceipts = new List<DeliveryReceipt>();
            var exportDeclarations = mariaDBContext.Declarations.Where(x =>
            x.DeClaType == 2
            && x.DeClaStatus == DeClaStatus.confirm
            && (String.IsNullOrEmpty(request.DeclarationNumber) || x.DeClaNumber.Equals(request.DeclarationNumber))
            && (x.DeClaDateReData <= RegisterToData && x.DeClaDateReData >= RegisterFromData))
                .Include(x => x.DeClarationDetails)
                .ThenInclude(x => x.Product)
                .ToList();
            foreach (var exportDeClaration in exportDeclarations)
            {
                List<DeliveryProductReceipt> deliveryProductReceipts = new List<DeliveryProductReceipt>();
                foreach (var deClarationDetail in exportDeClaration.DeClarationDetails)
                {
                    var itemExist = deliveryProductReceipts.FirstOrDefault(x => x.ProductCode.Equals(deClarationDetail.ProductCode));
                    if (itemExist != null)
                    {
                        int? quantityImport = mariaDBContext.DeClarationDetails.FirstOrDefault(x => x.DeClaNumber.Equals(exportDeClaration.DeClaParentNumber) && x.ProductCode.Equals(deClarationDetail.ProductCode))?.DeClaDetailQuantity;
                        int quantityExport = deClarationDetail.DeClaDetailQuantity;
                        itemExist.QuantityExport = itemExist.QuantityExport + quantityExport;
                        itemExist.QuantityImport = itemExist.QuantityImport + quantityImport;
                    }
                    else
                    {
                        DeliveryProductReceipt deliveryProductReceipt = new DeliveryProductReceipt()
                        {
                            ProductCode = deClarationDetail.ProductCode,
                            DeclarationNumberImport = $"{exportDeClaration.DeClaParentNumber} ({exportDeClaration.DeClaDateRe})",
                            Unit = deClarationDetail.Product.ProductUnit,
                            ProductName = deClarationDetail.Product.ProductName,
                            QuantityExport = deClarationDetail.DeClaDetailQuantity,
                            QuantityImport = mariaDBContext.DeClarationDetails.FirstOrDefault(x => x.DeClaNumber.Equals(exportDeClaration.DeClaParentNumber) && x.ProductCode.Equals(deClarationDetail.ProductCode))?.DeClaDetailQuantity
                        };
                        deliveryProductReceipts.Add(deliveryProductReceipt);
                    }
                }
                DeliveryReceipt deliveryReceipt = new DeliveryReceipt()
                {
                    DeclarationNumber = $"{exportDeClaration.DeClaNumber} ({exportDeClaration.DeClaDateRe})",
                    ExportDate = exportDeClaration.Content["dateexported"]?.ToString(),
                    DeliveryOrderFrom = "VietJet Air",
                    ExportNumber = exportDeClaration.Content["exportnumber"]?.ToString(),
                    WaybillnNumber = exportDeClaration.Content["waybillnumber"]?.ToString(),
                    DeliveryProductReceipts = deliveryProductReceipts
                };
                deliveryReceipts.Add(deliveryReceipt);
            }
            return deliveryReceipts;
        }
        public object ExtensionDispatchReport(WarehouseReceiptRequest request)
        {
            DateTime RegisterFromData = request.DateFrom == null || String.IsNullOrEmpty(request.DateFrom) ? DateTime.MinValue : DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
            DateTime RegisterToData = request.DateTo == null || String.IsNullOrEmpty(request.DateTo) ? DateTime.MaxValue : DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            List<DeclarationExtensionReport> declarationExtensionReports = new List<DeclarationExtensionReport>();
            var importDeclarations = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Include(x => x.Product)
                .Where(x =>
                x.DeClaration.DeClaType == 1
                && x.DeClaration.DeClaStatus == DeClaStatus.confirm
                && (String.IsNullOrEmpty(request.DeclarationNumber) || x.DeClaNumber.Equals(request.DeclarationNumber))
                && (x.DeClaration.DeClaDateReData <= RegisterToData && x.DeClaration.DeClaDateReData >= RegisterFromData))
                .GroupBy(x => x.DeClaNumber).ToList();


            foreach (var importDeclaration in importDeclarations)
            {
                List<HSData> hsDatas = new List<HSData>();

                var hsCodes = importDeclaration.GroupBy(x => x.DeClaDetailProductNumber).ToList();


                foreach (var itemImport in hsCodes)
                {
                    List<string> productCodes = itemImport.Select(x => x.ProductCode).ToList();
                    HSData hsData = new HSData
                    {
                        HSCode = itemImport.Key,
                        HSImportQuantity = itemImport.Sum(x => x.DeClaDetailQuantity),
                        HSExportQuantity = mariaDBContext.DeClarationDetails.Include(x => x.DeClaration).Where(x => x.DeClaration.DeClaType == 2 && x.DeClaration.DeClaStatus == DeClaStatus.confirm && x.DeClaration.DeClaParentNumber.Equals(importDeclaration.Key) && x.DeClaDetailProductNumber.Equals(itemImport.Key)).Sum(x => x.DeClaDetailQuantity),
                        //HSInventoryQuantity = mariaDBContext.Inventories.Where(x => x.DeNumber.Equals(importDeclaration.Key) && productCodes.Contains(x.ProductCode)).Sum(x => x.InQuantity),
                        HSInventoryQuantity = mariaDBContext.DeclarationExtensions.Where(x => x.DeclarationNumber.Equals(importDeclaration.Key) && x.HsCode.Equals(itemImport.Key)).Sum(x => x.QuantityInventory),
                        HSRenewalQuantity = mariaDBContext.Inventories.Where(x => x.DeNumber.Equals(importDeclaration.Key) && productCodes.Contains(x.ProductCode)).Sum(x => x.InQuantity)
                    };
                    hsDatas.Add(hsData);
                }
                DeclarationExtensionReport declarationExtensionReport = new DeclarationExtensionReport
                {
                    TaxCode = configuration["Export:TaxCode"],
                    DeclarationNumber = importDeclaration.Key,
                    DeclarationDate = importDeclaration.FirstOrDefault()?.DeClaration?.DeClaDateRe,
                    DispatchNumber = importDeclaration.FirstOrDefault()?.DeClaration?.DeClaExtendedDispatch,
                    RenewalDate = importDeclaration.FirstOrDefault()?.DeClaration?.DeClaRenewalDate?.ToString("dd/MM/yyyy"),
                    RenewalDateTo = importDeclaration.FirstOrDefault()?.DeClaration?.DeClaNewDateV2,
                    HSDatas = hsDatas,
                    DispatchDate = importDeclaration.FirstOrDefault().DeClaration.DeClaExtendedDispatchDate.HasValue ? importDeclaration.FirstOrDefault()?.DeClaration?.DeClaExtendedDispatchDate.Value.ToString("dd/MM/yyyy") : null
                };
                declarationExtensionReports.Add(declarationExtensionReport);
            }
            return declarationExtensionReports;
        }
    }
}
