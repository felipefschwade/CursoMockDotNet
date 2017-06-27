using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class GeradorDePagamento
    {
        private LeilaoDaoFalso leilaoDao;
        private PagamentoDAO pagamentoDao;
        private Avaliador avaliador;

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao, Avaliador avaliador, PagamentoDAO pagamentoDao)
        {
            this.leilaoDao = leilaoDao;
            this.avaliador = avaliador;
            this.pagamentoDao = pagamentoDao;
        }

        public virtual void gera()
        {
            List<Leilao> encerrados = leilaoDao.encerrados();
            foreach (var l in encerrados)
            {
                this.avaliador.avalia(l);

                Pagamento pagamento = new Pagamento(this.avaliador.maiorValor, ProximoDiaUtil());

                this.pagamentoDao.Salva(pagamento);
            }
        }

        private DateTime ProximoDiaUtil()
        {
            var hoje = new RelogioDoSistema().Hoje();
            if (hoje.DayOfWeek == DayOfWeek.Saturday) return DateTime.Today.AddDays(2);
            if (hoje.DayOfWeek == DayOfWeek.Saturday) return DateTime.Today.AddDays(1);
            return hoje;
        }
    }
}
