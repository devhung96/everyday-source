using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Declarations.Requests;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using Project.Modules.Inventories.Entites;
using Project.Modules.Seals.Entity;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Project.Modules.Declarations.Requests.ExportReponse;

namespace Project.Modules.Declarations.Services
{
    public interface IExportService
    {
        (List<ExportReponse> inventoryN1s, string message) ExportN1(ExportRequest export);
        (List<ExportReponse> inventoryN1s, string message) ExportX1(ExportRequest export);
    }
    public class ExportService : IExportService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IConfiguration configuration;
        public ExportService(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.mariaDBContext = mariaDBContext;
        }

        public (List<ExportReponse> inventoryN1s, string message) ExportN1(ExportRequest export)
        {
            InforExport infor = new InforExport
            {
                TaxCode = configuration["Export:TaxCode"],
                WarehouseCode = configuration["Export:WHCode"],
                ShopCode = configuration["Export:ShopCode"],
                AccountingDate = DateTime.Now // update 24102020
            };
            string noBill = "N1" + infor.AccountingDate.ToString("yyyyMMdd");
            List<string> deSeach = new List<string>();
            DateTime searchTo;
            DateTime searchFrom;
          

            List<DeClaration> deClarations = mariaDBContext.Declarations.Where(m => m.DeClaType == 1 && m.DeClaStatus == DeClaStatus.confirm).ToList();
            List<DeClaration> deClarationsExports = mariaDBContext.Declarations.Where(m => m.DeClaType == 2 && m.DeClaStatus == DeClaStatus.confirm).ToList();

            if (string.IsNullOrEmpty(export.DateFrom))
            {
                export.DateFrom = DateTime.Now.ToString();
            }
            export.DeDateFrom = DateTime.ParseExact(export.DateFrom, "dd/MM/yyyy", null);
            searchTo = export.DeDateFrom.Value.AddDays(-1);
            searchFrom = new DateTime(searchTo.Year, 1, 1);

            #region Lay danh sach to khai thoa man dieu kien
          
            deClarations = deClarations.Where(m => (
                                                        !m.DeClaNewDate.HasValue
                                                        && m.DeClaDateReData >= searchFrom
                                                        && m.DeClaDateReData <= searchTo
                                                     )
                                                   || (
                                                       m.DeClaNewDate.HasValue
                                                       && m.DeClaDateReData <= searchTo
                                                       && m.DeClaNewDate.Value.Date.AddYears(1) >= searchFrom
                                                       )
                                                  ).ToList();
            #endregion

            deSeach = deClarations.Select(m => m.DeClaNumber).ToList();
            List<DeClarationDetail> deClarationDetails = mariaDBContext.DeClarationDetails
                                                                       .Where(m => deSeach.Contains(m.DeClaNumber))
                                                                       .Include(m => m.Product)
                                                                       .OrderBy(m => m.DeClaNumber)
                                                                       .ToList();

            List<ExportReponse> results = new List<ExportReponse>();

            foreach (DeClarationDetail deDetail in deClarationDetails)
            {
                DeClaration deClaration =  deClarations.FirstOrDefault(m => m.DeClaNumber.Equals(deDetail.DeClaNumber));
                DateTime dateAdd;
                DateTime.TryParseExact(deClaration.Content["dateadded"].ToString(),"dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateAdd);

                // tong xuat huy 

                #region Danh sach hang huy trong khoang từ ngày nhập tới ngày TO

                List<int> idDestroys = mariaDBContext.Destroys
                                                     .Where(m => m.DestroyRequestDate >= dateAdd && m.DestroyRequestDate <= searchTo)
                                                     .Select(m => m.DestroyId)
                                                     .ToList();

                List<DestroyDetail> destroyDetails = mariaDBContext.DestroyDetails
                                                                   .Where(m => idDestroys.Contains(m.DestroyId))
                                                                   .ToList();
                #endregion

                int sumDestroy = destroyDetails.Where(m => m.DeClaNumber.Equals(deDetail.DeClaNumber) && m.ProductCode.Equals(deDetail.ProductCode))
                                               .Sum(m => m.DestroyDetailQuantity);
                // tong ban 
                #region Danh sach hang ban trong khoang Nhap=>To

                List<string> sealNumbers = mariaDBContext.Seals
                                                          .Where(m => m.FlightDate >= dateAdd && m.FlightDate <= searchTo)
                                                          .Select(m => m.SealNumber)
                                                          .ToList();

                List<SealDetail> sealDetails = mariaDBContext.SealDetails.Where(m => sealNumbers.Contains(m.SealNumber)).ToList();
                #endregion

                int sumSealDetails = sealDetails.Where(m => m.DeClaNumber.Equals(deDetail.DeClaNumber) && m.ProductCode.Equals(deDetail.ProductCode))
                                                .Sum(m=>m.QuantitySell);
                // tong tai xuat 

                #region Danh sach to khai tai nhap trong khoang nhap => to

                List<DeClaration> deExports = deClarationsExports.Where(m => m.DeClaParentNumber.Equals(deDetail.DeClaNumber)).ToList();
                List<string> deNumberExports = deExports.Where(m => m.DeClaDateReData > dateAdd && m.DeClaDateReData <= searchTo)
                                                    .Select(m => m.DeClaNumber)
                                                    .ToList();
                List<DeClarationDetail> deDetailExports = mariaDBContext.DeClarationDetails.Where(m => deNumberExports.Contains(m.DeClaNumber)).ToList();
               
                #endregion

                int sumComeBack = deDetailExports.Where(m => m.DeClaNumber.Equals(deDetail.DeClaNumber) && m.ProductCode.Equals(deDetail.ProductCode))
                                                  .Sum(m => m.DeClaDetailQuantity);

                deDetail.DeClaDetailQuantity = deDetail.DeClaDetailQuantity - sumDestroy - sumSealDetails - sumComeBack;
               
                if (deDetail.DeClaDetailQuantity>0)
                {
                    results.Add(new ExportReponse(deClaration, deDetail, infor, noBill + deDetail.DeClaNumber + deDetail.DeClaDetailProductNumber));
                }
            }
            return (results, "Xuất tồn đầu thành công.");
        }

        public (List<ExportReponse> inventoryN1s, string message) ExportX1(ExportRequest export)
        {
            InforExport infor = new InforExport
            {
                TaxCode = configuration["Export:TaxCode"],
                WarehouseCode = configuration["Export:WHCode"],
                WarehouseCodeOut = configuration["Export:OutCode"],
                AccountingDate = DateTime.Now
            };
            string noBill = "X1";
            List<ExportReponse> exportReponses = new List<ExportReponse>();
            List<ExportReponse> results = new List<ExportReponse>();
            List<int> detailIds = new List<int>();
            List<Destroy> destroys = mariaDBContext.Destroys.Where(m=>m.DestroyStatus== DestroyStatus.CONFIRMED).ToList();
            if (!string.IsNullOrEmpty(export.DateFrom) && !string.IsNullOrEmpty(export.DateTo))
            {
                export.DeDateFrom = DateTime.ParseExact(export.DateFrom, "dd/MM/yyyy", null);
                export.DeDateTo = DateTime.ParseExact(export.DateTo, "dd/MM/yyyy", null);
                destroys = destroys.Where(m =>
                                              m.DestroyDate >= export.DeDateFrom.Value.Date
                                              && m.DestroyDate <= export.DeDateTo.Value.Date)
                                          .ToList();
            }

            foreach (Destroy destroy in destroys)
            {
                detailIds = mariaDBContext.DestroyDetails.Where(m => m.DestroyId == destroy.DestroyId)
                                                         .Select(m => m.DestroyDetailId)
                                                         .ToList();
                foreach (int detailId in detailIds)
                {
                    exportReponses = mariaDBContext.DestroyDetails
                                                   .Where(m => m.DestroyDetailId == detailId).Include(m => m.Product)
                                                   .Include(m => m.DeClaration)
                                                   .Select(m => new ExportReponse(m, m.Product, infor, noBill + destroy.DestroyCode + m.DeClaNumber + m.ProductCode, m.DeClaration))
                                                   .ToList();
                    results.AddRange(exportReponses);
                }
            }
            return (results, "Xuất thành công X1");
        }
    }
}
