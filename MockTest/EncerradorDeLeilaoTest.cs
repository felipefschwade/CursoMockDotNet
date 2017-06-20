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
    public class EncerradorDeLeilaoTest
    {
        private Leilao leilao;
        private Leilao leilao2;
        private Leilao leilao3;
        private Mock<RepositorioDeLeiloes> dao;

        [TestInitialize()]
        public void TestInitialize()
        {
            leilao = new Leilao("TV 21 pol");
            leilao.naData(new DateTime(2017, 06, 11));
            leilao2 = new Leilao("Play 4");
            leilao2.naData(new DateTime(2017, 06, 10));
            leilao3 = new Leilao("XBox");
            leilao3.naData(DateTime.Now);
            dao = new Mock<RepositorioDeLeiloes>();
        }
        [TestMethod]
        public void DeveEncerarLeiloesComMaisDeUmaSemana()
        {
            List<Leilao> retorno = new List<Leilao>() {
                leilao,
                leilao2
            };

            dao.Setup(m => m.correntes()).Returns(retorno);

            var encerrador = new EncerradorDeLeilao(dao.Object);
            encerrador.encerra();

            Assert.AreEqual(2, encerrador.total);
            Assert.IsTrue(leilao.encerrado);
            Assert.IsTrue(leilao2.encerrado);
        }
        [TestMethod]
        public void NaoDeveEncerrarLeilaoDeHoje()
        {
            List<Leilao> retorno = new List<Leilao>() {
                leilao3,
                leilao
            };

            dao.Setup(m => m.correntes()).Returns(retorno);

            var encerrador = new EncerradorDeLeilao(dao.Object);
            encerrador.encerra();

            Assert.AreEqual(1, encerrador.total);
            Assert.IsFalse(leilao3.encerrado);
            Assert.IsTrue(leilao.encerrado);
        }
        [TestMethod]
        public void NaoDeveFazerNadaCasoNaoHajaLeiloes()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>());
            var encerrador = new EncerradorDeLeilao(dao.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
        }
    }
}
