using mock.dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTest
{
    public class LeilaoBuilder
    {
        private Leilao leilao;

        public LeilaoBuilder(string descricao)
        {
            this.leilao = new Leilao(descricao);
        }

        public LeilaoBuilder NaData(DateTime data)
        {
            this.leilao.naData(data);
            return this;
        }

        public LeilaoBuilder Lance(Usuario user, double valor)
        {
            this.leilao.propoe(new Lance(user, valor));
            return this;
        }

        public Leilao Build()
        {
            return leilao;
        }
    }
}
