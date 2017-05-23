﻿using Bingo.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bingo
{
    public partial class Bingo : Form
    {
        public Bingo()
        {
            InitializeComponent();
        }

        private void btn_entraJogo_Click(object sender, EventArgs e)
        {
            Conf_Player confPlayer = new Conf_Player();
            confPlayer.ip = textBox_ip.Text;
            confPlayer.porta = Int32.Parse(textBox_porta.Text);
            confPlayer.nome = textBox_jogador.Text;
            conectarJogo(confPlayer);
        }

        private void conectarJogo(Conf_Player conf)
        {

            //Criar método de conectar

            if (true)
            {
                Jogo jogo = new Jogo();
                jogo.Show();
            }
            else
            {
                MessageBox.Show("Erro ao entra no jogo.", "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}
