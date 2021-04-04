using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class TrackLiquidity
    {
        public string DeclarationNumber { get; set; }
        public string DeclaRegister { get; set; }
        public string DeClaExtendedDispatch { get; set; }
        public string DeClaSettlementDispatch { get; set; }
        public DateTime? DeClaDateSettlementDispatch { get; set; }
        public int DeClaSettlementStatus { get; set; }
        public List<ChildData> ChildData { get; set; }
        public int TotalImportQuantity { get; set; }
        public int TotalExportQuantity { get; set; }
        public int TotalInventoryQuantity { get; set; }
        public int TotalProductquantity { get; set; }
        public object DeClaration { get; set; }
        public int TotalChildData { get; set; }
        public int Color { get; set; }
        public string NewDate { get; set; }
        public int OrderBy
        {
            get
            {
                if (DeClaSettlementStatus == 3)
                {
                    return 4;
                }
                else if (DeClaSettlementStatus == 2)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
        }

        public DateTime? ThenOrderBy
        {
            get
            {
                if (OrderBy == 4 || OrderBy == 3)
                {
                    return _thenOrderBy;
                }
                else
                {
                    return null;
                }

            }
            set
            {

            }
        }
        public DateTime? _thenOrderBy { get; set; }
    }
    public class ChildData
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public int ImportQuantity { get; set; }
        public int ExportQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int DestroyQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public int InventoryQuantity { get; set; }
        public DateTime? SettlementDate { get; set; }
    }

}
