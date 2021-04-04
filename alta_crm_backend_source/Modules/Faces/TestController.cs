using FaceManagement;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using RepositoriesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Faces
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : BaseController
    {
        private readonly FaceManagementService.FaceManagementServiceClient _clientFaceManagement;
        private readonly RepositoryManagement.RepositoryManagementClient _clientReponsitory;

        public TestController(FaceManagementService.FaceManagementServiceClient clientFaceManagement, RepositoryManagement.RepositoryManagementClient clientReponsitory)
        {
            _clientFaceManagement = clientFaceManagement;
            _clientReponsitory = clientReponsitory;
        }

        [HttpGet]
        public IActionResult Show()
        {
            FaceRequest faceRequest = new FaceRequest
            {
                Repository = "8ccdc3af-480f-4e9b-b5a1-fcb7fc746a53",
                Bucket = "face-detect-kafka",
                ObjectName = "kafka-upload/637445611927651797_Ronaldinho_AP.jpg"
            };
            FaceResponse result = _clientFaceManagement.detect(faceRequest);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult Register()
        {
            FaceRequest faceRequest = new FaceRequest
            {
                Repository = "8ccdc3af-480f-4e9b-b5a1-fcb7fc746a53",
                Bucket = "face-detect-kafka",
                ObjectName = "kafka-upload/637445611927651797_Ronaldinho_AP.jpg",
                ExternalId  = "1"
            };
            FaceResponse result = _clientFaceManagement.register(faceRequest);
            return Ok(result);
        }


        [HttpGet("repository")]
        public IActionResult Repository()
        {
            var request = new RepositoryCreateRequest
            {
                Comment = "test",
                Name = "Test1",
                Type = "lele"
            };
            try
            {
                var result = _clientReponsitory.create(request);
                return Ok(result);
            }
            catch (RpcException exception)
            {
                return BadRequest(exception.StatusCode.ToString());
            }
           
        }

        [HttpGet("type")]
        public IActionResult TypeList()
        {
           
            var result = _clientReponsitory.list_type(new Google.Protobuf.WellKnownTypes.Empty());
            return Ok(result);
        }


        [HttpGet("list-reponsitory")]
        public IActionResult ListReponsitory()
        {

            var result = _clientReponsitory.list(new SearchRequest {
                PageSize = 100,
                PagingState = "",
                Query = ""
            });
            return Ok(result);
        }


        [HttpGet("create-reponsitory")]
        public IActionResult CreateReponsitory()
        {

            var result = _clientReponsitory.create(new RepositoryCreateRequest
            {
               Comment = "HDBank",
               Name = "HDBank",
               Type = "VIP"
            });
            return Ok(result);
        }


        [HttpGet("list-face/{reponsitoryId}")]
        public IActionResult ListFace(string reponsitoryId)
        {

            var result = _clientFaceManagement.list(new FaceListRequest
            {
               Repository = reponsitoryId,
               Query = "",
               PageNextState = "",
               PageSize = 100
            });
            return Ok(result);
        }


        [HttpDelete("list-face/{reponsitoryId}")]
        public IActionResult DeleteFace(string reponsitoryId)
        {
            List<object> resultDeleted = new List<object>();
            var result = _clientFaceManagement.list(new FaceListRequest
            {
                Repository = reponsitoryId,
                Query = "",
                PageNextState = "",
                PageSize = int.MaxValue
            });

            foreach (var item in result.Data)
            {
                var delete = _clientFaceManagement.remove(new FaceRemoveRequest { 
                    Id  = item.Id,
                    Repository = item.Repository
                });
                resultDeleted.Add(delete);
            }
            return Ok(resultDeleted);
        }

        [HttpDelete("ClearAllFace")]
        public IActionResult ClearAllFace()
        {

            List<object> resultClear = new List<object>();

            var repositories = _clientReponsitory.list(new SearchRequest
            {
                PageSize = 10000,
                PagingState = "",
                Query = ""
            });
            foreach (var item in repositories.Data)
            {
                var tmpFaceWithUser = _clientFaceManagement.list(new FaceListRequest
                {
                    PageSize = 100000,
                    Repository = item.Id
                });
                if (tmpFaceWithUser.Data.Count > 0)
                {
                    foreach (var face in tmpFaceWithUser.Data)
                    {

                        try
                        {
                            var resultDelete = _clientFaceManagement.remove_external_id(new FaceRemoveExternalRequest
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository
                            });
                            resultClear.Add(new
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository,
                                StatusCode = resultDelete.Status
                            });
                        }
                        catch (Exception ex)
                        {
                            resultClear.Add(new
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository,
                                StatusCode = 500,
                                Message = ex.Message + ex.StackTrace
                            });
                        }


                    }

                }

            }
            return ResponseOk(resultClear);


        }

        [HttpDelete("ClearFaceByUserId/{userId}")]
        public IActionResult ClearFaceByUserId(string userId)
        {

            List<object> resultClear = new List<object>();

            var repositories = _clientReponsitory.list(new SearchRequest
            {
                PageSize = 10000,
                PagingState = "",
                Query = ""
            });
            foreach (var item in repositories.Data)
            {
                var tmpFaceWithUser = _clientFaceManagement.list(new FaceListRequest
                {
                    PageSize = 100000,
                    Repository = item.Id
                });
                if (tmpFaceWithUser.Data.Count > 0)
                {
                    foreach (var face in tmpFaceWithUser.Data.Where(x => x.ExternalId.Equals(userId)))
                    {

                        try
                        {
                            var resultDelete = _clientFaceManagement.remove_external_id(new FaceRemoveExternalRequest
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository
                            });
                            resultClear.Add(new
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository,
                                StatusCode = resultDelete.Status
                            });
                        }
                        catch (Exception ex)
                        {
                            resultClear.Add(new
                            {
                                ExternalId = face.ExternalId,
                                Repository = face.Repository,
                                StatusCode = 500,
                                Message = ex.Message + ex.StackTrace
                            });
                        }


                    }

                }

            }




            return ResponseOk(resultClear);
        }

    }
}
