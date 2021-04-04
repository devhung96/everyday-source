using AutoMapper;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.App.Services;
using Project.Modules.Slots.Entities;
using Project.Modules.Slots.Requests;
using Project.Modules.Slots.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Services
{
    public interface ISlotService : IService<Slot>
    {
        Slot Store(StoreSlotRequest request);
        Slot Update(UpdateSlotRequest request, Slot slot);
        List<Slot> GetAll();
        ResponseTable Search(SearchSlotRequest request);
    }
    public class SlotService : ISlotService
    {
        private readonly IRepositoryMariaWrapper _repositoryWrapper;
        private readonly IMapper Mapper;

        public SlotService(IRepositoryMariaWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public void Delete(Slot slot)
        {
            _repositoryWrapper.Slots.RemoveMaria(slot);
            _repositoryWrapper.SaveChanges();
        }

        public List<Slot> GetAll()
        {
            return _repositoryWrapper.Slots.FindAll().ToList();
        }

        public Slot GetById(string slotId)
        {
            Slot slot = _repositoryWrapper.Slots.GetById(slotId);
           
            //GetSlotResponse slotResponse = new GetSlotResponse()
            //{
            //    SlotId = slot.SlotId,
            //    SlotStartAt = slot.SlotStartAt.ToString(),
            //    SlotEndAt = slot.SlotEndAt.ToString()
            //};
            return slot;
        }

        public ResponseTable Search(SearchSlotRequest request)
        {
            var querySearch = _repositoryWrapper.Slots.FindAll();
            if (!string.IsNullOrEmpty(request.Search))
            {
                querySearch = querySearch.Where(s => s.SlotName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(request.SortField) || !string.IsNullOrEmpty(request.SortOrder))
            {
                //var query = request.SortField + " " + request.SortOrder;
                querySearch = querySearch.OrderBy(request.SortField + " " + request.SortOrder);
            }
            return new ResponseTable
            {
                Data = request.Page != 0 ? querySearch.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList() : querySearch.ToList(),
                Info = new Info
                {
                    Page = request.Page != 0 ? request.Page : 1,
                    Limit = request.Page != 0 ? request.Limit : querySearch.Count(),
                    TotalRecord = querySearch.Count(),
                }
            };
        }

        public Slot Store(StoreSlotRequest request)
        {
            Slot slot = new Slot() {
                SlotName = request.SlotName,
                SlotStartAt = TimeSpan.Parse(request.SlotStartAt),
                SlotEndAt = TimeSpan.Parse(request.SlotEndAt)
            };
            _repositoryWrapper.Slots.Add(slot);
            _repositoryWrapper.SaveChanges();
            return slot;
        }

        public Slot Update(UpdateSlotRequest request, Slot slot)
        {
            slot.SlotName = request.SlotName;
            slot.SlotStartAt = TimeSpan.Parse(request.SlotStartAt);
            slot.SlotEndAt = TimeSpan.Parse(request.SlotEndAt);
            _repositoryWrapper.Slots.UpdateMaria(slot);
            _repositoryWrapper.SaveChanges();
            //GetSlotResponse slotResponse = new GetSlotResponse()
            //{
            //    SlotId = slot.SlotId,
            //    SlotStartAt = slot.SlotStartAt.ToString(),
            //    SlotEndAt = slot.SlotEndAt.ToString()
            //};       
            return slot;
        }
    }
}
