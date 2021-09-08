/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
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

        //graphics objects for drawing
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        const int BALL_SPEED = 4;
        int ballX, ballY, ballSize;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        int p1X, p1Y, p2X, p2Y, pHeight, pWidth;



        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    Close();
                    break;                   
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            pWidth = 10;    //width for paddles
            pHeight = 40;   //height for paddles

            //p1 starting position
            p1X = PADDLE_EDGE;
            p1Y = this.Height / 2 - pHeight / 2;

            //p2 starting position
            p2X = this.Width - PADDLE_EDGE - pWidth;
            p2Y = this.Height / 2 - pHeight / 2;

            // TODO set Width and Height of ball
            ballSize = 7;


            // TODO set starting X position for ball to middle of screen, (use this.Width and ball.Width)
            ballX = this.Width / 2 - ballSize;
            // TODO set starting Y position for ball to middle of screen, (use this.Height and ball.Height)
            ballY = this.Height / 2 - ballSize;
            Refresh();
            Thread.Sleep(1000);

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight) { ballX += BALL_SPEED; }
            else { ballX -= BALL_SPEED; }
            
            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown) { ballY += BALL_SPEED; }
            else { ballY -= BALL_SPEED; }
            #endregion

            #region update paddle positions

            if (wKeyDown == true && p1Y > 0) { p1Y -= PADDLE_SPEED; }
            if (sKeyDown == true && p1Y < this.Height - pHeight) { p1Y += PADDLE_SPEED; }

            if (upKeyDown == true && p2Y > 0) { p2Y -= PADDLE_SPEED; }
            if (downKeyDown == true && p2Y < this.Height - pHeight) { p2Y += PADDLE_SPEED; }

            #endregion

            #region ball collision with top and bottom lines

            if (ballY < 0) 
            { 
                ballMoveDown = true;
                collisionSound.Play();
            }
            if (ballY > this.Height - ballSize) 
            { 
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            Rectangle p1rectangle = new Rectangle(p1X, p1Y, pWidth, pHeight);
            Rectangle p2rectangle = new Rectangle(p2X, p2Y, pWidth, pHeight);
            Rectangle ballrectangle = new Rectangle(ballX, ballY, ballSize, ballSize);
          
            if (p1rectangle.IntersectsWith(ballrectangle) || p2rectangle.IntersectsWith(ballrectangle))
            {
                ballMoveRight = !ballMoveRight;
                collisionSound.Play();
            }

            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */

            #endregion

            #region ball collision with side walls (point scored)

            if (ballX < 0)  // ball hits left wall logic
            {
                // TODO
                // --- play score sound
                // --- update player 2 score
                player2Score++;
                scoreSound.Play();
                // TODO use if statement to check to see if player 2 has won the game. If true run 
                if (player2Score == gameWinScore) { GameOver("Player 2"); }
                else 
                { 
                    ballMoveRight = true;
                    SetParameters();
                }
                // GameOver method. Else change direction of ball and call SetParameters method.
            }

            if (ballX > this.Width - ballSize)  // ball hits right wall logic
            {
                // TODO
                // --- play score sound
                // --- update player 1 score
                player1Score++;
                scoreSound.Play();
                // TODO use if statement to check to see if player 2 has won the game. If true run 
                if (player1Score == gameWinScore) { GameOver("Player 1"); }
                else 
                { 
                    ballMoveRight = false;
                    SetParameters();
                }
                // GameOver method. Else change direction of ball and call SetParameters method.
            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }    
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        private void GameOver(string winner)
        {            
            // TODO create game over logic
            // --- stop the gameUpdateLoop
            // --- show a message on the startLabel to indicate a winner, (need to Refresh).
            // --- pause for two seconds 
            // --- use the startLabel to ask the user if they want to play again
            gameUpdateLoop.Enabled = false;
            startLabel.Visible = true;
            startLabel.Text = $"{winner} Wins";
            Refresh();
            Thread.Sleep(2000);          
            startLabel.Text = $"Press Space to play again \n Press Escape to exit";
            Refresh();
            Thread.Sleep(1000);
            newGameOk = true;




        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw paddles using FillRectangle
            e.Graphics.FillRectangle(drawBrush, p1X, p1Y, pWidth, pHeight);
            e.Graphics.FillRectangle(drawBrush, p2X, p2Y, pWidth, pHeight);
            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(drawBrush, ballX, ballY, ballSize, ballSize);
            // TODO draw scores to the screen using DrawString
            e.Graphics.DrawString($"{player1Score} - {player2Score}", drawFont, drawBrush, this.Width / 2 - 15, 3);
        }

    }
}
