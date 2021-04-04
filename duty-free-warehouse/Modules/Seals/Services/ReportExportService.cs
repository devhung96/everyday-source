using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Destroys.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Response;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Services
{
    public interface IReportExportService
    {
        TableResponse ShowData(ReportExportRequest request);
        List<ReportX12X11Response> ReportX11(DateTime? dateFrom, DateTime? dateTo);
        List<ReportX12X11Response> ReportX12(DateTime? dateFrom, DateTime? dateTo);
        List<ReportX5Response> ReportX5(DateTime? dateFrom, DateTime? dateTo);
    }
    public class ReportExportService : IReportExportService
    {
        public readonly MariaDBContext dBContext;
        public readonly IConfiguration configuration;
        public ReportExportService(MariaDBContext DbContext, IConfiguration Configuration)
        {
            dBContext = DbContext;
            configuration = Configuration;
        }
        public TableResponse ShowData(ReportExportRequest request)
        {
            List<ReportExportResponse> reportExportResponse = new List<ReportExportResponse>();
            List<ReportExportResponse> sells = dBContext.SellDetails
                .Include(x => x.Sell)
                .Include(x => x.Product)
                .Select(x => new ReportExportResponse(x))
                .ToList();
            List<ReportExportResponse> declarations = dBContext.DeClarationDetails
                .Include(x => x.DeClaration)
                .Include(x => x.Product)
                .Where(x => x.DeClaration.DeClaType.Equals(2))
                .Select(x => new ReportExportResponse(x))
                .ToList();
            List<ReportExportResponse> destroys = dBContext.DestroyDetails
                .Include(x => x.Destroy)
                .Include(x => x.Product)
                .Where(x => x.Destroy.DestroyStatus == DestroyStatus.CONFIRMED)
                .Select(x => new ReportExportResponse(x))
                .ToList();
                
            reportExportResponse.AddRange(sells);
            reportExportResponse.AddRange(declarations);
            reportExportResponse.AddRange(destroys);

            reportExportResponse = reportExportResponse
                    .Where(x => (
                        string.IsNullOrEmpty(request.DateFrom) ||
                        string.IsNullOrEmpty(request.DateTo) ||
                        (x.DateSell >= request.DateFromObj && x.DateSell <= request.DateToObj)))
                    .Where(x => (
                        string.IsNullOrEmpty(request.Search) ||
                        x.ProductCode.Contains(request.Search) ||
                        x.ProductName.Contains(request.Search) ||
                        (x.Invoice != null && x.Invoice.Contains(request.Search))
                    )).ToList();
            int countData = reportExportResponse.Count;
            long sumProduct = reportExportResponse.Sum(x => x.ProductQuantity); // tổng sản phẩm sau khi lọc, tất cả các trang
            if (request.Page > 0)
            {
                reportExportResponse = reportExportResponse
                    .Skip(request.Limit * (request.Page - 1))
                    .Take(request.Limit)
                    .ToList();
            }
            TableResponse tableResponse = new TableResponse
            {
                TotalProduct = sumProduct,
                TotalRecord = countData,
                Limit = request.Limit,
                Page = request.Page,
                Data = reportExportResponse
            };
            return tableResponse;
        }

        public List<ReportX12X11Response> ReportX11(DateTime? dateFrom, DateTime? dateTo)
        {
            List<Seal> seals = dBContext.Seals
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.Product)
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.DeClaration)
                .ThenInclude(x => x.DeClarationDetails)
                .Where(x =>
                    (x.Status.Equals((int)StatusSeals.EXPORT) || x.Status.Equals((int)StatusSeals.SELL)) 
                    && (dateFrom == null || dateTo == null || (x.FlightDate >= dateFrom && x.FlightDate <= dateTo))
                )
                .ToList();

            List<ReportX12X11Response> reportX12Responses = new List<ReportX12X11Response>();
            foreach (var item in seals)
            {
                reportX12Responses.AddRange(new ReportX12X11Response().GenerateData(item, configuration, 11));
            }
            return reportX12Responses;
        }
        public List<ReportX12X11Response> ReportX12(DateTime? dateFrom, DateTime? dateTo)
        {
            List<Seal> seals = dBContext.Seals
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.Product)
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.DeClaration)
                .ThenInclude(x => x.DeClarationDetails)
                .Where(x => 
                    x.Status.Equals((int)StatusSeals.SELL) && (dateFrom == null || dateTo == null || (x.FlightDate >= dateFrom && x.FlightDate <= dateTo))
                )
                .ToList();
            List<ReportX12X11Response> reportX12Responses = new List<ReportX12X11Response>();
            foreach (var item in seals)
            {
                reportX12Responses.AddRange(new ReportX12X11Response().GenerateData(item, configuration, 12));
            }
            return reportX12Responses;
        }

        public List<ReportX5Response> ReportX5(DateTime? dateFrom, DateTime? dateTo)
        {
            List<Seal> seals = dBContext.Seals
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.Product)
                .Include(x => x.SealDetails)
                .ThenInclude(x => x.DeClaration)
                .ThenInclude(x => x.DeClarationDetails)
                .Where(x =>
                    (x.Status.Equals((int)StatusSeals.EXPORT) || x.Status.Equals((int)StatusSeals.SELL)) && (dateFrom == null || dateTo == null || (x.FlightDate >= dateFrom && x.FlightDate <= dateTo))
                )
                .ToList();

            List<Sell> sells = dBContext.Sells
                .Include(x => x.SellDetails)
                .ThenInclude(x => x.Product)
                .Where(x => (dateFrom == null || dateTo == null || (x.FlightDate >= dateFrom && x.FlightDate <= dateTo)))
                .ToList();

            List<ReportX5Response> reportX5Response = new List<ReportX5Response>();
            foreach (var item in sells)
            {
                // Check đối tượng
                Citypair citypair = dBContext.Citypairs.Where(x => x.Schedule.Contains(item.FlightNumberDetail)).FirstOrDefault();

                int? typeDoiTuong = null;
                if(citypair != null)
                {
                    Schedule a = JsonConvert.DeserializeObject<List<Schedule>>(citypair.Schedule)
                        .FirstOrDefault(x => x.flightNumber.Equals(item.FlightNumberDetail));
                    typeDoiTuong = a?.typeSchedule == 1 ? 9 : 10;
                }
                    
                reportX5Response.AddRange(new ReportX5Response().
                    GenerateData(seals.FirstOrDefault(x => x.FlightDate.Equals(item.FlightDate) && x.FlightNumber.Contains(item.FlightNo)), 
                    item,
                    typeDoiTuong,
                    configuration));
            }

            return reportX5Response;
        }
    }
}
