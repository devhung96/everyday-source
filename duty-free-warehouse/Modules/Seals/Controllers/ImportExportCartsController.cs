using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Requests;
using Project.Modules.Seals.Services;

namespace Project.Modules.Seals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportCartsController : BaseController
    {
        public readonly IImportExportCartService _importExportCartService;
        public ImportExportCartsController(IImportExportCartService importExportCartService)
        {
            _importExportCartService = importExportCartService;
        }

        private (bool, string, List<Seal>) FilterSealTable(FilterSealTableRequest request, List<Seal> data)
        {
            if (!string.IsNullOrEmpty(request.FlightDateTimeForm))
            {
                if (!DateTime.TryParseExact(request.FlightDateTimeForm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime flightDateForm))
                {
                    return (false, "Sai định dạng ngày!!!", new List<Seal>());
                }
                data = data.Where(s => s.FlightDate >= flightDateForm).ToList();
            }

            if (!string.IsNullOrEmpty(request.FlightDateTimeTo))
            {
                if (!DateTime.TryParseExact(request.FlightDateTimeTo, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime flightDateTo))
                {
                    return (false, "Sai định dạng ngày!!!", new List<Seal>());
                }
                data = data.Where(s => s.FlightDate <= flightDateTo).ToList();
            }

            if (!string.IsNullOrEmpty(request.ExportDateTimeForm))
            {
                if (!DateTime.TryParseExact(request.ExportDateTimeForm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ExportDateForm))
                {
                    return (false, "Sai định dạng ngày!!!", new List<Seal>());
                }
                data = data.Where(s => s.ImportDate.HasValue && s.ImportDate.GetValueOrDefault().Date >= ExportDateForm.Date).ToList();
            }

            if (!string.IsNullOrEmpty(request.ExportDateTimeTo))
            {
                if (!DateTime.TryParseExact(request.ExportDateTimeTo, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ExportDateTimeTo))
                {
                    return (false, "Sai định dạng ngày!!!", new List<Seal>());
                }
                data = data.Where(s => s.ImportDate.HasValue && s.ImportDate.GetValueOrDefault().Date <= ExportDateTimeTo.Date).ToList();
            }

            if (request.StatusSeal != null)
            {
                data = data.Where(s => s.Status == request.StatusSeal.Value).ToList();
            }
            return (true, "Success", data);
        }

        [HttpPost]
        public IActionResult ShowAll([FromBody] FilterSealTableRequest request)
        {
            (List<Seal> data, string message) = _importExportCartService.ShowAll();
            double lastePage = 0;
            
            data = data.Where(s => string.IsNullOrEmpty(request.Search) || 
                    s.FlightNumber.ToUpper().Contains(request.Search.ToUpper()) 
                    || s.SealNumber.ToUpper().Contains(request.Search.ToUpper())
                ).ToList();

            (bool check, string messageFilter, List<Seal> dataFiler) = FilterSealTable(request, data);
            if (!check)
            {
                return ResponseBadRequest(messageFilter);
            }
                
            if (request.PerPage == 0 || request.Page == 0)
            {
                return ResponseOk(dataFiler, message);
            }

            DataPaginationResponse paginations = new DataPaginationResponse
            {
                perPage = request.PerPage,
                page = request.Page,
                total = dataFiler.Count
            };

            lastePage = paginations.total / paginations.perPage;
            paginations.lastePage = Math.Ceiling(lastePage);
            if(paginations.lastePage < paginations.page)
            {
                paginations.page = paginations.lastePage;
            }
                
            if(paginations.page <= 0)
            {
                paginations.page = 1;
            }

            double skip = (paginations.page - 1) * paginations.perPage;
            paginations.data = dataFiler
                .Skip((int)Math.Ceiling(skip))
                .Take((int)paginations.perPage)
                .ToArray();

            return Ok(paginations);
        }
        [HttpGet("{sealID}")]
        public IActionResult Detail(string sealID)
        {
            (Seal data, string message) = _importExportCartService.Detail(sealID);
            return ResponseOk(data,message);
        }

        private static readonly List<string> seals = new List<string>();
        [HttpPost("status/{sealID}")]
        public IActionResult UpdateStatus(string sealID, [FromBody] UpdateStatusRequest request)
        {
            if(request.Date is null)
            {
                request.Date = DateTime.Now;
            }
                
            if(seals.FirstOrDefault(x => x.Equals(sealID)) != null)
            {
                return ResponseBadRequest("Thao tác đang được thực hiện, vui lòng chờ");
            }
            seals.Add(sealID);
            (Seal data, string message) = _importExportCartService.ChangeStatus(sealID, request.Date.Value);
            seals.Remove(sealID);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("quantity/{sealDetailID}")]
        public IActionResult UpdateQuantity(int sealDetailID, UpdateQuatityrealRequest valueUpdate)
        {
            (SealDetail data, string message) = _importExportCartService.Updatequantityreal(sealDetailID,valueUpdate);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}