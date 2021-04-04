using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    public class DataPaginationResponse
    {
        public double total { get; set; }
        public double perPage { get; set; }
        public double lastePage { get; set; }
        public double page { get; set; }
        public object[] data { get; set; }
        public int TongTonDauKy { get; set; }
        public int TongNhapTrongKy { get; set; }
        public int TongXuatTrongKy { get; set; }
        public int TongTonCuoiKy { get; set; }
        
    }
    public class ResponseReport
    {
        public object[] data { get; set; }
        public int TongTonDauKy { get; set; }
        public int TongNhapTrongKy { get; set; }
        public int TongXuatTrongKy { get; set; }
        public int TongTonCuoiKy { get; set; }
    }
}
