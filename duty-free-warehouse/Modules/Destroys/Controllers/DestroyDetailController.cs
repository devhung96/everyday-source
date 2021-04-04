using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Declarations.Services;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using Project.Modules.Destroys.Requests;
using Project.Modules.Destroys.Services;

namespace Project.Modules.Destroys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestroyDetailController : BaseController
    {
        private readonly IDestroyDetailService _destroyDetailService;
        private readonly IDestroyService _destroyService;
        private readonly IDeClarationServices _deClarationServices;
        public DestroyDetailController(IDestroyDetailService destroyDetailService, IDestroyService destroyService, IDeClarationServices deClarationServices)
        {
            _destroyDetailService = destroyDetailService;
            _destroyService = destroyService;
            _deClarationServices = deClarationServices;
        }


        [HttpPost]
        [Authorize]
        public IActionResult storeDestroyDetail([FromBody] StoreDestroyDetailRequest  request)
        {
            DateTime? dateTimeDt = null;
            if(!String.IsNullOrEmpty(request.DestroyDate))
            {
                dateTimeDt = DateTime.ParseExact(request.DestroyDate.ToString(), "dd/MM/yyyy", null);
            }
            string userName = User.FindFirst("UserName").Value;
            Destroy newDestroy = new Destroy
            {
                DestroyRequestDate = DateTime.UtcNow.AddHours(7),
                DestroyUser = userName,
                DestroyCode = request.DestroyCode,
                DestroyDate = dateTimeDt
            };
            (Destroy destroy, string messageDestroy) = _destroyService.Store(newDestroy);
            List<DestroyDetail> destroyDetails = new List<DestroyDetail>();
            foreach (ChildDestroyDetail childDestroyDetail in request.ChildDestroyDetails)
            {
                (DeClarationDetail deClarationDetail, string messageDeClarationDetail) = _destroyDetailService.CheckDeClarationDetail(childDestroyDetail.DeNumber, childDestroyDetail.ProductCode, childDestroyDetail.DestroyDetailQuantity);
                if(deClarationDetail == null)
                {
                    (Destroy deleteDestroy, _) = _destroyService.Delete(destroy.DestroyId);
                    return ResponseBadRequest(messageDeClarationDetail);
                }
                destroyDetails.Add(new DestroyDetail
                {
                    DeClaNumber = childDestroyDetail.DeNumber,
                    DestroyDetailNote = childDestroyDetail.DestroyDetailNote,
                    DestroyDetailQuantity = childDestroyDetail.DestroyDetailQuantity,
                    ProductCode = childDestroyDetail.ProductCode,
                    DestroyId = destroy.DestroyId,
                    ProductPirce = deClarationDetail.DeClaDetailInvoicePrice
                });

            }
            (List<DestroyDetail> data, string message) = _destroyDetailService.StoreDestroyDetails(destroyDetails);
            return ResponseOk(data, message);
        }

        [HttpPost("update")]
        public IActionResult updateDestroyDetail([FromBody] UpdateDestroyDetailRequest request)
        {
            (Destroy destroy, string messageDestroy) = _destroyService.ShowDestroy(request.DestroyId);
            if (destroy == null) return ResponseBadRequest(messageDestroy);
            DateTime? dateTimeDt = null;
            if (!String.IsNullOrEmpty(request.DestroyDate))
            {
                dateTimeDt = DateTime.ParseExact(request.DestroyDate.ToString(), "dd/MM/yyyy", null);
            }
            var updateDestroyDate = _destroyService.UpdateDestroyDate(request.DestroyId, dateTimeDt);
            if(!updateDestroyDate.check)
            {
                return ResponseBadRequest(updateDestroyDate.mess);
            }
            (List<DestroyDetail> destroyDetailsDelete, string messageDelete) = _destroyDetailService.DeleteDestroyDetailByDestroyId(request.DestroyId);
            List<DestroyDetail> destroyDetails = new List<DestroyDetail>();
            foreach (ChildDestroyDetail childDestroyDetail in request.ChildDestroyDetails)
            {
                (DeClarationDetail deClarationDetail, string messageDeClarationDetail) = _destroyDetailService.CheckDeClarationDetail(childDestroyDetail.DeNumber, childDestroyDetail.ProductCode, childDestroyDetail.DestroyDetailQuantity);
                if (deClarationDetail == null) return ResponseBadRequest(messageDeClarationDetail);
                destroyDetails.Add(new DestroyDetail
                {
                    DeClaNumber = childDestroyDetail.DeNumber,
                    DestroyDetailNote = childDestroyDetail.DestroyDetailNote,
                    DestroyDetailQuantity = childDestroyDetail.DestroyDetailQuantity,
                    ProductCode = childDestroyDetail.ProductCode,
                    DestroyId = request.DestroyId,
                    ProductPirce = deClarationDetail.DeClaDetailInvoicePrice
                });
            }
            (List<DestroyDetail> data, string message) = _destroyDetailService.StoreDestroyDetails(destroyDetails);
            return ResponseOk(data, message);
        }

        [HttpDelete("{idDestroyDetail}")]
        public IActionResult deleteDestroyDetail(int idDestroyDetail)
        {
            (DestroyDetail checkDestroyDetail, string messageCheckDestroyDetail) = _destroyDetailService.ShowDestroyDetail(idDestroyDetail);
            if (checkDestroyDetail == null) return ResponseBadRequest(messageCheckDestroyDetail);
            (DestroyDetail result, string message) = _destroyDetailService.DeleteDestroyDetail(idDestroyDetail);
            return ResponseOk(result, message);
        }

        [HttpGet("{idDestroy}")]
        public IActionResult showByDestroyId(int idDestroy)
        {
            (Destroy destroy, string messageDestroy) = _destroyService.ShowDestroy(idDestroy);
            if (destroy == null) return ResponseBadRequest(messageDestroy);
            (List<DestroyDetail> destroyDetails, string message) = _destroyDetailService.ShowDestroyDetailByDestroy(idDestroy);
            return ResponseOk(destroyDetails, message);
        }


        [HttpGet("infor/{productCode}/{deNumber}")]
        public IActionResult showInforDestroy(string productCode, string deNumber)
        {
            (DeClarationDetail deClarationDetail, string messageDeClarationDetail) = _destroyDetailService.GetDeClarationDetail(deNumber, productCode);
            if (deClarationDetail == null)
                return ResponseBadRequest(messageDeClarationDetail);
            return ResponseOk(deClarationDetail);
        }


        [HttpGet("all-declaration")]
        public IActionResult showAllDeclaration()
        {
            object result = _deClarationServices.ShowAll();
            return ResponseOk(result);
        }

        [HttpGet("show-product-by-declaration/{deNumber}")]
        public IActionResult showProductByDeclarationNumber(string deNumber)
        {
            (List<DeClarationDetail> deClarationDetails, string message) = _destroyDetailService.showProductByDeclarationNumber(deNumber);
            return ResponseOk(deClarationDetails, message);
        }






    }
}