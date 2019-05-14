using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.ZeroToThree.Scripts
{
    public class Board
    {
        private Block[] Blocks;

        public int ValueRange { get; set; }
        public int ClickIndex { get; set; }

        public int Width => 3;
        public int Height => 3;

        public Board()
        {
            this.Blocks = new Block[this.Width * this.Height];
            this.Clear();
        }

        public Board(Board other)
        {
            var oblocks = other.Blocks;
            this.Blocks = new Block[oblocks.Length];
            this.Clear();

            var tblocks = this.Blocks;

            for (int i = 0; i < tblocks.Length; i++)
            {
                tblocks[i] = other.Blocks[i]?.Clone();
            }

            this.ValueRange = other.ValueRange;
            this.ClickIndex = other.ClickIndex;
        }

        public List<BoardSolution> Solve(bool first)
        {
            var solutions = new List<BoardSolution>();
            var blocks = this.Blocks;

            if (blocks.Any(b => b == null) == true)
            {
                return solutions;
            }

            var unmasks = blocks.Where(b => b != null && b.Masking == false).ToList();
            var unmaskPairs = new List<Block[]>();

            while (true)
            {
                var block = unmasks.FirstOrDefault();

                if (block == null)
                {
                    break;
                }

                var conneteds = this.GetConnectedBlocks(block, (bb, b) => bb.Value == b.Value).ToArray();
                EnumerableUtils.RemoveAll(unmasks, conneteds);
                unmaskPairs.Add(conneteds);
            }

            foreach (var pairs in unmaskPairs)
            {
                var clone = new Board(this);

                foreach (var block in pairs)
                {
                    var index = this.GetBlockIndex(block);
                    var cblock = clone.Blocks[index];
                    clone.GrowValue(cblock);
                }

                var completeLines = clone.FindCompleteLines(new List<Block>());
                var item = pairs.Select(b => this.GetBlockIndex(b)).ToArray();

                if (completeLines > 0)
                {
                    var solution = new BoardSolution();
                    solution.Indices.Add(item);
                    solutions.Add(solution);

                    if (first == true)
                    {
                        break;
                    }

                }

                var childSolutions = clone.Solve(first);

                if (childSolutions.Count > 0)
                {
                    foreach (var sol in childSolutions)
                    {
                        var solution = new BoardSolution();
                        solution.Indices.Add(item);
                        solution.Indices.AddRange(sol.Indices);
                        solutions.Add(solution);

                        if (first == true)
                        {
                            break;
                        }

                    }

                    if (first == true)
                    {
                        break;
                    }

                }

            }

            return solutions;
        }

        public void Clear()
        {
            this.ValueRange = 0;
            this.ClickIndex = -1;

            this.ExhaustAll();
        }

        public event EventHandler<BlocksEventArgs> BlocksGen;

        private void OnBlocksGen(BlocksEventArgs e)
        {
            this.BlocksGen?.Invoke(this, e);
        }

        public event EventHandler<BlocksEventArgs> BlocksBreak;

        private void OnBlocksBreak(BlocksEventArgs e)
        {
            this.BlocksBreak?.Invoke(this, e);
        }

        public event EventHandler<BlocksEventArgs> BlocksUpdate;

        private void OnBlocksUpdate(BlocksEventArgs e)
        {
            this.BlocksUpdate?.Invoke(this, e);
        }

        public event EventHandler<BlocksEventArgs> BlocksFall;

        private void OnBlocksFall(BlocksEventArgs e)
        {
            this.BlocksFall?.Invoke(this, e);
        }

        public event EventHandler<BoardLineEventArgs> LineComplete;

        private void OnLineComplete(BoardLineEventArgs e)
        {
            this.LineComplete?.Invoke(this, e);
        }

        public event EventHandler<BlocksEventArgs> BlocksExhaust;

        protected virtual void OnBlocksExhaust(BlocksEventArgs e)
        {
            this.BlocksExhaust?.Invoke(this, e);
        }

        public void ExhaustAll()
        {
            var blocks = this.Blocks;
            var exhauteds = new List<Block>();

            for (int i = 0; i < blocks.Length; i++)
            {
                var block = blocks[i];
                blocks[i] = null;

                if (block != null)
                {
                    exhauteds.Add(block);
                }

            }

            this.OnBlocksExhaust(new BlocksEventArgs(exhauteds.ToArray()));
        }

        public bool Step()
        {
            if (this.Step0() == true)
            {
                using (var fs = new FileStream("test.txt", FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        var solutions = this.Solve(false);
                        solutions.Sort((o1, o2) => o1.Indices.Count.CompareTo(o2.Indices.Count));

                        sw.WriteLine("Solutions : " + solutions.Count);

                        foreach (var solution in solutions)
                        {
                            var line = string.Join(", ", solution.Indices.Select(array => $"[{string.Join(", ", array)}]"));
                            sw.WriteLine(line);
                        }

                    }

                }

                return true;
            }

            return false;
        }

        private bool Step0()
        {
            if (this.BreakCompleteLines() == true)
            {
                UnityEngine.Debug.Log("BreakCompleteLines");
                return true;
            }

            if (this.TryFall() == true)
            {
                UnityEngine.Debug.Log("TryFall");
                return true;
            }

            if (this.GenBlankBlocks() == true)
            {
                UnityEngine.Debug.Log("GenBlankBlocks");
                return true;
            }

            if (this.UpdateClicked() == true)
            {
                UnityEngine.Debug.Log("UpdateClicked");
                return true;
            }

            return false;
        }

        private void GetConnectedBlocks(Block baseBlock, List<Block> blocks, Func<Block, Block, bool> func)
        {
            var directions = new List<Vector2Int>();
            directions.Add(Vector2Int.left);
            directions.Add(Vector2Int.up);
            directions.Add(Vector2Int.right);
            directions.Add(Vector2Int.down);

            this.GetBlockIndex(baseBlock, out var baseCol, out var baseRow);

            foreach (var direction in directions)
            {
                var col = baseCol + direction.x;
                var row = baseRow + direction.y;

                if (this.IsInBoard(col, row) == true)
                {
                    var block = this[col, row];

                    if (block != null && blocks.Contains(block) == false)
                    {
                        if (func == null || func(baseBlock, block) == true)
                        {
                            blocks.Add(block);
                            this.GetConnectedBlocks(block, blocks, func);
                        }

                    }

                }

            }

        }

        private List<Block> GetConnectedBlocks(Block block, Func<Block, Block, bool> func)
        {
            var blocks = new List<Block>();
            blocks.Add(block);

            this.GetConnectedBlocks(block, blocks, func);

            return blocks;
        }

        public int GetBlockIndex(Block block)
        {
            return Array.IndexOf(this.Blocks, block);
        }

        public int GetBlockIndex(int col, int row)
        {
            return row * this.Width + col;
        }

        public void GetBlockIndex(Block block, out int col, out int row)
        {
            var index = this.GetBlockIndex(block);
            this.GetBlockIndex(index, out col, out row);
        }

        public void GetBlockIndex(int index, out int col, out int row)
        {
            row = index / this.Width;
            col = index % this.Width;
        }

        public bool IsInBoard(int col, int row)
        {
            if (0 <= col && col < this.Width)
            {
                if (0 <= row && row < this.Height)
                {
                    return true;
                }

            }

            return false;
        }

        public Block this[int col, int row]
        {
            get => this.Blocks[this.GetBlockIndex(col, row)];

            private set => this.Blocks[this.GetBlockIndex(col, row)] = value;
        }

        public Block this[int index]
        {
            get => this.Blocks[index];

            private set => this.Blocks[index] = value;
        }

        public bool GenBlankBlocks()
        {
            var blocks = new List<Block>();

            for (int row = 0; row < this.Height; row++)
            {
                for (int col = 0; col < this.Width; col++)
                {
                    if (this[col, row] == null)
                    {
                        var block = new Block();
                        block.Value = UnityEngine.Random.Range(0, this.ValueRange);

                        this[col, row] = block;
                        blocks.Add(block);
                    }

                }

            }

            if (blocks.Count > 0)
            {
                this.OnBlocksGen(new BlocksEventArgs(blocks.ToArray()));
                return true;
            }

            return false;
        }

        private bool UpdateClicked()
        {
            var index = this.ClickIndex;

            if (index > -1)
            {
                this.ClickIndex = -1;
                var block = this[index];

                if (block.Masking == false)
                {
                    var connecteds = this.GetConnectedBlocks(block, (bb, b) => bb.Value == b.Value);

                    foreach (var cb in connecteds)
                    {
                        this.GrowValue(cb);
                    }

                    this.OnBlocksUpdate(new BlocksEventArgs(connecteds.ToArray()));

                    return true;
                }

            }

            return false;
        }

        private void GrowValue(Block block)
        {
            block.Value = (block.Value + 1) % this.ValueRange;
            block.Masking = true;
        }

        private bool BreakCompleteLines()
        {
            var blockList = new List<Block>();
            int lines = FindCompleteLines(blockList);

            if (lines > 0)
            {
                var blocks = blockList.Distinct().ToArray();
                this.OnLineComplete(new BoardLineEventArgs(lines, blocks));

                foreach (var block in blocks)
                {
                    var index = this.GetBlockIndex(block);
                    this[index] = null;
                }

                this.OnBlocksBreak(new BlocksEventArgs(blocks));

                return true;
            }

            return false;
        }

        private int FindCompleteLines(List<Block> blockList)
        {
            var lines = 0;
            lines += this.FindCompleteCols(blockList);
            lines += this.FinsCompleteRows(blockList);

            return lines;
        }

        private int FinsCompleteRows(List<Block> list)
        {
            int lines = 0;

            for (int row = 0; row < this.Height; row++)
            {
                var blocks = this.GetCompleteBlocks(0, this.Width, row, row + 1);

                if (blocks != null)
                {
                    list.AddRange(blocks);
                    lines++;
                }

            }

            return lines;
        }

        private int FindCompleteCols(List<Block> list)
        {
            int lines = 0;

            for (int col = 0; col < this.Width; col++)
            {
                var blocks = this.GetCompleteBlocks(col, col + 1, 0, this.Height);

                if (blocks != null)
                {
                    list.AddRange(blocks);
                    lines++;
                }

            }

            return lines;
        }

        private List<Block> GetCompleteBlocks(int colStart, int colEnd, int rowStart, int rowEnd)
        {
            if (this.IsLineComplete(colStart, colEnd, rowStart, rowEnd) == true)
            {
                var list = new List<Block>();
                for (int col = colStart; col < colEnd; col++)
                {
                    for (int row = rowStart; row < rowEnd; row++)
                    {
                        list.Add(this[col, row]);
                    }

                }

                return list;
            }

            return null;
        }

        private bool IsLineComplete(int colStart, int colEnd, int rowStart, int rowEnd)
        {
            int? value = null;

            for (int col = colStart; col < colEnd; col++)
            {
                for (int row = rowStart; row < rowEnd; row++)
                {
                    var block = this[col, row];

                    if (block == null)
                    {
                        return false;
                    }

                    if (value.HasValue == true)
                    {
                        if (value.Value != block.Value)
                        {
                            return false;
                        }

                    }
                    else
                    {
                        value = block.Value;
                    }

                }

            }

            return true;
        }

        private bool TryFall()
        {
            var blocks = new List<Block>();

            for (int row = this.Height - 2; row > -1; row--)
            {
                for (int col = 0; col < this.Width; col++)
                {
                    var block = this.TryFall(col, row);

                    if (block != null)
                    {
                        blocks.Add(block);
                    }

                }

            }

            if (blocks.Count > 0)
            {
                this.OnBlocksFall(new BlocksEventArgs(blocks.ToArray()));

                return true;
            }

            return false;
        }

        private Block TryFall(int col, int row)
        {
            var block = this[col, row];

            if (block != null)
            {
                var downRow = row + 1;
                var down = this[col, downRow];

                if (down == null)
                {
                    this[col, downRow] = block;
                    this[col, row] = null;

                    return block;
                }

            }

            return null;
        }

    }

}
