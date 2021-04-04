using Newtonsoft.Json;
using Project.Modules.Declarations.Models;
using Project.Modules.Declarations.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_cityPair")]
    public class Citypair
    {
        [Key]
        [Column("citypair_id")]
        public int Id { get; set; }
        [Column("order")]
        public int Order { get; set; }
        [Column("citypair_route")]
        public string Route { get; set; }
        [Column("citypair_status")]
        public CityPairStatus Status { get; set; } = CityPairStatus.STARTING;
        [Column("citypair_date_start")]
        public DateTime DateStartObj { get; set; }
        [Column("citypair_schedule")]
        public string Schedule { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        public List<Seal> Seals { get; set; } = new List<Seal>();
        public CopySeal CopySeal { get; set; }

        [NotMapped]
        public List<CityPairSchedule> CityPairSchedules {
            get 
            {
                if (Schedule is null)
                {
                    return new List<CityPairSchedule>();
                }
                return JsonConvert.DeserializeObject<List<CityPairSchedule>>(Schedule);
            } 
            set
            {
                CityPairSchedules = value;
            }
        }
    }

    public enum CityPairStatus
    {
        STARTING = 1,
        STOPPED = 0,
        DEFAULT = -1
    }
}