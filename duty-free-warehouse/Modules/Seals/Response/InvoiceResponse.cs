using Newtonsoft.Json;
using Project.Modules.Exchangerates.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class InvoiceResponse
    {
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string PassportNumber { get; set; }
        public string Nationality { get; set; }
        public string SeatNumber { get; set; }
        public int TotalSold { get; set; }
        public string FlightNo { get; set; }
        public string FlightNumberDetail { get; set; }
        public int? TypeInvoice { get; set; } = null;
        public List<ToTalSell> Total { get; set; }
        public IEnumerable<object> SellDetail { get; set; }
        public InvoiceResponse(Sell sell, Citypair citypair = null)
        {
            InvoiceId = sell.SellID;
            InvoiceNo = sell.InvoiceNo;
            CustomerName = sell.CustomerName;
            PassportNumber = sell.PassportNumber;
            Nationality = sell.Nationality;
            SeatNumber = sell.SeatNumber;
            TotalSold = sell.SellDetails.Sum(v => v.SoldNumber);
            FlightNo = sell.FlightNo;
            FlightNumberDetail = sell.FlightNumberDetail;
            Total = sell.ToTalSell;
            SellDetail = sell.SellDetails.Select(v => new
            {
                productCode = v.ProductCode,
                productName = v.Product.ProductName,
                soldNumber = v.SoldNumber,
                price = v.Price.HasValue ? v.Price.Value.ToString() : String.Empty,
                currency = String.IsNullOrEmpty(v.Currency) ? String.Empty : v.Currency
            });

            if(sell.TypeInvoice != null)
            {
                TypeInvoice = sell.TypeInvoice;
            }
            else if(citypair != null)
            {
                List<Schedule> schedules = JsonConvert.DeserializeObject<List<Schedule>>(citypair.Schedule);
                TypeInvoice = schedules.FirstOrDefault(x => x.flightNumber.Equals(FlightNumberDetail))?.typeSchedule;
            }
        }

    }
    public class ToTalSell
    {
        public double TotalExchangerate { get; set; }
        public string exchangerateCode { get; set; }
    }
}
