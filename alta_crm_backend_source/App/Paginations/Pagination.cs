﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Project.App.Paginations
{
    public class PaginationV2Request
    {
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageSizeGreaterThanOrEqual1")]
        public int PageSize { get; set; } = int.MaxValue / 2;
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageNumberGreaterThanOrEqual1")]
        public int PageNumber { get; set; } = 1;
        public string OrderByQuery { get; set; } //Sample: "customerName desc, customerBirthday"
        public string SearchContent { get; set; }
    }

    public class TypeInt32MinValueValidationAttribute : ValidationAttribute
    {
        private readonly int MinValue;
        public TypeInt32MinValueValidationAttribute(int minValue)
        {
            MinValue = minValue;
        }
        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) >= MinValue;
        }
    }

    public class PaginationRes<T>
    {
        public IEnumerable<T> PagedData { get; set; }
        public PageInfo PageInfo { get; set; }

        public PaginationRes(IEnumerable<T> items, PageInfo pageInfo)
        {
            PagedData = items;
            PageInfo = pageInfo;
        }
    }


    public class PageInfo
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }

        public PageInfo() { }

        public PageInfo(int totalCount, int pageSize, int currentPage, int totalPages, bool hasNext, bool hasPrevious)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            HasNext = hasNext;
            HasPrevious = hasPrevious;
        }
    }

    public class Pagination<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
        public PageInfo PageInfo { get; private set; }

        public Pagination(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageInfo = new PageInfo(TotalCount, PageSize, CurrentPage, TotalPages, HasNext, HasPrevious);

            AddRange(items);
        }

        public static Pagination<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new Pagination<T>(items, source.Count(), pageNumber, pageSize);
        }

        public static Pagination<T> ToPagedList(IFindFluent<T, T> source, int pageNumber, int pageSize)
        {
            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();
            int countData = (int)source.Count();
            stopwatch.Stop();
            Console.WriteLine("Time elapsed count: {0}", stopwatch.Elapsed);
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Limit(pageSize).ToList();
            return new Pagination<T>(items, countData, pageNumber, pageSize);
        }
        public static Pagination<T> ToPagedListIEnum(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new Pagination<T>(items, source.Count(), pageNumber, pageSize);
        }

       
    }
}
