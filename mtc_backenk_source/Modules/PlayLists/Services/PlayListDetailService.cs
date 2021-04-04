using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.PlayLists.Entities;
using Project.Modules.PlayLists.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Services
{
    public interface IPlayListDetailService
    {
        (PlayListDetail data, string message) Store(StorePlayListDetailRequest value);
        (PlayListDetail data, string message) FindID(string playListDetailId);
        (PlayListDetail data, string message) Delete(string playListDetailId);
        (PlayListDetail data, string message) Edit(PlayListDetail data, UpdatePlayListDetailRequest valueUpdate);
        (PlayListDetail data, string message) EditOrder(string playListDetailId1, string playListDetailId2);
        (IQueryable<PlayListDetail> data, string message) ShowAll();
        (IQueryable<PlayListDetail> data, string message) ShowByPlaylistId(string playListId);


    }
    public class PlayListDetailService : IPlayListDetailService
    {
        private readonly IRepositoryWrapperMariaDB _context;
        private readonly IMapper mapper;
        public PlayListDetailService(IRepositoryWrapperMariaDB context, IMapper _mapper)
        {
            _context = context;
            mapper = _mapper;
        }
        public (PlayListDetail data, string message) Delete(string playListDetailId)
        {
            (PlayListDetail _data, string _message) playListDetail = FindID(playListDetailId);
            if (playListDetail._data == null)
                return (null, playListDetail._message);
            //playListDetail._data.playListDetailStatus = PlayListDetailStatusEnum.DELETED;
            _context.PlayListDetails.Remove(playListDetail._data);
            _context.SaveChanges();
            return (playListDetail._data, playListDetail._message);
        }

        public (PlayListDetail data, string message) Edit(PlayListDetail data, UpdatePlayListDetailRequest valueUpdate)
        { 
            try
            {
                data = mapper.Map(valueUpdate, data);
                _context.PlayListDetails.Update(data);
                _context.SaveChanges();
                return (data, "UpdatePlayListDetailSuccess");
            }
            catch (Exception e)
            {
                return (null, e.InnerException.Message);
            }
        }

        public (PlayListDetail data, string message) EditOrder(string playListDetailId1, string playListDetailId2)
        {
            (PlayListDetail orderDetail1, string message1) = FindID(playListDetailId1);
            (PlayListDetail orderDetail2, string message2) = FindID(playListDetailId2);
            if (orderDetail1 == null || orderDetail2 == null)
                return (null, message1);
            int order1 = orderDetail1.OrderMedia;
            int order2 = orderDetail2.OrderMedia;
            orderDetail1.OrderMedia = order2;
            orderDetail2.OrderMedia = order1;
            _context.SaveChanges();
            return (orderDetail1,"UpdateOrderMediaSuccess");
        }

        public (PlayListDetail data, string message) FindID(string playListDetailId)
        {
            PlayListDetail playListDetail = _context.PlayListDetails.FindAll().Include(x => x.PlayList).Include(x=> x.Template).FirstOrDefault(x=> x.PlayListDetailId == playListDetailId);
            if (playListDetail == null)
                return (null, "PlayListDetailIdCanNotFound");
            return (playListDetail, "ShowPlayListDetailSuccess");
        }

        public (IQueryable<PlayListDetail> data, string message) ShowAll()
        {
            IQueryable<PlayListDetail> playListDetails = _context.PlayListDetails.FindAll().Include(x => x.PlayList).Include(x => x.Template).OrderByDescending(x => x.CreatedAt);
            return (playListDetails, "ShowAllSuccess");
        }

        public (IQueryable<PlayListDetail> data, string message) ShowByPlaylistId(string playListId)
        {
            PlayList playList = _context.PlayLists.FindByCondition(x => x.PlayListId.Equals(playListId)).FirstOrDefault();
            if (playList == null)
                return (null, "PlayListDetailIdCanNotFound");
            IQueryable<PlayListDetail> playListDetails = _context.PlayListDetails.FindByCondition(x => x.PlayListId == playListId).Include(x => x.PlayList).Include(x => x.Template).OrderBy(x => x.TimeBegin);
            return (playListDetails, "ShowAllSuccess");
        }

        public (PlayListDetail data, string message) Store(StorePlayListDetailRequest value)
        {
            try
            {
                PlayListDetail orderNewPlaylistDetail = _context.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(value.PlayListId)).ToList().OrderBy(x=>x.OrderMedia).LastOrDefault();
                value.OrderMedia = 0;
                if (orderNewPlaylistDetail != null)
                {
                    orderNewPlaylistDetail.OrderMedia = orderNewPlaylistDetail.OrderMedia + 1;
                }
                else
                {
                    value.OrderMedia = 1;
                };
                PlayListDetail data = mapper.Map<PlayListDetail>(value);
                _context.PlayListDetails.Add(data);
                _context.SaveChanges();
                return (data, "StorePlayListDetailSuccess");
            }
            catch (Exception e)
            {
                return (null, e.InnerException.Message);
            }

        }
    }
}
