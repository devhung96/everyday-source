using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeClarations.Entites
{
    public enum DeClaStatus
    {
        confirm = 1,
        unconfimred = 0,
    }
    [Table("wh_declaration")]
    public class DeClaration
    {
        [Key]
        [Column("de_number")]
        public string DeClaNumber { get; set; }
        [Column("de_date_re")]
        [JsonIgnore]
        public DateTime DeClaDateReData { get; set; }
        /// <summary>
        /// 1: nhập khẩu
        /// 2: xuất khẩu
        /// </summary>
        [Column("de_type")]
        public int DeClaType { get; set; }
        [EnumDataType(typeof(DeClaStatus))]
        [Column("de_status")]
        public DeClaStatus DeClaStatus { get; set; }
        [Column("de_content")]
        public string DeClaContent { get; set; }
        [Column("de_parent_number")]
        public string DeClaParentNumber { get; set; }
        [NotMapped]
        public JObject Content { get { return JObject.Parse(this.DeClaContent); } }
        public List<DeClarationDetail> DeClarationDetails { get; set; }
        [Column("de_extended_dispatch")]
        public string DeClaExtendedDispatch { get; set; }
        [Column("de_extended_dispatch_date")]
        public DateTime? DeClaExtendedDispatchDate { get; set; }
        [Column("de_settlement_dispatch")]
        public string DeClaSettlementDispatch { get; set; }
        [Column("de_renewal_date")]
        public DateTime? DeClaRenewalDate { get; set; }
        [Column("de_settlement_status")]
        public int DeClaSettlementStatus { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [NotMapped]
        public string DeClaDateRe { get { return this.DeClaDateReData.ToString("dd/MM/yyyy"); } }
        [Column("de_new_date")]
        public DateTime? DeClaNewDate { get; set; }
        [NotMapped]
        public string DeClaNewDateV2 { get { if (this.DeClaNewDate.HasValue) return this.DeClaNewDate.Value.ToString("dd/MM/yyyy"); return null; } }
    }
}
