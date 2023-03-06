using BattleshipLiteLibrary.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {


        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>()
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };
            List<int> numbers = new List<int>()
            {
                1,
                2,
                3,
                4,
                5
            };
            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }

            }
        }
        public static int GetShotCount(PlayerInfoModel player)
        {
            int shotCount = 0;
            foreach (var shot in player.ShotGrid)
            {
                if (shot.Status != GridSpotStatus.Empty)
                {
                    shotCount += 1;
                }
            }
            return shotCount;
        }
        public static bool PlaceShips(PlayerInfoModel model, string location)
        {
            (string row, int collumn) = SplitShotIntoRowAndColumn(location);
            bool isValidLocation = ValidateGridLocation(model, row, collumn);

            bool IsSpotOpen = ValidateShipLocation(model, row, collumn);
            bool output = false;
            if (isValidLocation && IsSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row.ToUpper(),
                    SpotNumber = collumn,
                    Status = GridSpotStatus.Ship
                });
                output = true;
            }
            return output;
        }

        private static bool ValidateShipLocation(PlayerInfoModel model, string row, int collumn)
        {
            bool isValidLocation = true;
            foreach (var ship in model.ShipLocations)
            {
                if (ship.SpotLetter == row.ToUpper() && ship.SpotNumber == collumn)
                {
                    isValidLocation = false;
                }
            }
            return isValidLocation;
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, string row, int collumn)
        {
            bool isValidLocation = false;
            foreach (var shot in model.ShotGrid)
            {
                if (shot.SpotLetter == row.ToUpper() && shot.SpotNumber == collumn)
                {
                    isValidLocation = true;
                }
            }
            return isValidLocation;
        }

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            bool isActive = false;
            foreach (var ship in player.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                {
                    isActive= true;
                }
            }
            return isActive;
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel()
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };
            model.ShotGrid.Add(spot);
        }

        public static (string row, int collumn) SplitShotIntoRowAndColumn(string shot)
        {
            string row = "";
            int collumn = 0;

            if (shot.Length != 2)
            {
                throw new ArgumentException("This Was An Invalid Shot Type", "shot");

            }

            char[] shotArray = shot.ToArray();

            row = shotArray[0].ToString();
            collumn = int.Parse(shotArray[1].ToString());

            return (row, collumn);

        }

        public static bool ValidateShot(PlayerInfoModel player, string row, int collumn)
        {
            bool isValidShot = false;
            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == collumn)
                {
                    if (gridSpot.Status == GridSpotStatus.Empty)
                    {
                        isValidShot = true;
                    }
                }
            }
            return isValidShot;
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int collumn)
        {
            bool isAHit = false;
            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.SpotLetter == row.ToUpper() && ship.SpotNumber == collumn)
                {
                    isAHit = true;
                    ship.Status = GridSpotStatus.Sunk;
                }
            }
            return isAHit;
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, string row, int collumn, bool isAHit)
        {
            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == collumn)
                {
                    if (isAHit)
                    {
                        gridSpot.Status= GridSpotStatus.Hit;
                    }
                    else
                    {
                        gridSpot.Status = GridSpotStatus.Miss;
                    }
                }
            }
        }
    }
}
