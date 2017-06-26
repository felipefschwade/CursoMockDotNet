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
        private Mock<Carteiro> carteiro;

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
            carteiro = new Mock<Carteiro>();
        }
        [TestMethod]
        public void DeveEncerarLeiloesComMaisDeUmaSemana()
        {
            List<Leilao> retorno = new List<Leilao>() {
                leilao,
                leilao2
            };

            dao.Setup(m => m.correntes()).Returns(retorno);

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
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

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(1, encerrador.total);
            Assert.IsFalse(leilao3.encerrado);
            Assert.IsTrue(leilao.encerrado);
        }
        [TestMethod]
        public void NaoDeveFazerNadaCasoNaoHajaLeiloes()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>());
            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
        }

        [TestMethod]
        public void DeveChamarOAtualiza()
        {
            List<Leilao> retorno = new List<Leilao>() {
                leilao,
                leilao2,
                leilao3
            };

            dao.Setup(m => m.correntes()).Returns(retorno);

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(m => m.atualiza(leilao), Times.Once());
            dao.Verify(m => m.atualiza(leilao2), Times.Once());
            dao.Verify(m => m.atualiza(leilao3), Times.Never());
        }
        
        [TestMethod]
        public void NaoDeveChamarOAtualizaNenhumaVez()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>() { leilao3 });

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(m => m.atualiza(leilao3), Times.Never());
        }

        [TestMethod]
        public void DeveContinuarMesmoComExcecoes()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>() { leilao, leilao2, leilao3 });
            dao.Setup(m => m.atualiza(leilao)).Throws(new Exception());

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            carteiro.Verify(m => m.envia(leilao), Times.Never());
            dao.Verify(m => m.atualiza(leilao2), Times.Once());
            carteiro.Verify(m => m.envia(leilao2), Times.Once());

        }

        [TestMethod]
        public void DeveContinuarMesmoComExcecoesDoCarteiro()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>() { leilao, leilao2, leilao3 });
            carteiro.Setup(m => m.envia(leilao)).Throws(new Exception());

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            carteiro.Verify(m => m.envia(leilao), Times.Once());
            dao.Verify(m => m.atualiza(leilao), Times.Once());
            dao.Verify(m => m.atualiza(leilao2), Times.Once());
            carteiro.Verify(m => m.envia(leilao2), Times.Once());

        }

        [TestMethod]
        public void NaoDeveChamarOCarteiroCasoOcorraExcecaoEmTodos()
        {
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>() { leilao, leilao2, leilao3 });
            dao.Setup(m => m.atualiza(leilao)).Throws(new Exception());
            dao.Setup(m => m.atualiza(leilao2)).Throws(new Exception());

            var encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            carteiro.Verify(c => c.envia(It.IsAny<Leilao>()), Times.Never());
        }
    }
}
