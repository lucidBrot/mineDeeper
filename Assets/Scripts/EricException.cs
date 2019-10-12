using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class EricException : Exception
    {
        public override string Message => "It's Eric's fault...";
    }
}
