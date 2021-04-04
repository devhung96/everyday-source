using Project.Modules.Parameters.Entities;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Parameters.Requests
{
    public class AddParameterRequest
    {
        [Required]
        public string ParameterKey { get; set; }
        [Required]
        public TYPE_PARAMS ParameterType { get; set; }
        [Required]
        public string ParameterName { get; set; }
        [Required]
        public object ParameterValue { get; set; }
    }
}
