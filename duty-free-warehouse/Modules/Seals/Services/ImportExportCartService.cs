using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.Modules.Inventories.Services;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Project.Modules.Seals.Services
{
    public interface IImportExportCartService
    {
        (List<Seal> data, string message) ShowAll();
        (Seal data, string message) Detail(string sealID);
        (Seal data, string message) FindID(string sealID);
        (Seal data, string message) ChangeStatus(string sealID, DateTime date);
        (SealDetail data, string message) Updatequantityreal(int sealDetailID, UpdateQuatityrealRequest valueUpdate);
    }
    public class ImportExportCartService : IImportExportCartService
    {
        public readonly MariaDBContext _mariaDBContext;
        public readonly IInventoryService _inventoryService;
        public readonly ISealImportService _isealImportService;
        public ImportExportCartService(MariaDBContext mariaDBContext,IInventoryService inventoryService, ISealImportService isealImportService)
        {
            _mariaDBContext = mariaDBContext;
            _inventoryService = inventoryService;
            _isealImportService = isealImportService;
        }

        public (Seal data, string message) Detail(string sealID)
        {
            (Seal val, _) = FindID(sealID);
            return (val, "Hiển chi tiết hàng hóa theo Seal thành công");
        }

        public (Seal data, string message) FindID(string sealID)
        {
            Seal _value = _mariaDBContext.Seals
                           .Include(x => x.SealDetails)
                           .ThenInclude(x => x.Product )
                           .Include(x => x.SealDetails)
                           .ThenInclude(x => x.DeClaration)
                           .Where(x => x.SealNumber.Equals(sealID)).FirstOrDefault();
            if (_value == null)
                return (null, "Không tìm thấy số seal!!!");
            return (_value, "Hiển chi tiết hàng hóa theo Seal thành công!!!");
        }

        public (List<Seal> data, string message) ShowAll()
        {
            List<Seal> _values = _mariaDBContext.Seals.OrderByDescending(x => x.FlightDate).ToList();
            foreach (var _value in _values)
            {
                int exportQuantity = _mariaDBContext.SealDetails.Where(x => x.SealNumber == _value.SealNumber).Sum(s => s.QuantityExport);
                int QuantitySell = _mariaDBContext.SealDetails.Where(x => x.SealNumber == _value.SealNumber).Sum(s => s.QuantitySell);
                _value.ExportQuantity = exportQuantity;
                _value.QuantitySell = QuantitySell;
            }
            return (_values, "Hiển thị thành công quản lý điều chuyển!!!");
        }

        public (Seal data, string message) ChangeStatus(string sealID, DateTime date)
        {
            (Seal seal, string message) = FindID(sealID);
            if (seal == null)
                return (null,message);
            if (seal.Status == (int)StatusSeals.NEW)
            {
                (object check, string mess) = _isealImportService.CheckXuatKho(seal.SealNumber);
                if (check == null)
                    return (null, mess);
                seal.Status = (int)StatusSeals.EXPORT;
                seal.ExportDate = date;
                _mariaDBContext.SaveChanges();
                foreach (var SealDetail in seal.SealDetails)
                {
                   _inventoryService.Remmove(SealDetail.DeClaNumber, SealDetail.QuantityExport, SealDetail.ProductCode);
                   
                }
                return (seal, "Cập nhật trạng thái thành công !!!");
            }  
            if (seal.Status == (int)StatusSeals.EXPORT)
            {
                seal.Status = (int)StatusSeals.SELL;
                seal.ImportDate = date;
                _mariaDBContext.SaveChanges();
                foreach (var SealDetail in seal.SealDetails)
                {
                    if (seal.Status == (int)StatusSeals.SELL)
                    {
                        _inventoryService.Add(SealDetail.DeClaNumber, SealDetail.QuantityReal, SealDetail.ProductCode);
                    }
                }
                return (seal, "Cập nhật trạng thái thành công !!!");
            }
            return (seal, "Cập nhật trạng thái thành công !!!");
        }

        public (SealDetail data, string message) Updatequantityreal(int sealDetailID, UpdateQuatityrealRequest valueUpdate)
        {
            SealDetail value = _mariaDBContext.SealDetails.Find(sealDetailID);
            if (value == null)
                return (null, "Không tìm thấy chi tiết hàng hóa");
            value.QuantityReal = valueUpdate.quantityReal;
            _mariaDBContext.SaveChanges();
            return (value,"Cập nhật thành công số lượng thực tế !!!");
        }
    }
}
