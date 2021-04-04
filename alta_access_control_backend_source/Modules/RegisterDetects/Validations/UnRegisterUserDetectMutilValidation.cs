using FluentValidation;
using Project.Modules.RegisterDetects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Validations
{
    public class UnRegisterUserDetectMutilValidation : AbstractValidator<UnRegisterUserDetectMutilRequest>
    {
        public UnRegisterUserDetectMutilValidation()
        {
        }
    }
}
