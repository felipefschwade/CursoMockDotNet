using mock.dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.infra
{
    public interface RepositorioDeLeiloes
    {
        void salva(Leilao leilao);
        List<Leilao> encerrados();
        List<Leilao> correntes();
        void atualiza(Leilao leilao);
    }
}
