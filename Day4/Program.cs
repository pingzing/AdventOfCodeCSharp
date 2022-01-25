using System.Diagnostics;
using BingoBoard = System.Collections.Generic.Dictionary<int, BingoCell>;

var linesEnumerable = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "input.txt"));
var linesEnumerator = linesEnumerable.GetEnumerator();
linesEnumerator.MoveNext();
List<int> numbersLine = linesEnumerator.Current
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(x => int.Parse(x))
    .ToList();

// Skip the blank separator line
linesEnumerator.MoveNext();
List<BingoBoard> bingoCards = new List<BingoBoard>();

BingoBoard currentCard = new();
int currRow = 0;
int currCol = 0;
// Read the rest of the bingo cards
foreach (string line in linesEnumerable)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        bingoCards.Add(currentCard);
        currentCard = new BingoBoard();
        currRow = 0;
        currCol = 0;
        continue;
    }

    string[] cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    foreach (string cell in cells)
    {
        int cellValue = int.Parse(cell);
        currentCard[cellValue] = new BingoCell(currRow, currCol);
        currCol++;
    }

    currCol = 0;
    currRow++;
}

PartOne(bingoCards, numbersLine);
PartTwo(bingoCards, numbersLine);

void PartOne(List<BingoBoard> bingoCards, List<int> numbersLine)
{
    // Run the first 5 iterations of the board, can't have any winners until then
    for (int i = 0; i < 5; i++)
    {
        MarkBoards(bingoCards, numbersLine[i]);
    }

    for (int i = 5; i < numbersLine.Count; i++)
    {
        MarkBoards(bingoCards, numbersLine[i]);
        BingoBoard? winner = FindWinners(bingoCards)?.FirstOrDefault();
        if (winner != null)
        {
            int unmarkedSum = winner.Where(x => x.Value.Marked == false).Sum(x => x.Key);
            Console.WriteLine($"Winner found. Unmarked sum: {unmarkedSum}. Last called: {numbersLine[i]}. Multiplied together: {unmarkedSum * numbersLine[i]}");
            return;
        }
    }
}

void PartTwo(List<BingoBoard> bingoCards, List<int> numbersLine)
{
    List<BingoBoard> winners = new List<BingoBoard>();

    List<BingoBoard>? currentWinners = null;
    int calledWhenWon = -1;
    for (int i = 0; i < numbersLine.Count; i++)
    {
        MarkBoards(bingoCards, numbersLine[i]);
        var newWinners = FindWinners(bingoCards);
        if (newWinners != null)
        {
            currentWinners = newWinners.ToList();
            calledWhenWon = numbersLine[i];
            foreach (var winner in newWinners)
            {
                bingoCards.Remove(winner);
            }
            winners.AddRange(newWinners);

            // All winners found, we're done!
            if (bingoCards.Count == 0)
            {
                break;
            }
        }
    }

    int unmarkedSum = currentWinners.First().Where(x => x.Value.Marked == false).Sum(x => x.Key);
    Console.WriteLine($"Last winner found. Unmarked sum: {unmarkedSum}. Last called: {calledWhenWon}. Multiplied together: {unmarkedSum * calledWhenWon}");
}

void MarkBoards(List<BingoBoard> bingoCards, int bingoNumber)
{
    foreach (var board in bingoCards)
    {
        if (board.TryGetValue(bingoNumber, out BingoCell? cell))
        {
            cell.Marked = true;
        }
    }
}

IEnumerable<BingoBoard>? FindWinners(List<BingoBoard> bingoCards)
{
    // check all rows and columns of each board
    List<BingoBoard> winners = new List<BingoBoard>();
    foreach (var card in bingoCards)
    {
        for (int i = 0; i < 5; i++)
        {
            bool rowWon = GetRowWon(card, i);
            bool colWon = GetColumnWon(card, i);
            if (rowWon || colWon)
            {
                winners.Add(card);
            }
        }
    }
    if (!winners.Any())
    {
        return null;
    }
        

    return winners;
}

bool GetRowWon(BingoBoard board, int rowIndex)
{
    return board.Where(x => x.Value.Row == rowIndex)
        .All(x => x.Value.Marked);
}

bool GetColumnWon(BingoBoard board, int colIndex)
{
    return board.Where(x => x.Value.Column == colIndex)
        .All(x => x.Value.Marked);
}

[DebuggerDisplay("Row: {Row}, Col: {Column}, Marked: {Marked}")]
public class BingoCell
{
    public int Row { get; init; }
    public int Column { get; init; }
    public bool Marked { get; set; }

    public BingoCell(int row, int column)
    {
        Row = row;
        Column = column;
    }
}