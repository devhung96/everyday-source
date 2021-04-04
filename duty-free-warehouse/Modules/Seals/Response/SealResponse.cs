using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class SealResponse
    {
        public string SealNumber { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightDate { get; set; }
        public string AcReg { get; set; }
        public int Status { get; set; }
        public string Route { get; set; }
        public string Return { get; set; }
        
        public SealResponse()
        {

        }
        public SealResponse(Seal seal)
        {
            SealNumber = seal.SealNumber;
            FlightNumber = seal.FlightNumber;
            FlightDate = seal.FlightDate;
            AcReg = seal.AcReg;
            Status = seal.Status;
            Return = seal.Return;
            Route = seal.Route;
        }

        public SealResponse(SealImportRequest sealImportRequest)
        {
            AcReg = sealImportRequest.AcReg;
            FlightDate = sealImportRequest.FlightDate;
            FlightNumber = sealImportRequest.FlightNumber;
            Return = sealImportRequest.SealNumberReturn;
            SealNumber = sealImportRequest.SealNumber;
            Route = sealImportRequest.Route;
            Status = 0;
        }
    }
}
