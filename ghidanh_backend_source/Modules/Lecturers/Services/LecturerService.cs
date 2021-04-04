using AutoMapper;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Accounts.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Project.Modules.Lecturers.Services
{
    public interface ILecturerService
    {
        Task<(Lecturer lecturer, string message)> StoreAsync(AddLecturerRequest request);
        Task<(Lecturer lecturer, string message)> UpdateAsync(UpdateLecturerRequest request, string lecturerId);
        (ResponseTable result, string message) ShowTable(RequestTable request);
        (Lecturer lecturer, string message) GetLecturer(string lecturerId);
        (Lecturer lecturer, string message) DeleteLecturer(string lecturerId);
        List<LectureSelect> ShowAll();
        List<LectureSelect> GetListLectureTeachSubject(string subjectId);
        long GetNumberLecturer();
    }
    public class LecturerService : ILecturerService
    {
        private readonly IRepositoryMariaWrapper repository;
        private readonly string pathForder = "Lecturer/" + DateTime.Now.ToString("yyyyMMdd");
        private readonly IMapper mapper;
        public LecturerService(IRepositoryMariaWrapper _repository, IMapper _mapper)
        {

            this.mapper = _mapper;
            this.repository = _repository;
        }

        public (Lecturer lecturer, string message) DeleteLecturer(string lecturerId)
        {
            (Lecturer lecturer, string message) = GetLecturer(lecturerId);
            if (lecturer is null)
                return (lecturer, message);
            if (!String.IsNullOrEmpty(lecturer.LecturerImage))
            {
                _ = GeneralHelper.DeleteFile(lecturer.LecturerImage);
            }
            List<AccountPermission> accountPermissions = repository.AccountPermissions
                                                                   .FindByCondition(m => m.AccountId.Equals(lecturer.AccountId))
                                                                   .ToList();

            Account account = repository.Accounts.GetById(lecturer.AccountId);

            repository.Lecturers.RemoveMaria(lecturer);
            repository.SaveChanges();

            repository.AccountPermissions.RemoveRangeMaria(accountPermissions);
            repository.SaveChanges();

            repository.Accounts.RemoveMaria(account);
            repository.SaveChanges();
            return (lecturer, "DeleteSuccess");
        }

        public (Lecturer lecturer, string message) GetLecturer(string lecturerId)
        {
            Lecturer lecturer = repository.Lecturers.GetById(lecturerId);
            if (lecturer is null)
            {
                return (null, "IdNotExist");
            }
            lecturer.OfficialSubject = repository.Subjects.GetById(lecturer.SubjectId);
            List<string> extraSubjectIds = repository.LecturerSubjects
                                                     .FindByCondition(m => m.LecturerId.Equals(lecturerId) && !m.SubjectId.Equals(lecturer.SubjectId))
                                                     .Select(m => m.SubjectId)
                                                     .ToList();

            lecturer.ExtraSubject = repository.Subjects.FindByCondition(m => extraSubjectIds.Contains(m.SubjectId)).ToList();
            return (lecturer, "Success");
        }

        public List<LectureSelect> GetListLectureTeachSubject(string subjectId)
        {
            List<string> lecturerIds = repository.LecturerSubjects
                                                 .FindByCondition(l => l.SubjectId.Equals(subjectId))
                                                 .Select(l => l.LecturerId)
                                                 .ToList();

            if (lecturerIds.Count == 0)
            {
                return null;
            }
            return repository.Lecturers.FindByCondition(le => lecturerIds.Contains(le.LecturerId))
                                       .Select(le => new LectureSelect(le))
                                       .ToList();
        }

        public long GetNumberLecturer()
        {
            return repository.Lecturers.FindAll().Count();
        }

        public List<LectureSelect> ShowAll()
        {
            List<LectureSelect> lectures = repository.Lecturers
                                                     .FindAll()
                                                     .Select(m => new LectureSelect(m))
                                                     .ToList();

            return (lectures);
        }

        public (ResponseTable result, string message) ShowTable(RequestTable request)
        {
            List<Lecturer> lecturers = repository.Lecturers.FindAll().OrderByDescending(m => m.LecturerCreated).ToList();
            lecturers = lecturers.Where(m => String.IsNullOrEmpty(request.Search) || (
                                        m.LecturerCode.ToLower().Contains(request.Search.ToLower()) ||
                                        (m.LecturerLastName.ToLower() + " " + m.LecturerFistName.ToLower()).Contains(request.Search.ToLower()) ||
                                        m.LecturerEmail.ToLower().Contains(request.Search.ToLower()) ||
                                        m.LecturerPhone.ToLower().Contains(request.Search.ToLower())
                                       )).ToList();
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                lecturers = lecturers.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder)).ToList();

            }
            ResponseTable response = GeneralHelper.ShowTable(new List<object>(lecturers), request);
            return (response, "ShowLecturerSuccess");
        }

        public async Task<(Lecturer lecturer, string message)> StoreAsync(AddLecturerRequest request)
        {
            string saft = 5.RandomString();

            using var transaction = repository.BeginTransaction();
            try
            {


                Account account = new Account
                {
                    AccountCode = request.LecturerCode,
                    Saft = saft,
                    Password = (request.Password + saft).HashPassword(),
                    AccountType = Account.TYPE_ACCOUNT.LECTURER,
                    GroupCode = PERMISSION_DEFAULT.Lecturer,
                };
                repository.Accounts.Add(account);
                repository.SaveChanges();

                #region thêm quyền vào bảng Account Permission
                List<string> permissionCodes = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(PERMISSION_DEFAULT.Lecturer)).Select(m => m.PermissionCode).ToList();
                foreach (string permissionCode in permissionCodes)
                {
                    AccountPermission accountPermission = new AccountPermission(account.AccountId, permissionCode);
                    repository.AccountPermissions.Add(accountPermission);
                }
                #endregion

                Lecturer lecturer = mapper.Map<Lecturer>(request);
                if (!(request.Image is null))
                {
                    (string filePath, _) = await GeneralHelper.UploadFileProAsync(request.Image, this.pathForder);
                    lecturer.LecturerImage = filePath;
                }
                lecturer.AccountId = account.AccountId;
                repository.Lecturers.Add(lecturer);
                repository.SaveChanges();
                List<LecturerSubject> lecturerSubjects = new List<LecturerSubject>();
                if (!String.IsNullOrEmpty(request.SubjectId))
                {
                    lecturerSubjects.Add(new LecturerSubject() { SubjectId = request.SubjectId, LecturerId = lecturer.LecturerId });
                }
                foreach (string subjectId in request.ExtraSubjectIds)
                {
                    lecturerSubjects.Add(new LecturerSubject() { SubjectId = subjectId, LecturerId = lecturer.LecturerId });
                }
                repository.LecturerSubjects.AddRange(lecturerSubjects);
                repository.SaveChanges();
                transaction.Commit();
                return (lecturer, "StoreLecturerSuccess");

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, ex.Message);
            }
        }

        public async Task<(Lecturer lecturer, string message)> UpdateAsync(UpdateLecturerRequest request, string lecturerId)
        {
            using var transaction = repository.BeginTransaction();
            try
            {
                Lecturer lecturer = repository.Lecturers.FirstOrDefault(m => m.LecturerId.Equals(lecturerId));
                if (lecturer is null)
                {
                    return (null, "LecturerNotExist");
                }
                Lecturer isEmail = repository.Lecturers.FirstOrDefault(m => m.LecturerEmail.Equals(request.LecturerEmail) && !m.LecturerId.Equals(lecturer.LecturerId));
                if (!(isEmail is null))
                {
                    return (null, "LecturerEmailAreadlyExist");
                }
                Lecturer isPhone = repository.Lecturers.FirstOrDefault(m => m.LecturerPhone.Equals(request.LecturerPhone) && !m.LecturerId.Equals(lecturer.LecturerId));
                if (!(isPhone is null))
                {
                    return (null, "LecturerPhoneAreadlyExist");
                }
                lecturer = mapper.Map(request, lecturer);
                if (!(request.Image is null) && !String.IsNullOrEmpty(lecturer.LecturerImage))
                {
                    (bool, string) deleteImage = GeneralHelper.DeleteFile(lecturer.LecturerImage);
                }
                if (!(request.Image is null))
                {
                    (string filePath, string fileName) = await GeneralHelper.UploadFileProAsync(request.Image, this.pathForder);
                    lecturer.LecturerImage = filePath;
                }

                #region Cập nhật quyền vào bảng Account Permission

                #endregion

                repository.Lecturers.UpdateMaria(lecturer);
                repository.SaveChanges();
                List<string> SubjectIds = repository.LecturerSubjects.FindByCondition(m => m.LecturerId.Equals(lecturerId))
                                                                    !.Select(m => m.SubjectId)
                                                                    .ToList();

                #region Cập nhật môn học kiêm nhiệm

                var chung = request.ExtraSubjectIds.Intersect(SubjectIds);
                List<LecturerSubject> xoa = repository.LecturerSubjects
                                                      .FindByCondition(m => SubjectIds.Except(chung).Contains(m.SubjectId) && m.LecturerId.Equals(lecturerId))
                                                      .ToList();

                repository.LecturerSubjects.RemoveRangeMaria(xoa);
                repository.SaveChanges();
                var them = request.ExtraSubjectIds.Except(chung);
                List<LecturerSubject> lecturerSubjects = new List<LecturerSubject>();
                if (!String.IsNullOrEmpty(request.SubjectId))
                {
                    lecturerSubjects.Add(new LecturerSubject() { SubjectId = request.SubjectId, LecturerId = lecturer.LecturerId });
                }
                foreach (string item in them)
                {
                    lecturerSubjects.Add(new LecturerSubject() { LecturerId = lecturer.LecturerId, SubjectId = item });
                }
                repository.LecturerSubjects.AddRange(lecturerSubjects);
                repository.SaveChanges();

                #endregion
                transaction.Commit();
                return (lecturer, "UpdateLecturerSuccess");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, ex.Message);
            }
        }
    }
}
