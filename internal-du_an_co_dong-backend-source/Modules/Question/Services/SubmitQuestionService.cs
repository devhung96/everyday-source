using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Services.Common;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.App.Helpers;
using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Services;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Entities;
using Project.Modules.Question.Entities;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Project.Modules.Reports.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.Modules.Question.Services
{
    public interface ISubmitQuestionService
    {
        (object result, string message) SubmitSingleQuestion(SubmitQuestion submitQuestion, string token);
        (object result, string message) GetAllResultOrganization(string organizationID);
        (object result, string message) GetUserRevote(ListRevote listRevote);
        (object result, string message) FlagSubmitAll(FlagSubmitAll flag, string token);
    }
    public class SubmitQuestionService : ISubmitQuestionService
    {
        public readonly MariaDBContext _dbContext;
        public readonly MongoDBContext _mongo;
        public readonly IConfiguration _config;
        public readonly IAuthorityService _author;
        public readonly IQuestionAnswersService _questionServices;
        public readonly IReportService _reportService;
        public readonly ISoketIO _soketIO;
        public SubmitQuestionService(MariaDBContext dbContext,MongoDBContext mongo,IConfiguration config, IQuestionAnswersService questionServices, IReportService reportService, IAuthorityService author, ISoketIO soketIO)
        {
            _dbContext = dbContext;
            _mongo = mongo;
            _config = config;
            _questionServices = questionServices;
            _reportService = reportService;
            _author = author;
            _soketIO = soketIO;
        }
        public List<Authority> GetNodeBinaryBauDon(List<Authority> authorities, List<Authority> response,string questionID)
        {
            foreach (Authority item in authorities)
            {
                var NguoiCungUyQuyen = _dbContext.Authorities.Where(x => x.EventID == item.EventID
                                                                && x.AuthorityReceiveUserID == item.AuthorityUserID
                                                                && (x.QuestionID == questionID
                                                                )
                                                            ).ToList();
                if (NguoiCungUyQuyen.Count != 0)
                {
                    response.AddRange(NguoiCungUyQuyen);
                    GetNodeBinaryBauDon(NguoiCungUyQuyen, response, questionID);
                }
            }
            return (response);
        }
        public List<Authority> GetNodeBinaryUyQuyen(List<Authority> authorities, List<Authority> response, string questionID)
        {
            foreach (Authority item in authorities)
            {
                var NguoiCungUyQuyen = _dbContext.Authorities.Where(x => x.EventID == item.EventID
                                                                && x.AuthorityReceiveUserID == item.AuthorityUserID
                                                                && x.AuthorityType == AuthorityType.EVENT
                                                            ).ToList();
                if (NguoiCungUyQuyen.Count != 0)
                {
                    response.AddRange(NguoiCungUyQuyen);
                    GetNodeBinaryUyQuyen(NguoiCungUyQuyen, response, questionID);
                }
            }
            return (response);
        }

        public (object result,string message) FlagSubmitAll(FlagSubmitAll flag, string token)
        {
            Console.WriteLine("START:" + DateTime.Now);
            int SubmitBeforeEvent = 0;
            #region Check Event Id
            flag.AppId = _config["SurveyQuestion:AppId"];
            var Event = _dbContext.Events.FirstOrDefault(x => x.EventId == flag.EventId);
            if (Event is null)
            {
                return (null, "EventNotExists");
            }
            #endregion

            #region Get Question Mongo
            (string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{flag.AppId}/{flag.QuestionId}", null, null).Result;
             
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }

            var result = JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);

            var listQuestion = JsonConvert.DeserializeObject<List<ResponseQuestion>>(result.Data.ToString());
            if (listQuestion.Count == 0)
            {
                return (null, "QuestionNotFound");
            }
            var questions = listQuestion.FirstOrDefault();
            #endregion

            #region Get List User Chưa trả lời
            var listIntExcludeUserId = new List<int>();
            var listUserEvent = new List<EventUser>();
            
            if (Event.EventFlag == Events.Entities.EVENT_FLAG.CREATED)
            {
                 listUserEvent = _dbContext.EventUsers.Where(x => x.EventId == flag.EventId ).ToList();
                SubmitBeforeEvent = 1;
            }
            else
            {
                listUserEvent = _dbContext.EventUsers.Where(x => x.EventId == flag.EventId && x.UserLatch == USER_LATCH.ON ).ToList();
            }
            foreach (EventUser eventUser in listUserEvent)
            {
                var listAuthor = _dbContext.Authorities
                    .Where(x => 
                    x.EventID == flag.EventId && 
                    (x.QuestionID == flag.QuestionId || x.AuthorityType == AuthorityType.EVENT) && 
                    x.AuthorityUserID == eventUser.UserId
                    ).FirstOrDefault();
                
                if (listAuthor != null)
                {
                    listIntExcludeUserId.Add(eventUser.EventUserId);
                }
            }

            #region Get All Result
            GetResultSubmit getResultSubmit = new GetResultSubmit()
            {
                AppId = flag.AppId,
                QuestionID = flag.QuestionId
            };
            List<ResponseResultSubmit> listResultTruyenQuaSubmit = new List<ResponseResultSubmit>();


            (object resultSubmit, string messageSubmit) = _questionServices.GetResultSubmitVoid(getResultSubmit);
            if (resultSubmit != null)
            {
                var listResult = (List<ResponseResultSubmit>)resultSubmit;
                //loc de truyen qua submit2
                listResultTruyenQuaSubmit = listResult;

                if (listResult.Count != 0)
                {
                    foreach (var item in listUserEvent)
                    {
                        if (listResult.Any(x => x.ContentJson.UserID == item.UserId))
                        {
                            listIntExcludeUserId.Add(item.EventUserId);
                        }
                    }
                }
            }

            #endregion

            #region Lấy danh sách những người vận hành có cổ phiếu bằng 0
            //var listEventUserNoStock = _dbContext.EventUsers.Where(x => x.EventId == flag.EventId && )
            #endregion
            listUserEvent = listUserEvent.Where(z => !listIntExcludeUserId.Contains(z.EventUserId)).ToList();
            #endregion

            #region Tạo câu trả lời cho những đại ca không trả lời
            int flagDapAnMacDinh = 0;

            List<Selection> answerAuto = new List<Selection>();  

            Feedback feedback1 = new Feedback();
            if (questions.AnswersJson.Count > 0)
            {
                var listAnswerCustome = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Selection>>(questions.Answers);
                int i = 0;
                foreach (var answer in listAnswerCustome)
                {
                    if (answer.Bingo)
                    {
                        listAnswerCustome[i].Selected = true;
                        listAnswerCustome[i].Auto = true;
                        flagDapAnMacDinh = 1;
                    }
                    i++;
                }
                answerAuto = listAnswerCustome;
            }
            if (questions.FeedbackJson != null)
            {
                var feedback = Newtonsoft.Json.JsonConvert.DeserializeObject<Feedback>(questions.FeedBack);
                if (feedback.Bingo)
                {
                    feedback.Selected = true;
                    feedback.Auto = true;
                    flagDapAnMacDinh = 1;
                }
                feedback1 = feedback;
            }
            if (flagDapAnMacDinh == 1)
            {
                List<RequestSubmit> listRequestSubmit = new List<RequestSubmit>();
                List<Authority> DanhSachBauDon = _dbContext.Authorities
                            .Where(x => x.EventID == flag.EventId
                            ).ToList();


                SubmitQuestion submitQuestion = new SubmitQuestion()
                {
                    AppId = flag.AppId,
                    Answers = answerAuto,
                    AnotherAnswer = questions.AnotherAnswer != null ? questions.AnotherAnswerJson : null,
                    EventID = flag.EventId,
                    Feedback = questions.FeedbackJson != null ? feedback1 : null,
                    QuestionID = questions.QuestionID,
                  //  UserID = item.UserId,
                    Flag = 1,//cờ này để api biết là máy chọn. không liên quan đến auto
                    FlagBeforeEvent = SubmitBeforeEvent
                };
                foreach (var item in listUserEvent)
                {
                    submitQuestion.UserID = item.UserId;
                    (RequestSubmit resultSubmitSingleQuestion2, string messageSubmitsingleQuestion2) = SubmitSingleQuestion2(submitQuestion, null, listQuestion, listResultTruyenQuaSubmit, DanhSachBauDon);

                    if (resultSubmitSingleQuestion2 != null)
                    {
                        //listRequestSubmit.Add(resultSubmitSingleQuestion2);
                        listRequestSubmit.Add(new RequestSubmit
                        {
                            AppId = resultSubmitSingleQuestion2.AppId,
                            Content = JsonConvert.SerializeObject(resultSubmitSingleQuestion2.Content),
                            Flag = resultSubmitSingleQuestion2.Flag,
                            QuestionID = resultSubmitSingleQuestion2.QuestionID,
                            ResultID = resultSubmitSingleQuestion2.ResultID
                        });

                    }

                }
                listRequestSubmit.ForEach(x => x.Content = JsonConvert.DeserializeObject<SubmitQuestion>(x.Content.ToString()));
                Console.WriteLine("END:" + DateTime.Now);
                //goi 1 lan qua kia
                (string data, int? statusCode) resultSubmit2 = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + "/submit-multi", JsonConvert.SerializeObject(listRequestSubmit), null).Result;

                if (resultSubmit2.statusCode != (int)HttpStatusCode.OK)
                {
                    return (null, resultSubmit2.data);
                }
            }
            #endregion

            #region Socket dev hung
            if (token != null)
            {
                if (questions.OptionJson.Any(x => int.Parse(x.ToString()) == (int)OptionQuestion.SEE_RESULTS_RIGHT_AWAY))
                {
                   _reportService.SubmitSocketChartAsync(flag.EventId, flag.QuestionId, token, OptionQuestion.SEE_RESULTS_RIGHT_AWAY);
                }
                else
                {
                    _reportService.SubmitSocketChartAsync(flag.EventId, flag.QuestionId, token, null);
                }
            }
            #endregion
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("END:" + DateTime.Now);
            return ("Thành công.", "SubmitDefaultSuccess");
        }
        //Submit single question
        public (object result, string message) SubmitSingleQuestion(SubmitQuestion submitQuestion,string token)

        {
            #region Check Event Id
            submitQuestion.AppId = _config["SurveyQuestion:AppId"];
            var Event = _dbContext.Events.FirstOrDefault(x => x.EventId == submitQuestion.EventID);
            if (Event is null)
            {
                return (null, "EventNotExists");
            }
            if (Event.EventFlag == Events.Entities.EVENT_FLAG.CREATED)
            {
                submitQuestion.FlagBeforeEvent = 1;
            }
            #endregion

            #region Get All Question Mongo
            (string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{submitQuestion.AppId}", null, null).Result;

            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);

            var listQuestion = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseQuestion>>(result.Data.ToString());
            if (listQuestion.Count == 0)
            {
                return (null, "Không tồn tại câu hỏi trong sự kiện này.");
            }

            //get All List question mongo
            var questions = listQuestion.Where(x => x.QuestionID == submitQuestion.QuestionID).FirstOrDefault();
            if (questions is null)
            {
                return (null, "QuestionNotFound");
            }

            //get User Submit
            var userSubmit = _dbContext.EventUsers
                .FirstOrDefault(x => 
                x.UserId == submitQuestion.UserID && 
                x.EventId == submitQuestion.EventID );
            if (userSubmit is null)
            {
                return (null, "UserEventNotFound");
            }
            
            if (userSubmit.UserLatch == USER_LATCH.OFF && Event.EventFlag == Events.Entities.EVENT_FLAG.BEGIN)
            {
                return (null, "UserNotClose");
            }
            //if (userSubmit.UserLoginStatus == USER_LOGIN_STATUS.OFF && Event.EventFlag == Events.Entities.EVENT_FLAG.CREATED)
            //{
            //    return (null, "Tài khoản chưa đăng nhập.");
            //}
            //if (userSubmit.UserLatch == USER_LATCH.OFF && Event.EventFlag == Events.Entities.EVENT_FLAG.BEGIN)
            //{
            //    return (null, "Tài Khoản Không Có Quyền Trả Lời Câu Hỏi Này.");
            //}

            #region Check Type Question and Answers

            if (questions.Type == TypeQuestion.CHOOSE)
            {
                if (submitQuestion.Answers is null)
                {
                    Console.WriteLine("Single choose không đúng dịnh dạng array hoặc rỗng.");
                    return (null, "Đáp án không được bỏ trống.");
                }
            }
            if (questions.Type == TypeQuestion.MULTICHOOSE)
            {
                if (submitQuestion.Answers is null)
                {
                    Console.WriteLine("Multi choose không đúng dịnh dạng array hoặc rỗng.");
                    return (null, "Đáp án không được bỏ trống.");
                }
            }
            if (questions.Type == TypeQuestion.QA)
            {
                if (submitQuestion.Feedback is null)
                {
                    Console.WriteLine("QA không đúng dịnh dạng array hoặc rỗng.");
                    return (null, "Hỏi đáp không được bỏ trống.");
                }
            }
            #region Rating and scale
            //if (questions.Type == TypeQuestion.RATING)
            //{
            //    if (submitQuestion.AnotherAnswer != null)
            //    {
            //        var ratingCheck = Newtonsoft.Json.JsonConvert.DeserializeObject<Rating>(submitQuestion.AnotherAnswer.ToString());
            //        if (ratingCheck.ImageIcon is null || ratingCheck.NoStep is null)
            //        {
            //            return (null, "Content is required with type !!");
            //        }
            //    }
            //    else
            //    {
            //        return (null, "Content is required with type !!");
            //    }
            //}
            //if (questions.Type == TypeQuestion.SCALE)
            //{
            //    if (submitQuestion.AnotherAnswer != null)
            //    {
            //        var scale = Newtonsoft.Json.JsonConvert.DeserializeObject<NumricScale>(submitQuestion.AnotherAnswer.ToString());
            //        if (scale.IconMax is null || scale.IconMin is null || scale.Max is null || scale.Min is null)
            //        {
            //            return (null, "Content is required with type !!");
            //        }
            //    }
            //    else
            //    {
            //        return (null, "Content is required with type !!");
            //    }
            //}
            #endregion
            if (submitQuestion.Answers is null)
            {
                submitQuestion.Answers = new List<Selection>();
            }

            #endregion
            #endregion

            #region Check CountAnswers
            //var CountAnswers = questions.CountAnswers;
            //questions.CountAnswers = null;
            var CountAnswers = questions.CountAnswers;
            int checkCountAnswer = 0;

            if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
            {
                checkCountAnswer++;
            }
            int stockAnswer = 0;
            #region Check Cổ phiếu trên giao diện truyền vào
            //if (questions.Type != TypeQuestion.QA)
            //{
            if (submitQuestion.Flag == 0)
            {
                if (submitQuestion.Answers.Count > 0)
                {
                    foreach (var item in submitQuestion.Answers)
                    {
                        if (item.Selected == true)
                        {
                            if (item.Stock != null)
                            {
                                stockAnswer += item.Stock.Value;
                            }
                        }
                    }
                }
                if (submitQuestion.Feedback != null)
                {
                    stockAnswer += submitQuestion.Feedback.Stock.Value;
                }
            }
            //}
            #endregion

            var listAnswerSelected = submitQuestion.Answers.Where(x => x.Selected == true).ToList().Count;
            checkCountAnswer += listAnswerSelected;
            Rating rating = new Rating();
            NumricScale numricScale = new NumricScale();
            if (submitQuestion.AnotherAnswer != null)
            {
                if (questions.Type == TypeQuestion.RATING)
                {
                    rating = JsonConvert.DeserializeObject<Rating>(submitQuestion.AnotherAnswer.ToString());
                    if (rating.Selected == true)
                    {
                        checkCountAnswer++;
                    }
                }
                if (questions.Type == TypeQuestion.SCALE)
                {
                    numricScale = JsonConvert.DeserializeObject<NumricScale>(submitQuestion.AnotherAnswer.ToString());
                    if (numricScale.Selected == true)
                    {
                        checkCountAnswer++;
                    }
                }
            }
            int value;
            if (CountAnswers != null && int.TryParse(CountAnswers.ToString(), out value))
            {
                if (CountAnswers < checkCountAnswer)
                {
                    return (null, "SelectedError");
                }
            }


            #endregion

            #region Check Đã Ủy Quyền nhưng vẫn muốn trả lời
            var checkAuthorAnswer = _dbContext.Authorities
                .Where(x => x.AuthorityUserID == submitQuestion.UserID
                && x.EventID == submitQuestion.EventID
                && (x.QuestionID == submitQuestion.QuestionID
                    || x.AuthorityType == AuthorityType.EVENT)
                ).FirstOrDefault();
            if (checkAuthorAnswer != null)
            {
                return (null, "OverCloseAnswers");
            }
            #endregion

            #region Get All Value Tree Node
            int stockTotal = 0;
            var DanhSachBauDon = _dbContext.Authorities
                            .Where(x => x.EventID == submitQuestion.EventID
                            && x.AuthorityReceiveUserID == submitQuestion.UserID
                            && x.QuestionID == submitQuestion.QuestionID
                            ).ToList();
            //bau don
            int stockBauDon = 0;
            List<Authority> NguoiBauDon = new List<Authority>();
            List<Authority> ResponseNguoiBauDon = new List<Authority>();
            //NguoiBauDon = GetNodeBinaryBauDon(DanhSachBauDon, ResponseNguoiBauDon, submitQuestion.QuestionID);
            if (DanhSachBauDon.Count > 0)
            {
                stockBauDon = DanhSachBauDon.Sum(x => x.AuthorityShare.Value);
            }
            var DanhSachUyQuyenFullSuKien = _dbContext.Authorities
                            .Where(x => x.EventID == submitQuestion.EventID
                            && x.AuthorityReceiveUserID == submitQuestion.UserID
                            && x.AuthorityType == AuthorityType.EVENT
                            ).ToList();
            int stockUyQuyen = 0;
            List<Authority> NguoiUyQuyen = new List<Authority>();
            List<Authority> ResponseNguoiUyQuyen = new List<Authority>();
            //NguoiUyQuyen = GetNodeBinaryUyQuyen(DanhSachUyQuyenFullSuKien, ResponseNguoiUyQuyen, submitQuestion.QuestionID);
            //if (NguoiUyQuyen.Count > 0)
            //{
            stockUyQuyen = DanhSachUyQuyenFullSuKien.Sum(x => x.AuthorityShare.Value);
            //}
            stockTotal = stockUyQuyen + stockBauDon;

            //cộng thêm stock của thằng submit
            stockTotal += userSubmit.UserStock;

            int TestStock = 0;
            int StockCuoiCung = 0; //tính tổng các stock đầu để làm tròn xuống stock cuối cùng

            //người vận hành submit. => tự tính stock cho người ta
            if (submitQuestion.Flag == 1)
            {
                //if (submitQuestion.Answers.Where(x => x.Selected == true).ToList().Count > 0)
                //{
                float countSelected = 0;
                if (submitQuestion.Answers.Count > 0)
                {
                    countSelected = submitQuestion.Answers.Where(x => x.Selected == true).ToList().Count;
                }
                     
                //feedBack
                if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
                {
                    countSelected++;
                }
                        
                //làm tròn lên
                double roundStock = Math.Ceiling(stockTotal / countSelected);
                //làm tròn xuống : Math.Floor

                //gán stock
                int i = 0;
                int flagRoundFirst = 0;
                foreach (var item in submitQuestion.Answers)
                {
                    if (item.Selected == true)
                    {
                        if (flagRoundFirst == 0)
                        {
                            submitQuestion.Answers[i].Stock = (int)roundStock;
                            flagRoundFirst++;
                            //
                            //TestStock += (int)roundStock;
                            StockCuoiCung += (int)roundStock;
                        }
                        else
                        {
                            submitQuestion.Answers[i].Stock = (int)Math.Floor(stockTotal / countSelected);
                            //
                            StockCuoiCung += (int)Math.Floor(stockTotal / countSelected);
                        }
                    }
                    i++;
                }

                //feedBack
                if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
                {
                    if (flagRoundFirst == 0)
                    {
                        submitQuestion.Feedback.Stock = (int)roundStock;
                        flagRoundFirst++;
                        //
                        TestStock += (int)roundStock;
                    }
                    else
                    {
                        submitQuestion.Feedback.Stock = (int)Math.Floor(stockTotal / countSelected);
                        //
                        TestStock += (int)Math.Floor(stockTotal / countSelected);
                    }
                }
                //}
            }
            if (submitQuestion.Flag == 0 && stockAnswer != stockTotal)
            {
                return (null, "Tổng số cổ phiếu truyền vào không đúng !!");
            }
            #endregion

            #region Insert Database and Check conditions
            //if (questions.Type != TypeQuestion.QA)
            //{
            submitQuestion.Stock = stockTotal;
            //}
            RequestSubmit reSubmit = new RequestSubmit();
            #region CheckReVote

            int flag = 0;
            //check revote
            (object resultGetall, string message) = GetAllResultOrganization(submitQuestion.AppId);
            //chưa check đúng kiểu dữ liệu hay không

            if (resultGetall != null)
            {
                var listResultSubmit = (List<ResponseResultSubmit>)resultGetall;
                if (listResultSubmit.Count != 0)
                {
                    var checkExistsThisSubmit = listResultSubmit.Where(x => x.QuestionID == submitQuestion.QuestionID
                    && x.AppId == submitQuestion.AppId
                    && submitQuestion.UserID == x.ContentJson.UserID
                    ).FirstOrDefault();
                    if (checkExistsThisSubmit != null)
                    {
                        flag = 1;
                        reSubmit.ResultID = checkExistsThisSubmit.ResultId;

                        //check da tra loi truoc su kien ma conơ bo phieu lai
                        var questionMariaDb = _dbContext.MiddleQuestions.Where(x => x.QuestionID == questions.QuestionID).FirstOrDefault();
                        if (questionMariaDb is null)
                        {
                            Console.WriteLine($"{questions.QuestionID} khong ton tai o mariaDb");
                            return (null, "QuestionNotFound");
                        }
                        if (questionMariaDb.BeforeEvent == BeforeEvent.Begin)
                        {
                            return (null, "QuestionBeginEvent");
                        }
                    }
                }
            }
            #endregion
            reSubmit.AppId = _config["SurveyQuestion:AppId"];
            //reSubmit.AppId = submitQuestion.AppId;
            reSubmit.Content = submitQuestion;
            reSubmit.Flag = flag;
            reSubmit.QuestionID = submitQuestion.QuestionID;
            if (stockTotal == 0)
            {
                return ("không tạo kết quả vì tổng cổ phần bằng 0", "SubmitSuccess");
            }
            (string data, int? statusCode) resultSubmit = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + "/submit", JsonConvert.SerializeObject(reSubmit), null).Result;
            //(string data, int? statusCode) resultSubmit = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"], JsonConvert.SerializeObject(reSubmit), null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }

            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            //UpdateSubmit
            //(string data, int? statusCode) resultSubmitUpdate = HttpMethod.Put.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + "/update-submit", JsonConvert.SerializeObject(reSubmit), null).Result;

            //if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            //{
            //    return (null, resultSubmit.data);
            //}

            //var responseSubmitUpdate = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);
            #endregion

            #region Socket dev hung
            if (token != null)
            {
                if (questions.OptionJson.Any(x => int.Parse(x.ToString()) == (int)OptionQuestion.SEE_RESULTS_RIGHT_AWAY))
                {
                    _reportService.SubmitSocketChartAsync(submitQuestion.EventID, submitQuestion.QuestionID, token, OptionQuestion.SEE_RESULTS_RIGHT_AWAY);
                }
                else 
                { 
                    _reportService.SubmitSocketChartAsync(submitQuestion.EventID, submitQuestion.QuestionID, token, null);
                }
            }
            #endregion
            
            return (responseSubmit.Data, "SubmitSuccess");
        }
        public (RequestSubmit result, string message) SubmitSingleQuestion2( SubmitQuestion  submitQuestion, string token,List<ResponseQuestion> listResponse, List<ResponseResultSubmit> listResult, List<Authority> Authorlist)
        {

            if(listResponse is null)
            {
                listResponse = new List<ResponseQuestion>();
            }
            if (listResult is null)
            {
                listResult = new List<ResponseResultSubmit>();
            }

            #region Get Question
            var questions = listResponse.Where(x => x.QuestionID == submitQuestion.QuestionID).FirstOrDefault();
            //get User Submit
            var userSubmit = _dbContext.EventUsers.FirstOrDefault(x => x.UserId == submitQuestion.UserID && x.EventId == submitQuestion.EventID);
            if (userSubmit is null)
            {
                return (null, "UserEventNotFound");
            }
            #endregion

            #region Check CountAnswers
            var CountAnswers = questions.CountAnswers;
            int checkCountAnswer = 0;

            if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
            {
                checkCountAnswer++;
            }
            int stockAnswer = 0;
            var listAnswerSelected = submitQuestion.Answers.Where(x => x.Selected == true).ToList().Count;
            checkCountAnswer += listAnswerSelected;
            #endregion

            #region Get All Value Tree Node
            int stockTotal = 0;
            var DanhSachBauDon = Authorlist
                            .Where(x => x.EventID == submitQuestion.EventID
                            && x.AuthorityReceiveUserID == submitQuestion.UserID
                            && x.QuestionID == submitQuestion.QuestionID
                            ).ToList();
            //bau don
            int stockBauDon = 0;
            List<Authority> NguoiBauDon = new List<Authority>();
            List<Authority> ResponseNguoiBauDon = new List<Authority>();
            //NguoiBauDon = GetNodeBinaryBauDon(DanhSachBauDon, ResponseNguoiBauDon, submitQuestion.QuestionID);
            if (DanhSachBauDon.Count > 0)
            {
                stockBauDon = DanhSachBauDon.Sum(x => x.AuthorityShare.Value);
            }
            var DanhSachUyQuyenFullSuKien = Authorlist
                            .Where(x => x.EventID == submitQuestion.EventID
                            && x.AuthorityReceiveUserID == submitQuestion.UserID
                            && x.AuthorityType == AuthorityType.EVENT
                            ).ToList();
            int stockUyQuyen = 0;
            List<Authority> NguoiUyQuyen = new List<Authority>();
            List<Authority> ResponseNguoiUyQuyen = new List<Authority>();
            //NguoiUyQuyen = GetNodeBinaryUyQuyen(DanhSachUyQuyenFullSuKien, ResponseNguoiUyQuyen, submitQuestion.QuestionID);
            //if (NguoiUyQuyen.Count > 0)
            //{
            stockUyQuyen = DanhSachUyQuyenFullSuKien.Sum(x => x.AuthorityShare.Value);
            //}
            stockTotal = stockUyQuyen + stockBauDon;

            //cộng thêm stock của thằng submit
            stockTotal += userSubmit.UserStock;

            int TestStock = 0;

            //người vận hành submit. => tự tính stock cho người ta
            if (submitQuestion.Flag == 1)
            {
                float countSelected = 0;
                if (submitQuestion.Answers.Count > 0)
                {
                    countSelected = submitQuestion.Answers.Where(x => x.Selected == true).ToList().Count;
                }

                //feedBack
                if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
                {
                    countSelected++;
                }

                //làm tròn lên
                double roundStock = Math.Ceiling(stockTotal / countSelected);
                //làm tròn xuống : Math.Floor

                //gán stock
                int i = 0;
                int flagRoundFirst = 0;
                foreach (var item in submitQuestion.Answers)
                {
                    if (item.Selected == true)
                    {
                        if (flagRoundFirst == 0)
                        {
                            submitQuestion.Answers[i].Stock = (int)roundStock;
                            flagRoundFirst++;
                            //
                            TestStock += (int)roundStock;
                        }
                        else
                        {
                            submitQuestion.Answers[i].Stock = (int)Math.Floor(stockTotal / countSelected);
                            //
                            TestStock += (int)Math.Floor(stockTotal / countSelected);
                        }
                    }
                    i++;
                }

                //feedBack
                if (submitQuestion.Feedback != null && submitQuestion.Feedback.Selected == true)
                {
                    if (flagRoundFirst == 0)
                    {
                        submitQuestion.Feedback.Stock = (int)roundStock;
                        flagRoundFirst++;
                        //
                        TestStock += (int)roundStock;
                    }
                    else
                    {
                        submitQuestion.Feedback.Stock = (int)Math.Floor(stockTotal / countSelected);
                        //
                        TestStock += (int)Math.Floor(stockTotal / countSelected);
                    }
                }
                //}
            }
            #endregion

            #region Check Cau Hoi truoc hoac sau su kien

            #endregion

            #region Insert Database and Check conditions
            submitQuestion.Stock = stockTotal;
            RequestSubmit reSubmit = new RequestSubmit();
            #region CheckReVote

            int flag = 0;
            //check revote
            //if (listResult.Count == 0)
            //{
            //    (object resultGetall, string message) = GetAllResultOrganization(submitQuestion.AppId);
            //    var listResultSubmit = (List<ResponseResultSubmit>)resultGetall;
            //    listResult = listResultSubmit;
            //}


            //chưa check đúng kiểu dữ liệu hay không

            if (listResult.Count != 0)
                {
                    var checkExistsThisSubmit = listResult.Where(x => x.QuestionID == submitQuestion.QuestionID
                    && x.AppId == submitQuestion.AppId
                    && submitQuestion.UserID == x.ContentJson.UserID
                    ).FirstOrDefault();
                    if (checkExistsThisSubmit != null)
                    {
                        flag = 1;
                        reSubmit.ResultID = checkExistsThisSubmit.ResultId;

                        //check da tra loi truoc su kien ma conơ bo phieu lai
                        var questionMariaDb = _dbContext.MiddleQuestions.Where(x => x.QuestionID == questions.QuestionID).FirstOrDefault();
                        if (questionMariaDb is null)
                        {
                            Console.WriteLine($"{questions.QuestionID} khong ton tai o mariaDb");
                            return (null, "QuestionNotFound");
                        }
                        if (questionMariaDb.BeforeEvent == BeforeEvent.Begin)
                        {
                            return (null, "QuestionBeginEvent");
                        }
                    }
                }
            #endregion
            reSubmit.AppId = _config["SurveyQuestion:AppId"];
            reSubmit.Content = submitQuestion.Copy();
            reSubmit.Flag = flag;
            reSubmit.QuestionID = submitQuestion.QuestionID;

            //@Tin Tran cổ phần sở hữu + cổ phần được ủy quyền = 0 thì Không tồn tại trong bất kì trường hợp biểu quyết nào hết nha
            if (stockTotal == 0)
            {
                return (null, "SubmitSuccess");
            }
            #endregion
            return (reSubmit, "SubmitSuccess");
        }

        public (object result, string message) GetUserRevote(ListRevote listRevote)
        {
            //lay danh sach nguoi uy quyen trươc sự kiện và ủy quyền bầu dồn phiếu cho câu hỏi đó.)
            var listAuthor = _dbContext.Authorities.Where( x => x.EventID == listRevote.EventId && (x.QuestionID == listRevote.QuestionId || x.AuthorityType == AuthorityType.EVENT)).ToList();
            //lấy danh sách tất cả user đang online trong event đó. xong ta loại trừ
            var Event = _dbContext.Events.FirstOrDefault(x => x.EventId == listRevote.EventId);
            if (Event is null)
            {
                return (null, "EventNotExists");
            }

            var listUserEvent = new List<EventUser>();
            if (Event.EventFlag == Events.Entities.EVENT_FLAG.CREATED)
            {
                listUserEvent = _dbContext.EventUsers.Where(x => x.EventId == listRevote.EventId).ToList();
            }
            else
            {
                listUserEvent = _dbContext.EventUsers.Where(x => x.EventId == listRevote.EventId && x.UserLatch == USER_LATCH.ON).ToList();
            }
            List<User> listUser = new List<User>();
            foreach (var item in listUserEvent)
            {
                //check user đó có tồn tại trong ủy quyền không. nếu không có ta thêm vào danh sách
                var checkExists = listAuthor.Where(x => x.AuthorityUserID == item.UserId).FirstOrDefault();
                if (checkExists is null)
                {
                    var user = _dbContext.Users.FirstOrDefault(x => x.UserId == item.UserId);
                    if (user != null)
                    {
                        listUser.Add(user);
                    }
                }
            }

            return (listUser, "GetUserRevoteSuccess");
        }
        public (object result,string message) GetAllResultOrganization(string organizationID)
        {
            (string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/submit/get-all/{organizationID}", null, null).Result;
            //(string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"/{organizationID}", null, null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }

            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            var listResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(responseSubmit.Data.ToString());
            
            return (listResult, "GetUserRevoteSuccess");
        }
    }
}
