using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Entities
{
    [Table("wh_menus")]
    public class Menu
    {

        [Key]
        [Column("menu_id")]
        public int MenuID { get; set; }
        [Column("menu_name")]
        public string MenuName { get; set; }
        [Column("menu_start_time")]
        public DateTime MenuStart { get; set; } = DateTime.Now;
        [Column("menu_stop_time")]
        public DateTime MenuStop { get; set; } = DateTime.Today.AddYears(5);
    }
}
