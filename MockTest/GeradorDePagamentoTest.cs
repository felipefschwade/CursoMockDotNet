using Microsoft.VisualStudio.TestTools.UnitTesting;
using mock.dominio;
using mock.infra;
using mock.servico;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTest
{
    [TestClass]
    public class GeradorDePagamentoTest
    {
        private Leilao leilao;
        private Usuario joao;
        private Usuario maria;
        private Usuario guilherme;
        private Leilao leilao2;
        private Leilao leilao3;
        private Mock<PagamentoDAO> dao;
        private Mock<Carteiro> carteiro;
        private Mock<LeilaoDaoFalso> leilaoDao;
        private Avaliador avaliador;

        [TestInitialize()]
        public void TestInitialize()
        {
            joao = new Usuario(1, "João");
            maria = new Usuario(1, "Maria");
            guilherme = new Usuario(1, "Guilherme");

            leilao = new LeilaoBuilder("Xbox 360")
                .Lance(joao, 300)
                .Lance(maria, 400)
                .Lance(guilherme, 600)
                .NaData(new DateTime(2017, 06, 11))
                .Build();

            leilao2 = new LeilaoBuilder("Play 4")
                .Lance(joao, 600)
                .Lance(maria, 250)
                .Lance(guilherme, 400)
                .NaData(DateTime.Now.AddDays(-7))
                .Build();

            leilao3 = new LeilaoBuilder("Tv 42")
                .Lance(joao, 550)
                .Lance(maria, 650)
                .Lance(guilherme, 700)
                .NaData(DateTime.Now)
                .Build();

            dao = new Mock<PagamentoDAO>();
            carteiro = new Mock<Carteiro>();
            leilaoDao = new Mock<LeilaoDaoFalso>();
            avaliador = new Avaliador();
        }

        [TestMethod]
        public void DeveTerUmPagamentoIgualOMaiorLance()
        {
            leilaoDao.Setup(m => m.encerrados()).Returns(new List<Leilao>() { leilao });
            
            Pagamento retorno = null;

            dao.Setup(p => p.Salva(It.IsAny<Pagamento>())).Callback<Pagamento>(r => retorno = r);

            var gerador = new GeradorDePagamento(leilaoDao.Object, avaliador, dao.Object);
            gerador.gera();

            Assert.AreEqual(600, retorno.valor);
        }
    }
}
