using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipLiteLibrary;
using BattleshipLiteLibrary.models;


namespace BattleShipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WellcomeMessage();
            HowToPlayWiki();
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;
            

            do
            {
                Console.WriteLine($"Opponent Board <{opponent.UsersName}>");
                Console.WriteLine(" vv  vv  vv  vv  vv");

                DisplayShotGrid(activePlayer);

                Console.WriteLine();
                Console.WriteLine();

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinues = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinues == true)
                {
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner); 

            Console.ReadLine();
        }

        private static void HowToPlayWiki()
        {
            Console.WriteLine("  Game Is About Strategy. You Have 5 Ships Which You Can Place In Desired Location In Given Grid.");
            Console.WriteLine("  Main Goal To Destroy Enemy's Ships Before They Do... Good Luck! ");
            Console.WriteLine();
            Console.WriteLine("Symbol Meanings ");

            Console.WriteLine("  Hit - X    Miss - O");
            Console.WriteLine();
            Console.WriteLine();

        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations To {winner.UsersName} For Winning!");
            Console.WriteLine($"{winner.UsersName} Took { GameLogic.GetShotCount(winner) } Shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int collumn = 0;
            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, collumn) = GameLogic.SplitShotIntoRowAndColumn(shot);

                    isValidShot = GameLogic.ValidateShot(activePlayer, row, collumn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    isValidShot= false;
                }
                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid Shot Location. Please Try Again");
                }
            } while (isValidShot == false);
            bool isAHit = GameLogic.IdentifyShotResult(opponent,row,collumn);

            GameLogic.MarkShotResult(activePlayer, row, collumn, isAHit);

            DisplayShotResult(row, collumn, isAHit);
        }

        private static void DisplayShotResult(string row,int collumn, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{collumn} Is a Hit!");
            }
            else
            {
                Console.WriteLine($"{row}{collumn} Is a Miss!");
            }
        }

        private static string AskForShot(PlayerInfoModel activePlayer)
        {
            Console.Write($"{activePlayer.UsersName}, please Enter Your Shot Selection: ");
            string output = Console.ReadLine().ToUpper();
            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }
        }

        private static void WellcomeMessage()
        {
            Console.WriteLine("Welcome To BattleShip Lite");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel player = new PlayerInfoModel();

            Console.WriteLine($"Player Information - {playerTitle}");
            
            player.UsersName = AskForUsersName();

            GameLogic.InitializeGrid(player);

            PlaceShips(player);

            Console.Clear();

            return player;

        }
        private static string AskForUsersName()
        {
            Console.Write("Please Input Your Username: ");
            string name = Console.ReadLine();
            return name;

        }
        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where Do You Want To Place Ship { model.ShipLocations.Count + 1 } : ");
                string location = Console.ReadLine();
                bool isValidLocation = false;
                try
                {
                    
                    isValidLocation = GameLogic.PlaceShips(model, location);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                if (isValidLocation == false)
                {
                    Console.WriteLine("That Was Not A Valid Location. Please try Again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
