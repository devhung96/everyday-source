using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Services
{
    public interface IDestroyDetailService
    {
        (List<DestroyDetail> destroyDetails, string message) StoreDestroyDetails(List<DestroyDetail> destroyDetails);
        (DestroyDetail destroyDetail, string message) DeleteDestroyDetail(int idDestroyDetail);

        (List<DestroyDetail> destroyDetails, string message) DeleteDestroyDetailByDestroyId(int idDestroy);
        (DestroyDetail destroyDetail, string message) ShowDestroyDetail(int idDestroyDetail);
        (List<DestroyDetail> destroyDetails, string message) ShowDestroyDetailByDestroy(int idDestroy);

        (DeClarationDetail deClarationDetail, string message) GetDeClarationDetail(string deNumber, string productCode);
        (DeClarationDetail deClarationDetail, string message) CheckDeClarationDetail(string deNumber, string productCode, int quantity);

        (List<DeClarationDetail> deClarationDetails, string message) showProductByDeclarationNumber(string deNumber);
    }
    public class DestroyDetailService : IDestroyDetailService
    {
        private readonly MariaDBContext _context;

        public DestroyDetailService(MariaDBContext context)
        {
            _context = context;
        }
        public (List<DestroyDetail> destroyDetails, string message) StoreDestroyDetails(List<DestroyDetail> destroyDetails)
        {
            _context.DestroyDetails.AddRange(destroyDetails);
            _context.SaveChanges();
            return (destroyDetails,"Thành công!");
        }

        public (DestroyDetail destroyDetail, string message) DeleteDestroyDetail(int idDestroyDetail)
        {
            DestroyDetail destroyDetail = _context.DestroyDetails.Find(idDestroyDetail);
            if (destroyDetail == null) return (null, "Không tìm thấy chi tiết hàng hủy!");
            _context.DestroyDetails.Remove(destroyDetail);
            _context.SaveChanges();
            return (destroyDetail, "Xóa chi tiết hàng hủy thành công!");
        }

        public (DestroyDetail destroyDetail, string message) ShowDestroyDetail(int idDestroyDetail)
        {
            DestroyDetail destroyDetail = _context.DestroyDetails.FirstOrDefault(x=> x.DestroyDetailId == idDestroyDetail);
            if (destroyDetail == null) return (null, "Không tìm thấy chi tiết hàng hủy!");
            return (destroyDetail, "Hiển thị chi tiết hàng hủy thành công!");
        }

        public (List<DestroyDetail> destroyDetails, string message) ShowDestroyDetailByDestroy(int idDestroy)
        {
            List<DestroyDetail> destroyDetails = _context.DestroyDetails.Where(x => x.DestroyId == idDestroy).Include(x =>x.Destroy).Include(x=>x.Product).ToList();
            destroyDetails.ForEach(x => x.ProductPirce = x.ProductPirce * x.DestroyDetailQuantity);
            return (destroyDetails,"Hiển thị thành công!");
        }

        public (DeClarationDetail deClarationDetail, string message) GetDeClarationDetail(string deNumber, string productCode)
        {
            DeClarationDetail deClarationDetail = _context.DeClarationDetails.Where(x => x.ProductCode == productCode && x.DeClaNumber == deNumber).Include(x => x.Product).Include(x => x.DeClaration).FirstOrDefault();
            if (deClarationDetail == null) return (null, "Phiếu nhập không tồn tại!");
            return (deClarationDetail, "Thành công!");
        }

        public (List<DestroyDetail> destroyDetails, string message) DeleteDestroyDetailByDestroyId(int idDestroy)
        {
            Destroy destroy = _context.Destroys.Find(idDestroy);
            if (destroy == null) return (null, "Không tìm thấy hàng hủy!");
            List<DestroyDetail> destroyDetails = _context.DestroyDetails.Where(x => x.DestroyId == idDestroy).ToList();
            _context.DestroyDetails.RemoveRange(destroyDetails);
            _context.SaveChanges();
            return (destroyDetails, "Thành công!");
        }

        public (List<DeClarationDetail> deClarationDetails, string message) showProductByDeclarationNumber(string deNumber)
        {
            List<DeClarationDetail> deClarationDetails = _context.DeClarationDetails.Where(x => x.DeClaNumber == deNumber).Include(x=>x.Product).ToList();
            return (deClarationDetails, "Thành công!");
        }

        public (DeClarationDetail deClarationDetail, string message) CheckDeClarationDetail(string deNumber, string productCode, int quantity)
        {
            DeClarationDetail deClarationDetail = _context.DeClarationDetails.Where(x => x.ProductCode == productCode && x.DeClaNumber == deNumber).Include(x => x.Product).Include(x => x.DeClaration).FirstOrDefault();
            if (deClarationDetail == null) return (null, $"Phiếu nhập : {deNumber} không tồn tại!");
            if(quantity >= deClarationDetail.DeClaDetailQuantity  ) return (null, $"Số lượng hủy phải nhỏ hơn bằng : {deClarationDetail.DeClaDetailQuantity} trong phiếu nhập!");
            return (deClarationDetail, "Thành công!");
        }
    }
}
