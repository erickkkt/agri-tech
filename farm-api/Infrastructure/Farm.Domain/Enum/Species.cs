using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Domain.Enum
{
    public enum Species
    {
        [Description("Deer")]
        Deer = 0,

        [Description("Sheep")]
        Sheep = 1,

        [Description("Cow")]
        Cow = 2,
    }
}
