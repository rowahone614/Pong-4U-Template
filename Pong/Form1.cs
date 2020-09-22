/*
 * Description:     A basic PONG simulator
 * Author:       Rowan Honeywell        
 * Date:         09/21/20  
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values
        //Brush Declaration
        SolidBrush Gray = new SolidBrush(Color.Gray);
        SolidBrush greenBrush = new SolidBrush(Color.OliveDrab);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 10);
        //Sound Declaration
        SoundPlayer paddleSound = new SoundPlayer(Properties.Resources.paddle);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);
        SoundPlayer winSound = new SoundPlayer(Properties.Resources.win_sound);
        SoundPlayer pointSound = new SoundPlayer(Properties.Resources.point_sound);
        //Boolean Declaration
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;
        Boolean newGameOk = true;
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        //Ball Characteristics
        int ballSpeed = 4;
        Rectangle ball;
        //Paddle Characteristics
        int paddleSpeed = 4;
        Rectangle p1, p2;
        //Integer Declaration
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 5;
        int hitNumber = 0;
        //Random Declaration
        Random colourGen = new Random();
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        startLabel.Text = "3";
                        startLabel.Refresh();
                        Thread.Sleep(1000);
                        startLabel.Text = "2";
                        startLabel.Refresh();
                        Thread.Sleep(1000);
                        startLabel.Text = "1";
                        startLabel.Refresh();
                        Thread.Sleep(1000);
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            const int PADDLE_EDGE = 20;
            ballSpeed = 4;
            paddleSpeed = 4;
            hitNumber = 0;
            p1.Width = p2.Width = 10;
            p1.Height = p2.Height = 40;

            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            ball.Height = ball.Width = 5;
            ball.X = this.Width / 2 + ball.Width / 2;
            ball.Y = this.Height / 2 + ball.Height / 2;
        }

        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            if (ballMoveRight == true)
            {
                ball.X = ball.X + ballSpeed;
            }
            else
            {
                ball.X = ball.X - ballSpeed;
            }

            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + ballSpeed;
            }
            if (ballMoveDown == false)
            {
                ball.Y = ball.Y - ballSpeed;
            }
            #endregion

            #region update paddle positions

            if (aKeyDown == true && p1.Y > 0)
            {
                p1.Y = p1.Y - paddleSpeed;
            }
            else if (zKeyDown == true && p1.Y <= (this.Height - p1.Height))
            {
                p1.Y = p1.Y + paddleSpeed;
            }
            if (jKeyDown == true && p2.Y > 0)
            {
                p2.Y = p2.Y - paddleSpeed;
            }
            else if (mKeyDown == true && p2.Y <= (this.Height - p2.Height))
            {
                p2.Y = p2.Y + paddleSpeed;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y <= 0)
            {
                ballMoveDown = true;
                collisionSound.Play();
            }
            else if (ball.Y >= (this.Height - ball.Height))
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if (ball.IntersectsWith(p1))
            {
                ballMoveRight = true;
                paddleSound.Play();
                ballSpeed++;
                paddleSpeed++;
                hitNumber++;
            }
            else if (ball.IntersectsWith(p2))
            {
                ballMoveRight = false;
                paddleSound.Play();
                ballSpeed++;
                paddleSpeed++;
                hitNumber++;
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)
            {
                pointSound.Play();
                player2Score++;
                Thread.Sleep(750);

                if (player2Score == gameWinScore)
                {
                    winSound.Play();
                    GameOver("Player Two");
                }
                else
                {
                    SetParameters();
                }

            }

            if (ball.X > this.Width)
            {
                pointSound.Play();
                player1Score++;
                Thread.Sleep(750);

                if (player1Score == gameWinScore)
                {
                    winSound.Play();
                    GameOver("Player One");
                }
                else
                {
                    SetParameters();
                }

            }
            #endregion

            this.Refresh();
        }

        private void GameOver(string winner)
        {
            newGameOk = true;

            gameUpdateLoop.Stop();
            startLabel.Visible = true;
            startLabel.Text = winner + " is the winner!";
            startLabel.Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Would you like to play again? Yes: Space - No: N";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(redBrush, p1);
            e.Graphics.FillRectangle(blueBrush, p2);
            e.Graphics.FillRectangle(Gray, (this.Width / 2) - 1, 0, 2, this.Height);
            int r = colourGen.Next(0, 256);
            int g = colourGen.Next(0, 256);
            int b = colourGen.Next(0, 256);
            Color ballColour = Color.FromArgb(r, g, b);
            SolidBrush ballBrush = new SolidBrush(ballColour);
            e.Graphics.FillRectangle(ballBrush, ball);
            e.Graphics.DrawString("Player One:" + player1Score, drawFont, redBrush, 20, 20);
            e.Graphics.DrawString("Player Two:" + player2Score, drawFont, blueBrush, this.Width - 125, 20);
            e.Graphics.DrawString("Hits:" + hitNumber, drawFont, greenBrush, this.Width / 2 - 23, this.Height - 20);
        }

    }
}
