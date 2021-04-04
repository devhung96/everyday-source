using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Destroys.Entities;
using Project.Modules.Destroys.Requests;
using Project.Modules.Inventories.Entites;
using Project.Modules.Seals.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Services
{

    public interface IDestroyService
    {
        (Destroy destroy, string message) Store(Destroy destroy);
        (Destroy destroy, string message) Delete(int idDestroy);
        (Destroy destroy, string message) ShowDestroy(int destroyId);
        (object data, string message) ShowAll(RequestTableShowAllDestroy requestTableShowAllDestroy);
        (Destroy data, string message) Confirm(int destroyId);

        (object result, string message) BaoCaoTonKhoTheoNgay(string search);
        (object result, string message) BaoCaoTonKhoTheoNgay(RequestTable requestTable);
        (bool check, string mess) UpdateDestroyDate(int destroyId, DateTime? DestroyDate);
    }
    public class DestroyService: IDestroyService
    {
        private readonly MariaDBContext _context;
        public DestroyService(MariaDBContext context)
        {
            _context = context;
        }

        public (Destroy data, string message) Confirm(int destroyId)
        {
            Destroy destroy = _context.Destroys.Find(destroyId);
            if (destroy == null)
                return (null, "Lịch sử hủy không tồn tại!");
            if (destroy.DestroyStatus == DestroyStatus.CONFIRMED) return (null, "Lịch sử hủy đã được xác nhận!");
            List<DestroyDetail> destroyDetails = _context.DestroyDetails.Where(x => x.DestroyId == destroyId).ToList();
            foreach (DestroyDetail item in destroyDetails)
            {
                Inventory inventory = _context.Inventories.FirstOrDefault(x => x.ProductCode == item.ProductCode && x.DeNumber == item.DeClaNumber);
                if (inventory == null) return (null, $"Sản phẩm :{item.ProductCode} không tồn tại trong kho!");
                if (item.DestroyDetailQuantity > inventory.InQuantity) return (null, $"Sản phẩm :{item.ProductCode} không đủ số lượng trong  kho!");
                inventory.InQuantity = inventory.InQuantity - item.DestroyDetailQuantity;
            }
            destroy.DestroyStatus = DestroyStatus.CONFIRMED;
            _context.SaveChanges();
            return (destroy, "Xác nhận thành công!");
        }

        public (Destroy destroy, string message) Delete(int idDestroy)
        {
            Destroy destroy = _context.Destroys.FirstOrDefault(x => x.DestroyId == idDestroy);
            if (destroy == null) return (null, "Lịch sử không tìm thấy!");
            _context.Destroys.Remove(destroy);
            _context.SaveChanges();
            return (destroy, "Xóa lịch sử hủy thành công!");

        }

        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }



        public (object data, string message) ShowAll(RequestTableShowAllDestroy requestTableShowAllDestroy)
        {
            var result = new List<object>();
            
           List<Destroy> destroys = _context.Destroys.Include(x => x.DestroyDetails).Where(x =>x.DestroyRequestDate >= requestTableShowAllDestroy.timeBeginDt && x.DestroyRequestDate <= requestTableShowAllDestroy.timeEndDt && (String.IsNullOrEmpty(requestTableShowAllDestroy.destroyCode) || x.DestroyCode.Contains(requestTableShowAllDestroy.destroyCode))).OrderBy(OrderValue(requestTableShowAllDestroy.sortField, requestTableShowAllDestroy.sortOrder)).ToList();
            foreach (Destroy destroy in destroys)
            {
                result.Add(new
                {
                    DestroyId = destroy.DestroyId,
                    DestroyCode = destroy.DestroyCode,
                    DestroyRequestDate = destroy.DestroyRequestDate,
                    DestroyDate = destroy.DestroyDate,
                    DestroyStatus = destroy.DestroyStatus,
                    DestroyUser = destroy.DestroyUser,
                    TotalDestroy = destroy.DestroyDetails.Sum(x => x.DestroyDetailQuantity),
                    TotalValue = destroy.DestroyDetails.Sum(x => x.ProductPirce * x.DestroyDetailQuantity)
                });
            }
            #region Paganation
            ResponseTable responseTable = new ResponseTable()
            {
                results = result.Skip((requestTableShowAllDestroy.page - 1) * requestTableShowAllDestroy.results).Take(requestTableShowAllDestroy.results).ToList(),
                info = new Info()
                {
                    page = requestTableShowAllDestroy.page,
                    totalRecord = result.Count,
                    results = requestTableShowAllDestroy.results
                }
            };
            #endregion
            return (responseTable, "Hiển thị thành công!");
        }

        public (Destroy destroy, string message) ShowDestroy(int destroyId)
        {
            Destroy destroy = _context.Destroys.Find(destroyId);
            if(destroy == null)
                return (null, "Lịch sử hủy không tồn tại!");
            return (destroy, "Hiển thị thành công!");
        }

        public (bool check,string mess) UpdateDestroyDate(int destroyId, DateTime? DestroyDate)
        {
            Destroy destroy = _context.Destroys.Find(destroyId);
            if (destroy == null)
                return (false, "Lịch sử hủy không tồn tại!");
            destroy.DestroyDate = DestroyDate;
            _context.SaveChanges();
            return (true, "Hiển thị thành công!");
        }

        public (Destroy destroy, string message) Store(Destroy destroy)
        {
            _context.Destroys.Add(destroy);
            _context.SaveChanges();
            return (destroy, "Tạo mới thành công!");
        }

        public (object result , string message) BaoCaoTonKhoTheoNgay(string search)
        {
            DateTime date = DateTime.UtcNow.AddHours(7);
            List<ReportInventory> reportInventories = new List<ReportInventory>();
            var groupInventories = _context.Inventories.Include(x => x.Product)
                .Where(x =>
                         (search == null || String.IsNullOrEmpty(search) || (search != null && x.ProductCode.Equals(search))|| (search != null && x.Product.ProductName.Contains(search)))
                    )
                .GroupBy(x=>x.ProductCode).ToList();
            reportInventories.AddRange(DataProcessing(groupInventories, date));
            return (reportInventories, "Xuất báo cáo thành công!");
        }

        public (object result, string message) BaoCaoTonKhoTheoNgay(RequestTable requestTable)
        {
            DateTime date = DateTime.UtcNow.AddHours(7);
            List<ReportInventory> reportInventories = new List<ReportInventory>();
            var groupInventories = _context.Inventories.Include(x => x.Product)
                .Where(x => 
                            (requestTable.search == null || String.IsNullOrEmpty(requestTable.search) || (requestTable.search != null && x.ProductCode.Equals(requestTable.search)))
                            || (requestTable.search == null || String.IsNullOrEmpty(requestTable.search) || (requestTable.search != null && x.Product.ProductName.Contains(requestTable.search)))
                    ).GroupBy(x => x.ProductCode).ToList();
            reportInventories.AddRange(DataProcessing(groupInventories, date));
            #region Paganation
            var result = reportInventories.AsQueryable().OrderBy(OrderValue(requestTable.sortField, requestTable.sortOrder)).ToList();
            ResponseTable responseTable = new ResponseTable()
            {
                results = result.Skip((requestTable.page - 1) * requestTable.results).Take(requestTable.results).ToList(),
                info = new Info()
                {
                    page = requestTable.page,
                    totalRecord = result.Count,
                    results = requestTable.results
                },
                total = new
                {
                    totalAmountInventory = reportInventories.Sum(x => x.amountInventory),
                    totalAmountExport = reportInventories.Sum(x => x.amountExport),
                    totalAmountDestroy = reportInventories.Sum(x => x.amountDestroy),
                    totalSum = reportInventories.Sum(x => x.sum)
                }
            };
            #endregion
            return (responseTable, "Xuất báo cáo thành công!");


        }
    
        public List<ReportInventory> DataProcessing(List<IGrouping<string,Inventory>> groupInventories, DateTime date)
        {
            List<ReportInventory> reportInventories = new List<ReportInventory>();
            foreach (var grInventory in groupInventories)
            {
                ReportInventory tmpReportInventory = new ReportInventory
                {
                    productCode = grInventory.Key,
                    productName = grInventory.FirstOrDefault().Product.ProductName,
                    productUnit = grInventory.FirstOrDefault().Product.ProductUnit,
                    amountInventory = grInventory.Sum(x => x.InQuantity),
                    amountExport = _context.SealDetails.Include(x => x.Seal).Where(x => x.ProductCode == grInventory.Key && x.Seal.Status == (int)StatusSeals.EXPORT).Sum(x => x.QuantityExport),
                    amountDestroy = _context.DestroyDetails.Include(x => x.Destroy).Where(x => x.ProductCode == grInventory.Key && x.Destroy.DestroyStatus == DestroyStatus.CONFIRMED).Sum(x => x.DestroyDetailQuantity),
                    createAt = date
                };
                // Remove amountDestroy theo yêu cầu chị tuyền . Commit : b882501b
                tmpReportInventory.sum = tmpReportInventory.amountInventory + tmpReportInventory.amountExport;
                reportInventories.Add(tmpReportInventory);
            }
            return reportInventories;
        }
    }
}
