using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreLab.InterpreterModels
{
    interface IResolvable
    {
        string ResolveTemplate(object target);
    }
}
