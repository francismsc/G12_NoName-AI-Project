using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System.Collections.Generic;


namespace NoName
{

    public class G12_NoName : AbstractThinker
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
            return base.ToString() + "_v1";
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
            PColor player, PColor turn, int depth,float alpha, float beta)
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
            else if(depth == maxDepth)
            {
             selectedMove = (FutureMove.NoMove, Heuristic(board, player));
            }
            else
            {
                selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                for (int i = 0; i< Cols; i++)
                {
                    if(board.IsColumnFull(i)) continue;

                    for (int j = 0; j < 2; j++)
                    {
                        PShape shape = (PShape)j;

                        float eval;

                        if (board.PieceCount(turn, shape) == 0)
                        {

                            continue;
                        }

                        board.DoMove(shape,i);
                        eval = -Negamax(board,ct,player.Other(),turn.Other(), depth + 1,-beta, -alpha).score;
                        board.UndoMove();

                        if(eval > selectedMove.score)
                        {

                            selectedMove = (new FutureMove(i, shape), eval);
                        }

                        if(eval > alpha)
                        {
                            alpha = eval;
                        }

                        if(alpha >= beta)
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

            float RinaRow = 0;
            float RinaRowS = 0;
            float HinaRow = 0;
            float HinaRowS = 0;

            float SRinaRow = 0;
            float SRinaRowS = 0;
            float SHinaRow = 0;
            float SHinaRowS = 0;

            float DinaRow = 0;
            float DinaRowS = 0;
            float SDinaRow = 0;
            float SDinaRowS = 0;

            float D2inaRow = 0;
            float D2inaRowS = 0;
            float SD2inaRow = 0;
            float SD2inaRowS = 0;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Current heuristic value
            float h = 0;

            
            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                
                RinaRow = 0;
                RinaRowS = 0;
                SRinaRow = 0;
                SRinaRowS = 0;

                for (int j = 0; j < board.cols; j++)
                {
                    Piece? piece = board[i, j];
      

                    if (piece.HasValue)
                    {

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
                        // If the piece is of our shape, increment the
                        // heuristic inversely to the distance from the center

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

                        if (board[i,j].Value.color == color)
                        {
                            RinaRow++;
                            RinaRowS++;

                        }else
                        {
                            if (RinaRowS >= board.piecesInSequence)
                            {
                                h += RinaRowS;

                            }
                            RinaRow = 0;
                            RinaRowS = 0;
                            
                        }

                        if (board[i, j].Value.shape == color.Shape())
                        {
                            SRinaRow++;
                            SRinaRowS++;

                        }
                        else
                        {
                            if (SRinaRowS >= board.piecesInSequence)
                            {
                                h += SRinaRowS;

                            }

                            SRinaRow = 0;
                            SRinaRowS = 0;

                        }

                    }
                    else
                    {
                        RinaRow = 0;
                        RinaRowS++;
                        SRinaRow = 0;
                        SRinaRowS++; 
                    }



                    if (RinaRow >= 2 && RinaRowS >= board.piecesInSequence)
                    {
                        h += RinaRow * RinaRow ;

                    }


                    if (SRinaRow >= 2 && SRinaRowS >= board.piecesInSequence)
                    {
                        h += SRinaRow * SRinaRow;

                    }

                }
            }

            for (int i = 0; i < board.cols; i++)
            {
                if (HinaRowS >= board.piecesInSequence)
                {

                    h += HinaRowS;

                }
                if (SHinaRowS >= board.piecesInSequence)
                {
                    h += SHinaRowS;


                }
                HinaRow = 0;
                HinaRowS = 0;
                SHinaRow = 0;
                SHinaRowS = 0;

                for (int j = 0; j < board.rows; j++)
                {
                    Piece? piece = board[j, i];



                    if (piece.HasValue)
                    {

                        if (board[j, i].Value.color == color)
                        {
                            HinaRow++;
                            HinaRowS++;

                        }
                        else
                        {
                            if (HinaRowS >= board.piecesInSequence)
                            {

                                h += HinaRowS;

                            }

                            HinaRow = 0;
                            HinaRowS = 0;

                        }

                        if (board[j, i].Value.shape == color.Shape())
                        {
                            SHinaRow++;
                            SHinaRowS++;

                        }
                        else
                        {
                            if (SHinaRowS >= board.piecesInSequence)
                            {
                                h += SHinaRowS;


                            }
                            SHinaRow = 0;
                            SHinaRowS = 0;

                        }

                    }
                    else
                    {
                        HinaRow = 0;
                        HinaRowS++;
                        SHinaRow = 0;
                        SHinaRowS++;
                    }

                    if (HinaRow >= 2 && HinaRowS >= board.piecesInSequence)
                    {
                        h += HinaRow * HinaRow;



                    }

                    if (SHinaRow >= 2 && SHinaRowS >= board.piecesInSequence)
                    {
                        h += SHinaRow * SHinaRow;


                    }



                }
            }
            for (int k = 0; k < board.rows; k++)
            {

                if (DinaRowS >= board.piecesInSequence)
                {
                    h += DinaRowS;


                }
                if (SDinaRowS >= board.piecesInSequence)
                {
                    h += SDinaRowS;

                }
                DinaRow = 0;
                DinaRowS = 0;
                SDinaRow = 0;
                SDinaRowS = 0;

                for (int i = 0, j = 0; i < (board.rows - k) && j < board.cols; i++, j++)
                {
                    Piece? piece = board[(i+k), j];



                    if (piece.HasValue)
                    {

                        if (board[i+k, j].Value.color == color)
                        {
                            DinaRow++;
                            DinaRowS++;


                        }
                        else
                        {
                            if (DinaRowS >= board.piecesInSequence)
                            {
                                h += DinaRowS;


                            }
                            DinaRow = 0;
                            DinaRowS = 0;
                        }

                        if (board[i+k, j].Value.shape == color.Shape())
                        {
                            SDinaRow++;
                            SDinaRowS++;

                        }
                        else
                        {
                            if (SDinaRowS >= board.piecesInSequence)
                            {
                                h += SDinaRowS;

                            }
                            SDinaRow = 0;
                            SDinaRowS = 0;

                        }

                    }
                    else
                    {
                        DinaRow = 0;
                        DinaRowS++;
                        SDinaRow = 0;
                        SDinaRowS++;
                    }



                    if (DinaRow >= 2 && DinaRowS >= board.piecesInSequence)
                    {
                        h += DinaRow * DinaRow;


                    }


                    if (SDinaRow >= 2 && SDinaRowS >= board.piecesInSequence)
                    {
                        h += SDinaRow * SDinaRow;

                    }
                }
            }
            for (int k = board.rows - 1; k >= 0; k--)
            {
                if (DinaRowS >= board.piecesInSequence)
                {
                    h += D2inaRowS;

                }
                if (SDinaRowS >= board.piecesInSequence)
                {
                    h += SD2inaRowS;

                }

                D2inaRow = 0;
                D2inaRowS = 0;
                SD2inaRow = 0;
                SD2inaRowS = 0;
                for (int i = board.rows - 1, j = board.cols - 1; i >= 0 + k && j >= 0; i--, j--)
                {
                    Piece? piece = board[i-k, j];

                    if (piece.HasValue)
                    {

                        if (board[i-k, j].Value.color == color)
                        {
                            D2inaRow++;
                            D2inaRowS++;


                        }

                        else
                        {
                            if (DinaRowS >= board.piecesInSequence)
                            {
                                h += D2inaRowS;

                            }
                            D2inaRow = 0;
                            D2inaRowS = 0;
                        }

                        if (board[i-k, j].Value.shape == color.Shape())
                        {
                            SD2inaRow++;
                            SD2inaRowS++;

                        }
                        else
                        {
                            if (SDinaRowS >= board.piecesInSequence)
                            {
                                h += SD2inaRowS;

                            }


                            SD2inaRow = 0;
                            SD2inaRowS = 0;

                        }

                    }
                    else
                    {
                        D2inaRow = 0;
                        D2inaRowS++;
                        SD2inaRow = 0;
                        SD2inaRowS++;
                    }



                    if (DinaRow >= 2 && DinaRowS >= board.piecesInSequence)
                    {
                        h += D2inaRow * D2inaRow;

                    }


                    if (SDinaRow >= 2 && SDinaRowS >= board.piecesInSequence)
                    {
                        h += SD2inaRow * SD2inaRow;

                    }
                }
            }

            if (board.PieceCount(color, color.Shape()) <= 2)
            {
                h -= 100000;
            }


            // Return the final heuristic score for the given board
            
            return h;
        }
    

    }
}
