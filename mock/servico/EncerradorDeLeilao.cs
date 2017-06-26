﻿using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class EncerradorDeLeilao
    {
        public int total { get; private set; }
        public RepositorioDeLeiloes Dao { get; private set; }
        private Carteiro Carteiro;

        public EncerradorDeLeilao(RepositorioDeLeiloes dao, Carteiro carteiro)
        {
            total = 0;
            Dao = dao;
            Carteiro = carteiro;
        }

        public virtual void encerra()
        {
            List<Leilao> todosLeiloesCorrentes = Dao.correntes();

            foreach (var l in todosLeiloesCorrentes)
            {

                if (comecouSemanaPassada(l))
                {
                    try
                    {
                        l.encerra();
                        total++;
                        Dao.atualiza(l);
                        Carteiro.envia(l);
                    }
                    catch (Exception e)
                    {
                        //log
                    }
                }
            }
        }


        private bool comecouSemanaPassada(Leilao leilao)
        {

            return diasEntre(leilao.Data, DateTime.Now) >= 7;

        }

        private int diasEntre(DateTime inicio, DateTime fim)
        {
            int dias = (int)(fim - inicio).TotalDays;

            return dias;
        }

    }
}
