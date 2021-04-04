using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class RegisterFace
    {
        public string Record { get; set; }
        public bool Result { get; set; }
        public double Score { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int Mask { get; set; }
        public string Delete { get; set; }
        public string FullPath { get; set; }
    }
}
