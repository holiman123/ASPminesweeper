using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinesweeperModel
{
    [Serializable]
    public class MinesweeperGame
    {
        // -5 = detoneted bpmb, -4 = flag with bomb, -3 = flag withOUT bomb,
        // -2 = closed, -1 = bomb, 0-8 = opened and shows number
        public int[,] field { get; set; }
        public int bombsCount { get; set; }
        public int placedFlagsCount { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public bool isLost { get; set; } = false;
        public bool isWin { get; set; } = false;
        public bool isFieldBombed = false;

        public MinesweeperGame(int sizeX, int sizeY, int bombsCount)
        {
            createField(sizeX, sizeY, bombsCount);
        }
        public MinesweeperGame() { }

        public void createField(int sizeX, int sizeY, int bombsCount)
        {
            field = new int[sizeY, sizeX];
            placedFlagsCount = 0;
            this.bombsCount = bombsCount;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            isLost = false;
            isWin = false;
            isFieldBombed = false;

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    field[y, x] = -2;
        }
        public bool dig(int x, int y)
        {
            bool isDigged = false;

            if (!isFieldBombed)
            {
                // bombing
                Random rnd = new Random();
                int tempX;
                int tempY;
                for (int i = 0; i < bombsCount; i++)
                {
                    tempX = rnd.Next(sizeX);
                    tempY = rnd.Next(sizeY);
                    if (field[tempY, tempX] == -1 || ((tempY >= y-1 && tempY <= y+1) && (tempX >= x - 1 && tempX <= x + 1)))
                        i--;
                    else
                        field[tempY, tempX] = -1;
                }
                isFieldBombed = true;
            }
            // digging closed tile
            if (field[y, x] == -2 || field[y, x] == -1)
            {
                digOneTile(x, y);

                // if digged bomb
                if (field[y, x] == -1)
                {
                    field[y, x] = -5;
                    isLost = true;
                }

                else if (field[y, x] == 0)
                {

                    for (int i = y - 1; i <= y + 1; i++)
                        for (int j = x - 1; j <= x + 1; j++)
                        {
                            if ((0 <= i && i < sizeY) && (0 <= j && j < sizeX))    // in the field interval
                                if (i != y || j != x)
                                    if (field[i, j] == -2)
                                        dig(j, i);
                        }

                }
                isDigged = true;
            }

            // digging number
            if (field[y, x] > 0)
            {
                int nearFlagsCount = 0;
                for (int i = y - 1; i <= y + 1; i++)
                    for (int j = x - 1; j <= x + 1; j++)
                    {
                        if ((0 <= i && i < sizeY) && (0 <= j && j < sizeX))    // in the field interval
                            if (field[i, j] == -3 || field[i, j] == -4)
                                nearFlagsCount++;
                    }

                if (nearFlagsCount == field[y, x])  // if all flags standing nearby
                {
                    for (int i = y - 1; i <= y + 1; i++)
                        for (int j = x - 1; j <= x + 1; j++)
                        {
                            if ((0 <= i && i < sizeY) && (0 <= j && j < sizeX))    // in the field interval
                                if (i != y || j != x)
                                {
                                    if (field[i, j] == -2)
                                    {
                                        digOneTile(j, i);
                                        isDigged = true;
                                    }

                                    // if digged bomb
                                    if (field[i, j] == -1)
                                    {
                                        field[i, j] = -5;
                                        isLost = true;
                                        isDigged = true;
                                    }

                                    if (field[i, j] == 0)
                                    {
                                        for (int i1 = i - 1; i1 <= i + 1; i1++)
                                            for (int j1 = j - 1; j1 <= j + 1; j1++)
                                            {
                                                if ((0 <= i1 && i1 < sizeY) && (0 <= j1 && j1 < sizeX))    // in the field interval
                                                    if (i1 != i || j1 != j)
                                                        if (field[i1, j1] == -2)
                                                        {
                                                            dig(j1, i1);
                                                            isDigged = true;
                                                        }
                                            }
                                    }
                                }
                        }
                }
            }

            //check for win
            int closedNoneBombTiles = 0;
            for (int i = 0; i < sizeY; i++)
                for (int j = 0; j < sizeX; j++)
                    if (field[i, j] == -2 || field[i, j] == -5)
                        closedNoneBombTiles++;
            if (closedNoneBombTiles == 0)
                isWin = true;

            return isDigged;
        }
        public bool flag(int x, int y)
        {
            bool isFlaged = false;
            if (isFieldBombed)
            {
                if (field[y, x] == -3)      // if there is falg withOUT bomb
                {
                    field[y, x] = -2;       // return closed tile
                    placedFlagsCount--;
                    isFlaged = true;
                }
                else if (field[y, x] == -4)      // if there is flag with bomb
                {
                    field[y, x] = -1;       // return bomb
                    placedFlagsCount--;
                    isFlaged = true;
                }

                else if (field[y, x] == -1)      // if there is bomb
                {
                    field[y, x] = -4;       // place flag with bomb
                    placedFlagsCount++;
                    isFlaged = true;
                }
                else if (field[y, x] == -2)      // if there is no bomb
                {
                    field[y, x] = -3;       // place flag withOUT bomb
                    placedFlagsCount++;
                    isFlaged = true;
                }
            }
            return isFlaged;
        }

        private void digOneTile(int x, int y)
        {
            int nearBombsCount = 0;
            if (field[y, x] != -1)
            {
                for(int i = y-1; i <= y+1; i++)
                    for (int j = x-1; j <= x+1; j++)
                    {
                        if ((0 <= i && i < sizeY) && (0 <= j && j < sizeX))    // in the field interval
                            if (field[i, j] == -1 || field[i, j] == -4)
                                nearBombsCount++;
                    }
                field[y, x] = nearBombsCount;
            }
        }
    }
}
