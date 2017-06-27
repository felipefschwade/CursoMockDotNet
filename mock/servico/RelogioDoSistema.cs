using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    class RelogioDoSistema : Relogio
    {
        public DateTime Hoje()
        {
            return DateTime.Today;
        }
    }
}
