using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Project.Modules.Reports.Entities;
using Project.Modules.Sessions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Project.Modules.Question.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Services;
using Project.Modules.Authorities.Entities;

namespace Project.Modules.Reports.Services
{

    public interface IReportService
    {
        public Report ExportReport(string eventId);

        public ResponseQuestion GetQuestionFromSurvey(string questionId);

        public List<SubmitQuestion> GetAnswerWithQuestion(string questionId);
        public object GetChartWithQuestion(string questionId, string eventId);

        public object GetChartStockWithQuestion(string questionId);

        public object ExportReportDataTable(string eventId, RequestTable requestTable);

        public (int totalPeopleSubmit, int totalPeople) GetPeople(string questionId, string eventId, bool flagQuestion = false);

        public Task SubmitSocketChartAsync(string eventId, string questionId, string token, OptionQuestion? optionQuestion);

        public List<SupportExportReportQuestion> SupportGetChartWithSession(string sessionId);

        public SupportExportReport SupportGetInfo(string eventId);

        public object GeneratorDataChartResponse(string eventId, string questionId, bool displayShowChart = false, string note = "");

        public string GetOptionQuestion(string questionId);

        public List<int> GeneratorStock(int stock, int totalChild);
    }
    public class ReportService: IReportService
    {
        private readonly MariaDBContext _mariaDBContext;
        public readonly IConfiguration _config;
        public readonly ISoketIO _soketIO;
        private readonly IOperatorService _operatorService;


        public ReportService(MariaDBContext mariaDBContext, IConfiguration config, ISoketIO soketIO, IOperatorService operatorService)
        {
            _mariaDBContext = mariaDBContext;
            _config = config;
            _soketIO = soketIO;
            _operatorService = operatorService;
        }


        public Report ExportReport(string eventId)
        {

            //prepare Report
            Report report = new Report();

            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return report;

            var totalDelegate = 0;

            var authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).Select(x => x.AuthorityUserID).ToList();
            if(_event.EventFlag == EVENT_FLAG.CREATED)
            {
                totalDelegate = _mariaDBContext.EventUsers.Count(x => x.EventId == eventId && x.UserStock > 0 && !authorities.Contains(x.UserId));
            }
            else
            {
                totalDelegate = _mariaDBContext.EventUsers.Count(x => x.EventId == eventId &&  !authorities.Contains(x.UserId) && x.UserLatch == USER_LATCH.ON);
            }
           

            // Get session
            List<Session> sessions = _mariaDBContext.Sessions.Where(x => x.EventId == eventId).ToList();


            //prepare Session
            List<ItemSession> itemSessions = new List<ItemSession>();
            foreach (var item in sessions)
            {
                ItemSession itemSession = new ItemSession
                {
                    SessionName = item.SessionName,
                    NumberOfParticipants = totalDelegate,
                    NumberOfQuestionsDiscussed = _mariaDBContext.QuestionClients.Count(x => x.SessionId == item.SessionId),
                    NumberOfVotingQuestions = _mariaDBContext.MiddleQuestions.Count(x=> x.SessionID == item.SessionId && x.Type == TypeMiddle.ACTIVE),
                    QuestionIds = _mariaDBContext.MiddleQuestions.Where(x => x.SessionID == item.SessionId && x.Type == TypeMiddle.ACTIVE).Select(x=> x.QuestionID).ToList(),
                    DataExport = null
                };
                itemSession.DataExport = GetQuestionAndAnswer(itemSession.QuestionIds);
                itemSessions.Add(itemSession);
            }
            report.Sessions = itemSessions;
            report.TotalDelegate = totalDelegate;
            report.TotalQuestion = report.Sessions.Sum(x => x.NumberOfQuestionsDiscussed);
            report.TotalVoting = report.Sessions.Sum(x=>x.NumberOfVotingQuestions);
            return report;
        }


        public object ExportReportDataTable(string eventId, RequestTable requestTable)
        {

            //prepare Report
            Report report = new Report();

            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return report;

            var totalDelegate = 0;

            var authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).Select(x => x.AuthorityUserID).ToList();
            if (_event.EventFlag == EVENT_FLAG.CREATED)
            {
                totalDelegate = _mariaDBContext.EventUsers.Count(x => x.EventId == eventId && x.UserStock > 0 && !authorities.Contains(x.UserId));
            }
            else
            {
                totalDelegate = _mariaDBContext.EventUsers.Count(x => x.EventId == eventId && !authorities.Contains(x.UserId) && x.UserLatch == USER_LATCH.ON);
            }

            // Get session
            List<Session> sessions = _mariaDBContext.Sessions.Where(x => x.EventId == eventId).ToList();


            //prepare Session
            List<ItemSession> itemSessions = new List<ItemSession>();
            foreach (var item in sessions)
            {
                ItemSession itemSession = new ItemSession
                {
                    SessionName = item.SessionTitle,
                    SessionSort = item.SessionSort,
                    NumberOfParticipants = totalDelegate,
                    NumberOfQuestionsDiscussed = _mariaDBContext.QuestionClients.Count(x => x.SessionId == item.SessionId),
                    NumberOfVotingQuestions = _mariaDBContext.MiddleQuestions.Count(x => x.SessionID == item.SessionId && x.Type == TypeMiddle.ACTIVE),
                    QuestionIds = _mariaDBContext.MiddleQuestions.Where(x => x.SessionID == item.SessionId && x.Type == TypeMiddle.ACTIVE).Select(x => x.QuestionID).ToList(),
                    DataExport = null
                };
                itemSession.DataExport = GetQuestionAndAnswer(itemSession.QuestionIds);
                itemSessions.Add(itemSession);
            }

            report.Sessions = itemSessions;
            report.TotalDelegate = totalDelegate;
            report.TotalQuestion = report.Sessions.Sum(x => x.NumberOfQuestionsDiscussed);
            report.TotalVoting = report.Sessions.Sum(x => x.NumberOfVotingQuestions);

            List<ItemSession> data = new List<ItemSession>();

            

            #region Paganation
            data = itemSessions.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.SessionName.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.NumberOfParticipants.ToString().Contains(requestTable.Search.ToLower()) ||
                        x.NumberOfQuestionsDiscussed.ToString().Contains(requestTable.Search.ToLower()) ||
                        x.NumberOfVotingQuestions.ToString().Contains(requestTable.Search.ToLower())
                    )
                )).AsQueryable().OrderBy(OrderValue(requestTable.SortField, requestTable.SortOrder)).ToList();

            ResponseTable responseTable = new ResponseTable()
            {
                DateResult = data.Skip((requestTable.Page - 1) * requestTable.Results).Take(requestTable.Results).ToList(),
                Info = new Info()
                {
                    Page = requestTable.Page,
                    TotalRecord = data.Count,
                    Results = requestTable.Results

                },
                Total = new
                {
                    TotalDelegate = report.TotalDelegate,
                    TotalQuestion = report.TotalQuestion,
                    TotalVoting = report.TotalVoting
                }
            };
            #endregion
            return responseTable;


        }


        public object GetQuestionAndAnswer(List<string> questionIds)
        {
            #region Call api
            (string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{_config["SurveyQuestion:AppId"]}", null, null).Result;
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return null;
            }
            var result = JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            #endregion
            var listQuestion = JsonConvert.DeserializeObject<List<ResponseQuestion>>(result.Data.ToString());
            return listQuestion.Where(x => questionIds.Contains(x.QuestionID)).ToList();
        }


        public ResponseQuestion GetQuestionFromSurvey(string questionId)
        {
            (string data, int? statusCode) = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{_config["SurveyQuestion:AppId"]}", null, null).Result;
            if (statusCode != 200) return null;
            var dataPrepare = JObject.Parse(data);

            var questionPrepare = dataPrepare.SelectToken($"$.data[?(@questionID=='{questionId}')]");
            if (questionPrepare is null) return null ;
            return questionPrepare.ToObject<ResponseQuestion>();
        }


        public List<SubmitQuestion> GetAnswerWithQuestion(string questionId)
        {
            // Số người trả lời đc ghi nhận:
            ResponseResultSubmit hung = new ResponseResultSubmit();
            (string data, int? statusCode) = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/submit/get-all/{_config["SurveyQuestion:AppId"]}", null, null).Result;
            if (statusCode != 200) return null;

            var dataPrepare = JObject.Parse(data);
            var answerPrepare = JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(dataPrepare["data"].ToString());
            List<SubmitQuestion> answers = answerPrepare.Where(x => x.QuestionID == questionId).Select(x=> x.ContentJson).ToList();
            return answers;
        }


        public string GetOptionQuestion(string questionId)
        {
            ResponseQuestion question = this.GetQuestionFromSurvey(questionId);
            if (question is null) return "0";
            if(question.OptionJson.Any(x=> int.Parse(x.ToString()) ==  (int)OptionQuestion.SEE_RESULTS_RIGHT_AWAY))
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }


        public object GetChartWithQuestion(string questionId, string eventId)
        {
            //prepare question
            ResponseQuestion question = this.GetQuestionFromSurvey(questionId);
            if (question is null) return null;


            //prepare answer
            List<SubmitQuestion> answers = this.GetAnswerWithQuestion(questionId);

            if (question.IsSent)
            {
                // Tự tính số người trả lời mặc định
                bool flagQuestion = this.GetFlagQuestion(questionId);
                List<SubmitQuestion> answerDefault = this.GetAnswerWithQuestionDefault(questionId, eventId, flagQuestion).Where(x => !answers.Select(y => y.UserID).Contains(x.UserID)).ToList();
                answers.AddRange(answerDefault);
            }

           



            ChartResponse chartResponse = new ChartResponse
            {
                questionId = questionId,
                questionName = question.Content
            };

            if(question.AnswersJson != null)
            {
                foreach (var item in question.AnswersJson)
                {
                    chartResponse.data.Add(new AnswersResponse
                    {
                        title = item.Title,
                        label = item.Content,
                        value = 0
                    }); 
                }
            }


            if (question.FeedbackJson != null)
            {
                chartResponse.data.Add(new AnswersResponse
                {
                    title = "Đáp án khác",
                    label = "Đáp án khác",
                    value = 0
                });
            }

            // set answers for question 

            if (answers != null)
            {
                foreach (var item in answers)
                {
                    foreach (var tmp in chartResponse.data)
                    {
                        tmp.value = tmp.value + item.Answers.Where(x => x.Title.Equals(tmp.title) && x.Selected == true).Count();
                        //if (tmp.value > 0 )
                        //{
                        //    tmp.stock = tmp.stock + (int) item.Stock;
                        //}
                        tmp.stock = tmp.stock + item.Answers.Where(x => x.Title.Equals(tmp.title) && x.Selected == true).Select(x => x.Stock).ToList().convertStock();
                    }

                    if (item.Feedback != null)
                    {
                        var tmp = chartResponse.data.FirstOrDefault(x => x.title.Equals("Đáp án khác"));
                        if (item.Feedback.Selected) tmp.value += 1;
                        if (item.Feedback.Stock.HasValue) tmp.stock += item.Feedback.Stock.Value;
                    }
                }
            }
            return chartResponse;

        }


        public object GetChartStockWithQuestion(string questionId)
        {
            //prepare question
            ResponseQuestion question = this.GetQuestionFromSurvey(questionId);
            if (question is null) return null;


            //prepare answer
            List<SubmitQuestion> answers = this.GetAnswerWithQuestion(questionId);



            ChartResponse chartResponse = new ChartResponse
            {
                questionId = questionId,
                questionName = question.Content
            };

            foreach (var item in question.AnswersJson)
            {
                chartResponse.data.Add(new AnswersResponse
                {
                    title = item.Title,
                    label = item.Content,
                    value = 0
                });
            }


            // AnswersResponse
            if (question.FeedbackJson != null)
            {
                chartResponse.data.Add(new AnswersResponse
                {
                    title = "Đáp án khác",
                    label = "Đáp án khác",
                    value = 0
                });
            }




            

            // set answers for question 
            foreach (var item in answers)
            {
                foreach (var tmp in chartResponse.data)
                {
                    tmp.value = tmp.value + item.Answers.Where(x => x.Title.Equals(tmp.title)).Select(x=>x.Stock).ToList().convertStock();
                }

                if(item.Feedback!= null)
                {
                    var tmp = chartResponse.data.FirstOrDefault(x => x.title.Equals("Đáp án khác"));
                    if (item.Feedback.Selected) tmp.value += 1;
                    if (item.Feedback.Stock.HasValue) tmp.value += item.Feedback.Stock.Value;
                }

            }




            return chartResponse;

        }

        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }

        public List<SubmitQuestion> GetAnswerWithQuestionDefault(string questionId, string eventId, bool flagQuestion = false)
        {
            List<SubmitQuestion> result = new List<SubmitQuestion>();
            ResponseQuestion question = this.GetQuestionFromSurvey(questionId);
            if (question is null) return result;

            var countQuestionDefault = question.AnswersJson is null ?  0 : question.AnswersJson.Count(x => x.Bingo == true);
            var flagQuestionDefaultFeedBack = false;
            if(question.FeedbackJson != null)
            {
                flagQuestionDefaultFeedBack = question.FeedbackJson.Bingo;
            }

            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return result;

            if (countQuestionDefault > 0 || flagQuestionDefaultFeedBack)
            {
                List<EventUser> eventUsers = new List<EventUser>();
                List<Authority> authorities = _mariaDBContext.Authorities.Where(x => (x.AuthorityType == AuthorityType.EVENT || x.QuestionID == questionId) && x.EventID == eventId ).ToList();
               
                if (_event.EventFlag == EVENT_FLAG.CREATED || flagQuestion)
                {
                    var users = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserStock > 0 && !authorities.Select(y=> y.AuthorityUserID).Contains(x.UserId)).ToList();
                    eventUsers.AddRange(users);
                }
                else // Sau sự kiện
                {
                    var users = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserStock > 0 && !authorities.Select(y => y.AuthorityUserID).Contains(x.UserId) && x.UserLatch == USER_LATCH.ON).ToList();
                    eventUsers.AddRange(users);
                }
               

                // Cộng cổ phần 
                foreach (var item in eventUsers)
                {
                    // uy quyen 
                    var totalStockAuthority = authorities.Where(x => x.AuthorityReceiveUserID == item.UserId && x.AuthorityType == AuthorityType.EVENT).Sum(x => x.AuthorityShare.Value);

                    // bau phieu
                    var totalStockVote = _mariaDBContext.Authorities.Where(x => x.AuthorityType == AuthorityType.QUESTION && x.EventID == eventId && x.QuestionID == questionId && x.AuthorityReceiveUserID == item.UserId).Sum(x => x.AuthorityShare.Value);
                    totalStockAuthority = item.UserStock + totalStockAuthority + totalStockVote;


                    //generator answer
                    SubmitQuestion tmpAnswer = new SubmitQuestion
                    {
                        QuestionID = question.QuestionID,
                        Answers = new List<Selection>(),
                        UserID = item.UserId
                    };
                    List<int> cophans = this.GeneratorStock(totalStockAuthority, countQuestionDefault);
                    int idex = 0;
                    foreach (var tmpQuestion in question.AnswersJson.Where(x => x.Bingo == true).ToList())
                    {
                        tmpAnswer.Answers.Add(new Selection
                        {
                            Code = tmpQuestion.Code,
                            Auto = true,
                            Bingo = true,
                            Content = tmpQuestion.Content,
                            File = tmpQuestion.File,
                            Selected = true,
                            Stock = cophans[idex],
                            Title = tmpQuestion.Title
                        });
                        idex++;
                    }
                    result.Add(tmpAnswer);

                }


                // Genarator 
               

                
                
                

            }
            return result;

        }


        public List<int> GeneratorStock(int stock, int totalChild)
        {
            List<int> result = new List<int>();

            int tmpSum = stock;
            int tmptotalChild = totalChild;

            while (tmptotalChild > 0)
            {
                var tmp = tmpSum / tmptotalChild;
                if (tmpSum % tmptotalChild > 0)
                {
                    result.Add(tmp);
                    tmpSum = tmpSum - tmp;
                    tmptotalChild--;
                    continue;
                }

                result.Add(tmp);
                tmpSum = tmpSum - tmp;
                tmptotalChild--;
             
            }
            return result.OrderByDescending(x=> x).ToList();
        }

        // get bao nhieu nguoi tra loi cau hoi do tren tong so nguoi them gia su kien
        public (int totalPeopleSubmit, int totalPeople) GetPeople(string questionId,string eventId, bool flagQuestion = false)
        {
            // Số người trả lời
            List<SubmitQuestion> answers = this.GetAnswerWithQuestion(questionId);


            ResponseQuestion question = this.GetQuestionFromSurvey(questionId);
            if (question is null) return (0,0);

            if (question.IsSent)
            {
                // Tự tính số người trả lời mặc định
                List<SubmitQuestion> answerDefault = this.GetAnswerWithQuestionDefault(questionId, eventId, flagQuestion).Where(x=> !answers.Select(y=> y.UserID).Contains(x.UserID)).ToList();
                answers.AddRange(answerDefault);
            }
           

            // Tổng số người 
            int totalPeople = this.GetTotalPeople(questionId, eventId, flagQuestion);

            return (answers.Count(), totalPeople);
        }


        public int GetTotalPeople(string questionId, string eventId, bool flagQuestion = false)
        {
            MiddleQuestion middleQuestion = _mariaDBContext.MiddleQuestions.FirstOrDefault(x => x.QuestionID == questionId);
            if (middleQuestion is null) return 0;
            Session session = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId == middleQuestion.SessionID);
            if (session is null) return 0;

            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return 0;


            // Loại bỏ những thằng đã ủy quyền ra
            var authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).Select(x => x.AuthorityUserID).ToList();

            if (_event.EventFlag == EVENT_FLAG.CREATED || flagQuestion)
            {
                int countUser = _mariaDBContext.EventUsers.Count(x => x.EventId == session.EventId && x.UserStock > 0 && !authorities.Contains(x.UserId));
                return countUser;
            }
            else
            {
                int countUser = _mariaDBContext.EventUsers.Count(x => x.EventId == session.EventId && x.UserStock > 0  && !authorities.Contains(x.UserId) && x.UserLatch == USER_LATCH.ON);
                return countUser;
            }
        }


        // Trạng thái câu hỏi  đc vote sau hay trước sự kiện.
        // 1 begin 
        // 0  chua begin 
        public bool GetFlagQuestion(string questionId)
        {
            var question = _mariaDBContext.MiddleQuestions.FirstOrDefault(x => x.QuestionID == questionId);
            if (question is null) return false;
            if(question.BeforeEvent == 0) // trước evetn 
            {
                return true;
            }
            return false;
        }

        public object GeneratorDataChartResponse(string eventId, string questionId, bool displayShowChart = false, string note ="")
        {
            bool flagQuestion = this.GetFlagQuestion(questionId);
            object dataChart = this.GetChartWithQuestion(questionId, eventId);

            (int totalPeopleSubmit, int totalPeople) = this.GetPeople(questionId, eventId, flagQuestion);
            ChartForEvent chartForEvent = _operatorService.GetChart(eventId, flagQuestion);

            int totalCumulativeVotes = _operatorService.GetTotalCumulativeVotes(questionId, eventId);

            var resultChart = new GeneratorDataChart
            {
                displayShowChart = displayShowChart,
                result = dataChart,
                total = new TotalDataChart
                {
                    totalCumulativeVotes = totalCumulativeVotes,
                    totalPeopleSubmit = totalPeopleSubmit,
                    totalPeople = totalPeople,
                    totalNumberOfSharesRepresented = chartForEvent.totalNumberOfSharesRepresented,
                    totalShares = chartForEvent.totalShares,
                    totalStock = chartForEvent.totalStock
                },
                sum = chartForEvent
            };
            return resultChart;
        }


        public async Task SubmitSocketChartAsync(string eventId, string questionId, string token, OptionQuestion? optionQuestion = null )
        {

            var dataSocket = this.GeneratorDataChartResponse(eventId,questionId);
            string member = "0";
            if (optionQuestion.HasValue && optionQuestion.Value == OptionQuestion.SEE_RESULTS_RIGHT_AWAY)
            {
                member = "1";
            }
            await _soketIO.ForwardAsync(eventId, dataSocket, token, "realtimeChart", null, "1", member);
        }




        public List<SupportExportReportQuestion> SupportGetChartWithSession(string sessionId)
        {
            //prepare event

            Session session = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId== sessionId);
            if(session is null) return new List<SupportExportReportQuestion>();


            int sumAuthoriter = _mariaDBContext.Authorities.Where(x => x.EventID == session.EventId && x.AuthorityType == AuthorityType.EVENT).Sum(x => x.AuthorityShare.Value);

            int totalStock = _mariaDBContext.EventUsers.Where(x => x.EventId == session.EventId && x.UserLatch == USER_LATCH.ON).Sum(x => x.UserStock) + sumAuthoriter;
            int totalStockBefor = _mariaDBContext.EventUsers.Where(x => x.EventId == session.EventId).Sum(x => x.UserStock) + sumAuthoriter;


            List<SupportExportReportQuestion> result = new List<SupportExportReportQuestion>();

            List<MiddleQuestion> middleQuestions = _mariaDBContext.MiddleQuestions.Where(x => x.SessionID == sessionId).ToList();
            foreach (var item in middleQuestions)
            {
                ResponseQuestion question = this.GetQuestionFromSurvey(item.QuestionID);
                if (question is null) continue;

                var tmpQuestion = new SupportExportReportQuestion
                {
                    QuestionId = question.QuestionID,
                    QuestionName = question.Content,
                    Answers = new List<SupportExportReportAnswers>(),
                };
                


                //prepare answer
                List<SubmitQuestion> answers = this.GetAnswerWithQuestion(item.QuestionID);


                if (question.IsSent)
                {
                    // Tự tính số người trả lời mặc định
                    bool flagQuestion = this.GetFlagQuestion(item.QuestionID);
                    List<SubmitQuestion> answerDefault = this.GetAnswerWithQuestionDefault(item.QuestionID, session.EventId, flagQuestion).Where(x => !answers.Select(y => y.UserID).Contains(x.UserID)).ToList();
                    answers.AddRange(answerDefault);
                }
              



                if (question.AnswersJson is null) continue;

                var tmpAnswerFeeback = new SupportExportReportAnswers();
                tmpAnswerFeeback.label = "Đáp án khác";
                tmpAnswerFeeback.title = "Đáp án khác";
                tmpAnswerFeeback.percent = 0;
                tmpAnswerFeeback.stock = 0;


                foreach (var answer in question.AnswersJson)
                {

                    var tmpAnswer = new SupportExportReportAnswers
                    {
                        label = answer.Code,
                        title = answer.Content,
                        stock = 0,
                        percent = 0
                    };

  
                    if (answers is null)
                    {
                        tmpQuestion.Answers.Add(tmpAnswer);
                        continue;
                    }
                    foreach (var itemTmp in answers)
                    {
                        tmpAnswer.stock += itemTmp.Answers.Where(x => x.Title.Equals(answer.Title)).Select(x => x.Stock).ToList().convertStock();
                        if(itemTmp.FlagBeforeEvent == 1)
                        {
                            tmpAnswer.percent = (double)tmpAnswer.stock / totalStockBefor;
                        }
                        else
                        {
                            tmpAnswer.percent = (double)tmpAnswer.stock / totalStock;
                        }
                        
                    }
                    tmpQuestion.Answers.Add(tmpAnswer);

                }



                if (question.FeedbackJson != null)
                {
                    foreach (var itemTmp in answers)
                    {
                        if (itemTmp.Feedback != null && itemTmp.Feedback.Selected)
                        {
                            tmpAnswerFeeback.stock += itemTmp.Feedback.Stock.convertStock();


                            if (itemTmp.FlagBeforeEvent == 1)
                            {
                                tmpAnswerFeeback.percent = (double)tmpAnswerFeeback.stock / totalStockBefor;
                            }
                            else
                            {
                                tmpAnswerFeeback.percent = (double)tmpAnswerFeeback.stock / totalStock;
                            }
                            

                        }
                    }
                       
                    tmpQuestion.Answers.Add(tmpAnswerFeeback);
                }


                result.Add(tmpQuestion);
            }

            return result;
        }

        public SupportExportReport SupportGetInfo(string eventId)
        {
            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return new SupportExportReport();

            var totalNumberOfShareholdersAttending = _mariaDBContext.EventUsers.Count(x=> x.EventId == eventId && x.UserLatch == USER_LATCH.ON);
            var shareholderCodeTotal = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId).Sum(x => x.UserStock);
            var shareholderCodeAttending = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserLatch == USER_LATCH.ON).Sum(x=> x.UserStock);
            var percent = shareholderCodeAttending / shareholderCodeTotal;
            return new SupportExportReport
            {
                totalNumberOfShareholdersAttending = totalNumberOfShareholdersAttending,
                shareholderAttending = shareholderCodeAttending,
                shareholderTotal = shareholderCodeTotal,
                percent = percent
            };
        }



    }


    public class SupportExportReport
    {
        public int totalNumberOfShareholdersAttending { get; set; } =  0;
        public int shareholderTotal { get; set; } =  0;
        public int shareholderAttending { get; set; } =  0;
        public int percent { get; set; } =  0;
    }

    public class SupportExportReportQuestion
    {
        public string QuestionName { get; set; }
        public string QuestionId { get; set; }
        public List<SupportExportReportAnswers> Answers { get; set; }
        public bool QuestionType { get; set; } = false;
    }

    public class SupportExportReportAnswers
    {
        public string title { get; set; }
        public string label { get; set; }
        public double percent { get; set; } 
        public int stock { get; set; }
    }





    public class ChartResponse
    {
        public string questionName { get; set; }
        public string questionId { get; set; }
        public List<AnswersResponse> data { get; set; } = new List<AnswersResponse>();
    }

    public class AnswersResponse
    {
        public string title { get; set; }
        public string label { get; set; }
        public int value { get; set; } // so nguoi tra loi cau hoi cho dap an nay
        public int stock { get; set; }
    }


    public class ChartRealTime
    {
        public string eventID { get; set; }
        public string message { get; set; } 
        public object data { get; set; }
        public string admin { get; set; }
        public string member { get; set; }
        public string projector { get; set; }
        public List<string> members { get; set; }
    }

}
