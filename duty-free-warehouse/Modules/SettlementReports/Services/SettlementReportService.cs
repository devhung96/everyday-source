using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Response;
using Project.Modules.SettlementReports.Entities;
using Project.Modules.SettlementReports.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SettlementReports.Services
{
    public interface ISettlementReportService
    {
        (DataPaginationResponse data, string message) showAll(InputFiterRequest inputFiter);
    }
    public class SettlementReportService : ISettlementReportService
    {
        private readonly MariaDBContext _mariaDBContext;
        private readonly IConfiguration configuration;
        public SettlementReportService(MariaDBContext mariaDBContext, IConfiguration _configuration)
        {
            _mariaDBContext = mariaDBContext;
            configuration = _configuration;
        }
        public (string fromDate, string toDate) FromAndToDate(InputFiterRequest inputFiter)
        {
            if (String.IsNullOrEmpty(inputFiter.dateFrom))
            {
                inputFiter.dateFrom = "01/01/2020";
            }
            if (String.IsNullOrEmpty(inputFiter.dateTo))
            {
                inputFiter.dateTo = DateTime.Now.ToString("dd/MM/yyyy");
            }
            return (inputFiter.dateFrom, inputFiter.dateTo);
        }
        public double paginationsCheck(DataPaginationResponse paginations)
        {
            if (paginations.lastePage < paginations.page)
            {
                paginations.page = paginations.lastePage;
            }
            if (paginations.page <= 0)
            {
                paginations.page = 1;
            }
            return paginations.page;
        }
        public (DataPaginationResponse data, string message) showAll(InputFiterRequest inputFiter)
        {
            
            (inputFiter.dateFrom, inputFiter.dateTo) = FromAndToDate(inputFiter);
            bool checkDayDateTimeForm = DateTime.TryParseExact(inputFiter.dateFrom, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime DateTimeForm);
            if (!checkDayDateTimeForm)
            {
                return (null, "DateTimeForm sai định dạng ngày!!!");
            }
            bool checkDayDateTimeTo = DateTime.TryParseExact(inputFiter.dateTo, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime DateTimeTo);
            if (!checkDayDateTimeTo)
            {
                return (null, "DateTimeForm sai định dạng ngày!!!");
            }
            var deClarationDetails = _mariaDBContext.DeClarationDetails
               .Include(x => x.DeClaration)
               .Include(x => x.Product)
               .Where(
                x =>
                    x.DeClaration.DeClaStatus == DeClaStatus.confirm
                     )
               .ToList();
            List<SettlementReport> reports = new List<SettlementReport>();
            foreach (var deClarationDetail in deClarationDetails.GroupBy(x => x.ProductCode))
            {
                // Nhập đầu kỳ
                int nhapDauKy = deClarationDetails.Where
                    (
                        x =>
                            x.ProductCode.Contains(deClarationDetail.Key)
                            && x.DeClaration.DeClaDateReData < DateTimeForm
                            && x.DeClaration.DeClaType == 1 // 1 là phiếu nhập
                    )
                     .Sum(x => x.DeClaDetailQuantity);

                // Xuất trả đầu kỳ
                int xuatTraDauKy = deClarationDetails.Where
                    (
                        x =>
                            x.ProductCode.Contains(deClarationDetail.Key)
                            && x.DeClaration.DeClaDateReData < DateTimeForm
                            && x.DeClaration.DeClaType == 2 // 2 là phiếu xuất
                    )
                     .Sum(x => x.DeClaDetailQuantity);

                // Xuất bán đầu kỳ
                int xuatBanDauKy = _mariaDBContext.SealDetails
                    .Include(x => x.DeClaration)
                    .Where(
                            x =>
                                x.Seal.Status == (int)StatusSeals.SELL
                                && x.ProductCode == deClarationDetail.Key
                                && x.Seal.FlightDate < DateTimeForm
                          ).Sum(x => x.QuantitySell);

                // Xuất hủy đầu kỳ
                int xuatHuyDauKy = _mariaDBContext.DestroyDetails
                    .Include(x => x.DeClaration)
                    .Where(
                        x =>
                            x.Destroy.DestroyStatus == DestroyStatus.CONFIRMED
                            && x.ProductCode == deClarationDetail.Key
                            && x.DeClaration.DeClaDateReData < DateTimeForm

                    ).Sum(x => x.DestroyDetailQuantity);

                // Nhập trong kỳ
                int nhapTrongKy = deClarationDetails.Where
                    (
                        x =>
                            x.ProductCode.Contains(deClarationDetail.Key)
                            && x.DeClaration.DeClaType == 1 // 1 là phiếu nhập
                            && x.DeClaration.DeClaDateReData >= DateTimeForm
                            && x.DeClaration.DeClaDateReData <= DateTimeTo
                    ).Sum(x => x.DeClaDetailQuantity);

                // Xuất trả trong kỳ
                int xuatTraTrongKy = deClarationDetails.Where
                    (
                        x =>
                            x.ProductCode.Contains(deClarationDetail.Key)
                            && x.DeClaration.DeClaType == 2 // 2 là phiếu xuất
                            && x.DeClaration.DeClaDateReData >= DateTimeForm
                            && x.DeClaration.DeClaDateReData <= DateTimeTo
                    ).Sum(x => x.DeClaDetailQuantity);

                // Xuất bán trong kỳ
                int xuatBanTrongKy = _mariaDBContext.SealDetails
                    .Include(x => x.DeClaration)
                    .Where(
                            x =>
                                x.Seal.Status == (int)StatusSeals.SELL
                                && x.ProductCode == deClarationDetail.Key
                                && x.Seal.FlightDate >= DateTimeForm
                                && x.Seal.FlightDate <= DateTimeTo
                          ).Sum(x => x.QuantitySell);

                // Xuất hủy trong kỳ
                int xuatHuyTrongKy = _mariaDBContext.DestroyDetails
                    .Where(
                        x =>
                            x.Destroy.DestroyStatus == DestroyStatus.CONFIRMED
                            && x.ProductCode == deClarationDetail.Key
                            && x.Destroy.DestroyDate >= DateTimeForm
                            && x.Destroy.DestroyDate <= DateTimeTo
                    ).Sum(x => x.DestroyDetailQuantity);

                int tonDauKy = nhapDauKy - (xuatTraDauKy + xuatBanDauKy + xuatHuyDauKy);
                int xuatTrongKy = xuatBanTrongKy + xuatTraTrongKy + xuatHuyTrongKy;
                SettlementReport settlementReport = new SettlementReport();
                settlementReport.ProductNumber = deClarationDetail.FirstOrDefault().DeClaDetailProductNumber;
                settlementReport.codeProduct = deClarationDetail.Key;
                if(deClarationDetail.FirstOrDefault().Product != null)
                {
                    settlementReport.nameProduct = deClarationDetail.FirstOrDefault().Product.ProductName;
                    settlementReport.unit = deClarationDetail.FirstOrDefault().Product.ProductUnit;
                }else
                {
                    settlementReport.nameProduct = null;
                    settlementReport.unit = null;
                }
                settlementReport.tonDauKy = tonDauKy;
                settlementReport.nhapTrongKy = nhapTrongKy;
                settlementReport.xuatTrongKy = xuatTrongKy;
                settlementReport.tonCuoiKy = tonDauKy + nhapTrongKy - xuatTrongKy;
                settlementReport.TaxCode = configuration["Export:TaxCode"];
                settlementReport.NamBC = DateTime.ParseExact(inputFiter.dateTo, "dd/MM/yyyy", null);
                reports.Add(settlementReport);
            }
            reports = reports.Where(x => String.IsNullOrEmpty(inputFiter.Search) || x.nameProduct.Contains(inputFiter.Search) || x.codeProduct.Contains(inputFiter.Search)).OrderBy(x => x.codeProduct).ToList();
            DataPaginationResponse paginations = new DataPaginationResponse();
            if (inputFiter.perPage == null || inputFiter.page == null)
            {
                paginations.total = reports.Count;
                paginations.data = reports.ToArray();
                paginations.TongTonDauKy = reports.Select(x => x.tonDauKy).Sum();
                paginations.TongNhapTrongKy = reports.Select(x => x.nhapTrongKy).Sum();
                paginations.TongXuatTrongKy = reports.Select(x => x.xuatTrongKy).Sum();
                paginations.TongTonCuoiKy = reports.Select(x => x.tonCuoiKy).Sum();
                return (paginations, "Show All success!!!");
            }
            double lastePage = 0;
            paginations.perPage = (double)inputFiter.perPage;
            paginations.page = (double)inputFiter.page;
            paginations.total = reports.Count;
            lastePage = paginations.total / paginations.perPage;
            paginations.lastePage = Math.Ceiling(lastePage);
            paginations.page = paginationsCheck(paginations);
            double skip = (paginations.page - 1) * paginations.perPage;
            paginations.data = reports.Skip((int)Math.Ceiling(skip)).Take((int)paginations.perPage).ToArray();
            // Tính tổng
            paginations.TongTonDauKy = reports.Select(x => x.tonDauKy).Sum();
            paginations.TongNhapTrongKy = reports.Select(x => x.nhapTrongKy).Sum();
            paginations.TongXuatTrongKy = reports.Select(x => x.xuatTrongKy).Sum();
            paginations.TongTonCuoiKy = reports.Select(x => x.tonCuoiKy).Sum();
            return (paginations, "Show All success!!!");
        }

    }
}
