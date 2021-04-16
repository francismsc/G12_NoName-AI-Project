using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System.Collections.Generic;


namespace NoName
{

    public class NoName : AbstractThinker
    {



        private int maxDepth;
        private int numEvals;

        public const int defaultMaxDepth = 2;
        private System.Random random;


        public override void Setup(string str)
        {
            // Try to get the maximum depth from the parameters
            if (!int.TryParse(str, out maxDepth))
            {
                // If not possible, set it to the default
                maxDepth = defaultMaxDepth;
            }

            // If a non-positive integer was provided, reset it to the default
            if (maxDepth < 1) maxDepth = defaultMaxDepth;
        }

        public override string ToString()
        {
            return "G12" + base.ToString() + "_v2";
        }
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            numEvals = 0;
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                Negamax(board, ct, board.Turn, board.Turn, 0, float.NegativeInfinity, float.PositiveInfinity);



            // Return best move
            return decision.move;
        }

        private (FutureMove move, float score) Negamax(
            Board board, CancellationToken ct,
            PColor player, PColor turn, int depth, float alpha, float beta)
        {
            // Move to return and its heuristic value
            (FutureMove move, float score) selectedMove;

            // Current board state
            Winner winner;
            numEvals++;

            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                selectedMove = (FutureMove.NoMove, float.NaN);
            }

            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player.Other())
                {
                    selectedMove = (FutureMove.NoMove, -float.PositiveInfinity);

                }
                else if (winner.ToPColor() == player)
                {
                    selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
                }
                else
                {
                    selectedMove = (FutureMove.NoMove, 0f);
                }

            }
            else if (depth == maxDepth)
            {
                selectedMove = (FutureMove.NoMove, Heuristic(board, player));
            }
            else
            {
                selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                for (int i = 0; i < Cols; i++)
                {
                    if (board.IsColumnFull(i)) continue;

                    for (int j = 0; j < 2; j++)
                    {
                        PShape shape = (PShape)j;

                        float eval;

                        if (board.PieceCount(turn, shape) == 0)
                        {

                            continue;
                        }

                        board.DoMove(shape, i);
                        eval = -Negamax(board, ct, player.Other(), turn.Other(), depth + 1, -beta, -alpha).score;
                        board.UndoMove();

                        if (eval > selectedMove.score)
                        {

                            selectedMove = (new FutureMove(i, shape), eval);
                        }

                        if (eval > alpha)
                        {
                            alpha = eval;
                        }

                        if (alpha >= beta)
                        {
                            return selectedMove;
                        }
                    }
                }
            }


            return selectedMove;
        }
        private float Heuristic(Board board, PColor color)
        {

            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float)Math.Sqrt((Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            float vinaRow = 0;
            float vinaRowS = 0;
            float hinaRow = 0;
            float hinaRowS = 0;

            float sVinaRow = 0;
            float sVinaRowS = 0;
            float sHinaRow = 0;
            float sHinaRowS = 0;

            float dinaRow = 0;
            float dinaRowS = 0;
            float sDinaRow = 0;
            float sDinaRowS = 0;

            float d2inaRow = 0;
            float d2inaRowS = 0;
            float sD2inaRow = 0;
            float sD2inaRowS = 0;

            // Current heuristic value
            float h = 0;


            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {

                vinaRow = 0;
                vinaRowS = 0;
                sVinaRow = 0;
                sVinaRowS = 0;

                for (int j = 0; j < board.cols; j++)
                {
                    Piece? piece = board[i, j];


                    if (piece.HasValue)
                    {
                        //Give points to pieces that are of our color depending on 
                        //where it is in the board (more to the center = more points)
                        if (piece.Value.color == color)
                        {
                            h += 1 / (0.01f + Dist(centerRow, centerCol, i, j)) * 20;


                        }
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                        {

                            h -= 1 / (0.01f + Dist(centerRow, centerCol, i, j)) * 20;

                        }
                        //Give points to pieces that are of our Shape depending on
                        //where it is in the board (more to the center = more points)

                        if (piece.Value.shape == color.Shape())
                        {
                            h += 1 / (0.01f + Dist(centerRow, centerCol, i, j)) * 20;


                        }
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                        {
                            h -= 1 / (0.01f + Dist(centerRow, centerCol, i, j)) * 20;


                        }

                        //Count amount of vertical pieces of our color in a row and 
                        //amount of spaces(including of our color) in a row and give it points per amount in a row
                        if (board[i, j].Value.color == color)
                        {
                            vinaRow++;
                            vinaRowS++;

                        }
                        else
                        {
                            //Give it points if there is enough space to fill our needed sequence
                            if (vinaRowS >= board.piecesInSequence)
                            {
                                h += vinaRowS;

                            }
                            vinaRow = 0;
                            vinaRowS = 0;

                        }

                        //Count amount of vertical pieces of our shape in a row and 
                        //amount of spaces(including of our shape) in a row and give it points per amount in a row
                        if (board[i, j].Value.shape == color.Shape())
                        {
                            sVinaRow++;
                            sVinaRowS++;

                        }
                        else
                        {
                            //Give it points if there is enough space to fill our needed sequence
                            if (sVinaRowS >= board.piecesInSequence)
                            {
                                h += sVinaRowS;

                            }

                            sVinaRow = 0;
                            sVinaRowS = 0;

                        }

                    }
                    else
                    {
                        vinaRow = 0;
                        vinaRowS++;
                        sVinaRow = 0;
                        sVinaRowS++;
                    }

                    //Add points exponentially per amount of pieces of our color in a row
                    //if there is still space to meet our board sequence goal
                    if (vinaRow >= 2 && vinaRowS >= board.piecesInSequence)
                    {
                        h += vinaRow * vinaRow;

                    }

                    if (vinaRow >= board.piecesInSequence-2 && vinaRowS >= board.piecesInSequence)
                    {
                        h += vinaRow * vinaRow;

                    }

                    //Add points exponentially per amount of pieces of our shape in a row
                    //if there is still space to meet our board sequence goal
                    if (sVinaRow >= 2 && sVinaRowS >= board.piecesInSequence)
                    {
                        h += sVinaRow * sVinaRow * 2;

                    }

                    if (sVinaRow >= board.piecesInSequence - 2 && sVinaRowS >= board.piecesInSequence)
                    {
                        h += sVinaRow * sVinaRow * 2;

                    }

                }
            }

            //Same criteria but for horizontal pieces
            for (int i = 0; i < board.cols; i++)
            {
                if (hinaRowS >= board.piecesInSequence)
                {

                    h += hinaRowS;

                }
                if (sHinaRowS >= board.piecesInSequence)
                {
                    h += sHinaRowS;


                }
                hinaRow = 0;
                hinaRowS = 0;
                sHinaRow = 0;
                sHinaRowS = 0;

                for (int j = 0; j < board.rows; j++)
                {
                    Piece? piece = board[j, i];



                    if (piece.HasValue)
                    {

                        if (board[j, i].Value.color == color)
                        {
                            hinaRow++;
                            hinaRowS++;

                        }
                        else
                        {
                            if (hinaRowS >= board.piecesInSequence)
                            {

                                h += hinaRowS;

                            }

                            hinaRow = 0;
                            hinaRowS = 0;

                        }

                        if (board[j, i].Value.shape == color.Shape())
                        {
                            sHinaRow++;
                            sHinaRowS++;

                        }
                        else
                        {
                            if (sHinaRowS >= board.piecesInSequence)
                            {
                                h += sHinaRowS;


                            }
                            sHinaRow = 0;
                            sHinaRowS = 0;

                        }

                    }
                    else
                    {
                        hinaRow = 0;
                        hinaRowS++;
                        sHinaRow = 0;
                        sHinaRowS++;
                    }

                    if (hinaRow >= 2 && hinaRowS >= board.piecesInSequence)
                    {
                        h += hinaRow * hinaRow;



                    }

                    if (hinaRow >= board.piecesInSequence-2 && hinaRowS >= board.piecesInSequence)
                    {
                        h += hinaRow * hinaRow;



                    }

                    if (sHinaRow >= 2 && sHinaRowS >= board.piecesInSequence)
                    {
                        h += sHinaRow * sHinaRow * 2;


                    }

                    if (sHinaRow >= board.piecesInSequence - 2 && sHinaRowS >= board.piecesInSequence)
                    {
                        h += sHinaRow * sHinaRow * 2;


                    }



                }
            }

            //Same criteria but for Diagonal pieces from down to up 
            for (int k = 0; k < board.rows; k++)
            {

                if (dinaRowS >= board.piecesInSequence)
                {
                    h += dinaRowS;


                }
                if (sDinaRowS >= board.piecesInSequence)
                {
                    h += sDinaRowS;

                }
                dinaRow = 0;
                dinaRowS = 0;
                sDinaRow = 0;
                sDinaRowS = 0;

                for (int i = 0, j = 0; i < (board.rows - k) && j < board.cols; i++, j++)
                {
                    Piece? piece = board[(i + k), j];



                    if (piece.HasValue)
                    {

                        if (board[i + k, j].Value.color == color)
                        {
                            dinaRow++;
                            dinaRowS++;


                        }
                        else
                        {
                            if (dinaRowS >= board.piecesInSequence)
                            {
                                h += dinaRowS;


                            }
                            dinaRow = 0;
                            dinaRowS = 0;
                        }

                        if (board[i + k, j].Value.shape == color.Shape())
                        {
                            sDinaRow++;
                            sDinaRowS++;

                        }
                        else
                        {
                            if (sDinaRowS >= board.piecesInSequence)
                            {
                                h += sDinaRowS;

                            }
                            sDinaRow = 0;
                            sDinaRowS = 0;

                        }

                    }
                    else
                    {
                        dinaRow = 0;
                        dinaRowS++;
                        sDinaRow = 0;
                        sDinaRowS++;
                    }



                    if (dinaRow >= 2 && dinaRowS >= board.piecesInSequence)
                    {
                        h += dinaRow * dinaRow;


                    }


                    if (dinaRow >= board.piecesInSequence-2 && dinaRowS >= board.piecesInSequence)
                    {
                        h += dinaRow * dinaRow;


                    }

                    if (sDinaRow >= 2 && sDinaRowS >= board.piecesInSequence)
                    {
                        h += sDinaRow * sDinaRow * 2;

                    }

                    if (sDinaRow >= board.piecesInSequence - 2 && sDinaRowS >= board.piecesInSequence)
                    {
                        h += sDinaRow * sDinaRow * 2;

                    }
                }
            }

            //Same criteria but for Diagonal pieces from up to down
            for (int k = board.rows - 1; k >= 0; k--)
            {
                if (dinaRowS >= board.piecesInSequence)
                {
                    h += d2inaRowS;

                }
                if (sDinaRowS >= board.piecesInSequence)
                {
                    h += sD2inaRowS;

                }

                d2inaRow = 0;
                d2inaRowS = 0;
                sD2inaRow = 0;
                sD2inaRowS = 0;
                for (int i = board.rows - 1, j = board.cols - 1; i >= 0 + k && j >= 0; i--, j--)
                {
                    Piece? piece = board[i - k, j];

                    if (piece.HasValue)
                    {

                        if (board[i - k, j].Value.color == color)
                        {
                            d2inaRow++;
                            d2inaRowS++;


                        }

                        else
                        {
                            if (dinaRowS >= board.piecesInSequence)
                            {
                                h += d2inaRowS;

                            }
                            d2inaRow = 0;
                            d2inaRowS = 0;
                        }

                        if (board[i - k, j].Value.shape == color.Shape())
                        {
                            sD2inaRow++;
                            sD2inaRowS++;

                        }
                        else
                        {
                            if (sDinaRowS >= board.piecesInSequence)
                            {
                                h += sD2inaRowS;

                            }


                            sD2inaRow = 0;
                            sD2inaRowS = 0;

                        }

                    }
                    else
                    {
                        d2inaRow = 0;
                        d2inaRowS++;
                        sD2inaRow = 0;
                        sD2inaRowS++;
                    }



                    if (dinaRow >= 2 && dinaRowS >= board.piecesInSequence)
                    {
                        h += d2inaRow * d2inaRow;

                    }

                    if (dinaRow >= board.piecesInSequence-2 && dinaRowS >= board.piecesInSequence)
                    {
                        h += d2inaRow * d2inaRow;

                    }


                    if (sDinaRow >= 2 && sDinaRowS >= board.piecesInSequence)
                    {
                        h += sD2inaRow * sD2inaRow * 2;

                    }


                    if (sDinaRow >= board.piecesInSequence - 2 && sDinaRowS >= board.piecesInSequence)
                    {
                        h += sD2inaRow * sD2inaRow * 2;

                    }
                }
            }

            float evinaRow = 0;
            float evinaRowS = 0;
            float ehinaRow = 0;
            float ehinaRowS = 0;

            float esVinaRow = 0;
            float esVinaRowS = 0;
            float esHinaRow = 0;
            float esHinaRowS = 0;

            float edinaRow = 0;
            float edinaRowS = 0;
            float esDinaRow = 0;
            float esDinaRowS = 0;

            float ed2inaRow = 0;
            float ed2inaRowS = 0;
            float esD2inaRow = 0;
            float esD2inaRowS = 0;



            //Same criterias as above but now subtracting points
            //because its for the enemy pieces
            for (int i = 0; i < board.rows; i++)
            {

                evinaRow = 0;
                evinaRowS = 0;
                esVinaRow = 0;
                esVinaRowS = 0;

                for (int j = 0; j < board.cols; j++)
                {
                    Piece? piece = board[i, j];


                    if (piece.HasValue)
                    {



                        //Count amount of vertical pieces of our color in a row and 
                        //amount of spaces(including of our color) in a row and give it points per amount in a row
                        if (board[i, j].Value.color != color)
                        {
                            evinaRow++;
                            evinaRowS++;

                        }
                        else
                        {
                            //Give it points if there is enough space to fill our needed sequence
                            if (evinaRowS >= board.piecesInSequence)
                            {
                                h -= evinaRowS;

                            }
                            evinaRow = 0;
                            evinaRowS = 0;

                        }

                        //Count amount of vertical pieces of our shape in a row and 
                        //amount of spaces(including of our shape) in a row and give it points per amount in a row
                        if (board[i, j].Value.shape != color.Shape())
                        {
                            esVinaRow++;
                            esVinaRowS++;

                        }
                        else
                        {
                            //Give it points if there is enough space to fill our needed sequence
                            if (esVinaRowS >= board.piecesInSequence)
                            {
                                h -= esVinaRowS;

                            }

                            esVinaRow = 0;
                            esVinaRowS = 0;

                        }

                    }
                    else
                    {
                        evinaRow = 0;
                        evinaRowS++;
                        esVinaRow = 0;
                        esVinaRowS++;
                    }

                    //Add points exponentially per amount of pieces of our color in a row
                    //if there is still space to meet our board sequence goal
                    if (evinaRow >= 2 && evinaRowS >= board.piecesInSequence)
                    {
                        h -= evinaRow * evinaRow;

                    }

                    if (evinaRow >= board.piecesInSequence-2 && evinaRowS >= board.piecesInSequence)
                    {
                        h -= evinaRow * evinaRow;

                    }

                    //Add points exponentially per amount of pieces of our shape in a row
                    //if there is still space to meet our board sequence goal
                    if (esVinaRow >= 2 && esVinaRowS >= board.piecesInSequence)
                    {
                        h -= sVinaRow * esVinaRow * 2;

                    }

                    if (esVinaRow >= board.piecesInSequence - 2 && esVinaRowS >= board.piecesInSequence)
                    {
                        h -= sVinaRow * esVinaRow * 2;

                    }

                }
            }

            //Same criteria but for horizontal pieces
            for (int i = 0; i < board.cols; i++)
            {
                if (ehinaRowS >= board.piecesInSequence)
                {

                    h -= hinaRowS;

                }
                if (esHinaRowS >= board.piecesInSequence)
                {
                    h -= esHinaRowS;


                }
                ehinaRow = 0;
                ehinaRowS = 0;
                esHinaRow = 0;
                esHinaRowS = 0;

                for (int j = 0; j < board.rows; j++)
                {
                    Piece? piece = board[j, i];



                    if (piece.HasValue)
                    {

                        if (board[j, i].Value.color != color)
                        {
                            ehinaRow++;
                            ehinaRowS++;

                        }
                        else
                        {
                            if (ehinaRowS >= board.piecesInSequence)
                            {

                                h -= ehinaRowS;

                            }

                            ehinaRow = 0;
                            ehinaRowS = 0;

                        }

                        if (board[j, i].Value.shape != color.Shape())
                        {
                            esHinaRow++;
                            esHinaRowS++;

                        }
                        else
                        {
                            if (esHinaRowS >= board.piecesInSequence)
                            {
                                h -= esHinaRowS;


                            }
                            esHinaRow = 0;
                            esHinaRowS = 0;

                        }

                    }
                    else
                    {
                        ehinaRow = 0;
                        ehinaRowS++;
                        esHinaRow = 0;
                        esHinaRowS++;
                    }

                    if (ehinaRow >= 2 && ehinaRowS >= board.piecesInSequence)
                    {
                        h -= ehinaRow * ehinaRow;



                    }


                    if (ehinaRow >= board.piecesInSequence-2 && ehinaRowS >= board.piecesInSequence)
                    {
                        h -= ehinaRow * ehinaRow;



                    }

                    if (esHinaRow >= 2 && esHinaRowS >= board.piecesInSequence)
                    {
                        h -= esHinaRow * esHinaRow * 2;


                    }

                    if (esHinaRow >= board.piecesInSequence - 2 && esHinaRowS >= board.piecesInSequence)
                    {
                        h -= esHinaRow * esHinaRow * 2;


                    }



                }
            }

            //Same criteria but for Diagonal pieces from down to up 
            for (int k = 0; k < board.rows; k++)
            {

                if (edinaRowS >= board.piecesInSequence)
                {
                    h -= dinaRowS;


                }
                if (esDinaRowS >= board.piecesInSequence)
                {
                    h += esDinaRowS;

                }
                edinaRow = 0;
                edinaRowS = 0;
                esDinaRow = 0;
                esDinaRowS = 0;

                for (int i = 0, j = 0; i < (board.rows - k) && j < board.cols; i++, j++)
                {
                    Piece? piece = board[(i + k), j];



                    if (piece.HasValue)
                    {

                        if (board[i + k, j].Value.color != color)
                        {
                            edinaRow++;
                            edinaRowS++;


                        }
                        else
                        {
                            if (edinaRowS >= board.piecesInSequence)
                            {
                                h -= edinaRowS;


                            }
                            edinaRow = 0;
                            edinaRowS = 0;
                        }

                        if (board[i + k, j].Value.shape != color.Shape())
                        {
                            esDinaRow++;
                            esDinaRowS++;

                        }
                        else
                        {
                            if (esDinaRowS >= board.piecesInSequence)
                            {
                                h -= esDinaRowS;

                            }
                            esDinaRow = 0;
                            esDinaRowS = 0;

                        }

                    }
                    else
                    {
                        edinaRow = 0;
                        edinaRowS++;
                        esDinaRow = 0;
                        esDinaRowS++;
                    }



                    if (edinaRow >= 2 && edinaRowS >= board.piecesInSequence)
                    {
                        h -= edinaRow * edinaRow;


                    }

                    if (edinaRow >= board.piecesInSequence -2 && edinaRowS >= board.piecesInSequence)
                    {
                        h -= edinaRow * edinaRow;


                    }


                    if (esDinaRow >= 2 && esDinaRowS >= board.piecesInSequence)
                    {
                        h -= esDinaRow * esDinaRow * 2;

                    }

                    if (esDinaRow >= board.piecesInSequence-2 && esDinaRowS >= board.piecesInSequence)
                    {
                        h -= esDinaRow * esDinaRow * 2;

                    }
                }
            }

            //Same criteria but for Diagonal pieces from up to down
            for (int k = board.rows - 1; k >= 0; k--)
            {
                if (edinaRowS >= board.piecesInSequence)
                {
                    h -= d2inaRowS;

                }
                if (esDinaRowS >= board.piecesInSequence)
                {
                    h -= esD2inaRowS;

                }

                ed2inaRow = 0;
                ed2inaRowS = 0;
                esD2inaRow = 0;
                esD2inaRowS = 0;
                for (int i = board.rows - 1, j = board.cols - 1; i >= 0 + k && j >= 0; i--, j--)
                {
                    Piece? piece = board[i - k, j];

                    if (piece.HasValue)
                    {

                        if (board[i - k, j].Value.color != color)
                        {
                            ed2inaRow++;
                            ed2inaRowS++;


                        }

                        else
                        {
                            if (edinaRowS >= board.piecesInSequence)
                            {
                                h -= ed2inaRowS;

                            }
                            ed2inaRow = 0;
                            ed2inaRowS = 0;
                        }

                        if (board[i - k, j].Value.shape != color.Shape())
                        {
                            esD2inaRow++;
                            esD2inaRowS++;

                        }
                        else
                        {
                            if (esDinaRowS >= board.piecesInSequence)
                            {
                                h -= esD2inaRowS;

                            }


                            esD2inaRow = 0;
                            esD2inaRowS = 0;

                        }

                    }
                    else
                    {
                        ed2inaRow = 0;
                        ed2inaRowS++;
                        esD2inaRow = 0;
                        esD2inaRowS++;
                    }



                    if (edinaRow >= 2 && edinaRowS >= board.piecesInSequence)
                    {
                        h -= ed2inaRow * ed2inaRow;

                    }

                    if (edinaRow >= board.piecesInSequence-2 && edinaRowS >= board.piecesInSequence)
                    {
                        h -= ed2inaRow * ed2inaRow;

                    }


                    if (esDinaRow >= 2 && esDinaRowS >= board.piecesInSequence)
                    {
                        h -= esD2inaRow * esD2inaRow * 2;

                    }

                    if (esDinaRow >= board.piecesInSequence-2 && esDinaRowS >= board.piecesInSequence)
                    {
                        h -= esD2inaRow * esD2inaRow * 2;

                    }
                }
            }





            //Always leave 2 pieces of each shape for winning/losing scenarios
            if (board.PieceCount(color, color.Shape()) <= 2)
            {
                h -= 100000;
            }


            // Return the final heuristic score for the given board

            return h;
        }


    }
}
