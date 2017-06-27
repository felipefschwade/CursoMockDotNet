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
        private Usuario joao;
        private Usuario maria;
        private Usuario guilherme;
        private Leilao leilao2;
        private Leilao leilao3;
        private Mock<RepositorioDeLeiloes> dao;
        private Mock<Carteiro> carteiro;

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
