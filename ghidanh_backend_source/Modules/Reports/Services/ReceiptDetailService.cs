using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Entities;
using Project.Modules.Receipts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Reports.Services
{
    public interface IReceiptDetailService
    {
        public List<ReceiptDetailSubjectReponse> GetReceiptDetail(DateTime dateTime);
    }
    public class ReceiptDetailService : IReceiptDetailService
    {
        private readonly IRepositoryMariaWrapper _repositoryMariaWrapper;
        public ReceiptDetailService(IRepositoryMariaWrapper repositoryMariaWrapper)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;
        }

        public List<ReceiptDetailSubjectReponse> GetReceiptDetail(DateTime dateTime)
        {
            var receiptsInDay = _repositoryMariaWrapper.Receipts.FindByCondition(x => x.CreatedAt.Date == dateTime.Date).AsEnumerable().GroupBy(x => x.ClassId).Select(x => new { ClassId = x.Key, Receipt = x.ToList() }).ToList();
            var receiptsAccumulated = _repositoryMariaWrapper.Receipts.FindByCondition(x => x.CreatedAt.Date <= dateTime.Date).AsEnumerable().GroupBy(x => x.ClassId).Select(x => new { ClassId = x.Key, Receipt = x.ToList() }).ToList();
            var subjectWithClass = _repositoryMariaWrapper.ClassSchedules.FindByCondition(x => receiptsAccumulated.Select(x => x.ClassId).Contains(x.ClassId)).Include(x => x.Subject).Select(x => new { ClassId = x.ClassId, Subjects = x.Subject });

            List<ReceiptDetailClassReponse> prepareResult = new List<ReceiptDetailClassReponse>();


            //foreach (var item in receiptsInDay)
            //{
            //    ReceiptDetailReponse tmpReceiptDetail = new ReceiptDetailReponse
            //    {
            //        ClassId = item.ClassId,
            //        TotalStudentInPeriod = item.Receipt.Count,
            //        TotalAmountInPeriod = item.Receipt.Sum(x => x.Amount)
            //    };

            //    Class tmpClass = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == tmpReceiptDetail.ClassId);
            //    if (tmpClass != null)
            //    {
            //        tmpReceiptDetail.ClassName = tmpClass.ClassName;
            //    }

            //    var tmp = receiptsAccumulated.FirstOrDefault(x => x.ClassId == tmpReceiptDetail.ClassId);
            //    if (tmp != null)
            //    {
            //        tmpReceiptDetail.TotalStudentAccumulated = tmp.Receipt.Count;
            //        tmpReceiptDetail.TotalAmountAccumulated = tmp.Receipt.Sum(x => x.Amount);
            //    }
            //    result.Add(tmpReceiptDetail);
            //}

            foreach (var item in receiptsAccumulated)
            {
                ReceiptDetailClassReponse tmpReceiptDetail = new ReceiptDetailClassReponse
                {
                    ClassId = item.ClassId,
                    TotalStudentAccumulated = item.Receipt.Count,
                    TotalAmountAccumulated = item.Receipt.Sum(x => x.Amount)
                };


                var tmpSubject = subjectWithClass.FirstOrDefault(x => x.ClassId == item.ClassId);
                if(tmpSubject != null)
                {
                    tmpReceiptDetail.SubjectId = tmpSubject.Subjects.SubjectId;
                    tmpReceiptDetail.SubjectCode = tmpSubject.Subjects.SubjectCode;
                    tmpReceiptDetail.SubjectName = tmpSubject.Subjects.SubjectName;
                }
                //Class tmpClass = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == tmpReceiptDetail.ClassId);
                //if (tmpClass != null)
                //{
                //    tmpReceiptDetail.ClassName = tmpClass.ClassName;
                //}

                var tmp = receiptsInDay.FirstOrDefault(x => x.ClassId == tmpReceiptDetail.ClassId);
                if (tmp != null)
                {
                    tmpReceiptDetail.TotalStudentInPeriod = tmp.Receipt.Count;
                    tmpReceiptDetail.TotalAmountInPeriod = tmp.Receipt.Sum(x => x.Amount);
                }
                prepareResult.Add(tmpReceiptDetail);
            }

            List<ReceiptDetailSubjectReponse> result = new List<ReceiptDetailSubjectReponse>();

            var groupSubjects = prepareResult.GroupBy(x => x.SubjectId).Select(x => new { SubjectId = x.Key, GroupSubjects =  x.ToList() }).ToList();
            foreach (var item in groupSubjects)
            {
                ReceiptDetailSubjectReponse receiptDetailSubjectReponse = new ReceiptDetailSubjectReponse
                {
                    SubjectId = item.SubjectId,
                    SubjectCode = item.GroupSubjects.FirstOrDefault().SubjectCode,
                    SubjectName = item.GroupSubjects.FirstOrDefault().SubjectName,

                    TotalAmountAccumulated = item.GroupSubjects.Sum(x => x.TotalAmountAccumulated),
                    TotalAmountInPeriod = item.GroupSubjects.Sum(x => x.TotalAmountInPeriod),
                    TotalStudentAccumulated = item.GroupSubjects.Sum(x => x.TotalStudentAccumulated),
                    TotalStudentInPeriod = item.GroupSubjects.Sum(x => x.TotalStudentInPeriod)
                };
                result.Add(receiptDetailSubjectReponse);
            }
           
            return result;
        }

    }


    // Receipt Theo lớp
    public class ReceiptDetailClassReponse
    {
        public string ClassId { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }



        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }

        // Lũy kế
        public double TotalAmountAccumulated { get; set; }
        public double TotalStudentAccumulated { get; set; }

        // Trong kỳ
        public double TotalAmountInPeriod { get; set; }
        public double TotalStudentInPeriod { get; set; }


        public string Note { get; set; }


    }
    // Receipt Theo môn học
    public class ReceiptDetailSubjectReponse
    {
        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }

        // Lũy kế
        public double TotalAmountAccumulated { get; set; }
        public double TotalStudentAccumulated { get; set; }

        // Trong kỳ
        public double TotalAmountInPeriod { get; set; }
        public double TotalStudentInPeriod { get; set; }


        public string Note { get; set; }


    }
}
