using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.ZeroToThree.Scripts.BlocksEventArgs;

namespace Assets.ZeroToThree.Scripts
{
    public class Board
    {
        private readonly Block[] Blocks;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first">모든 해법을 찾지 않음, 1개 이상 해법이 존재하는 경우 해당 해법만 반환</param>
        /// <returns>null : 해답을 찾을 수 없는 상태, Count=0 : 해답 없음(게임오버)</returns>
        public List<BoardSolution> Solve(bool first)
        {
            var blocks = this.Blocks;

            if (blocks.Any(b => b == null) == true)
            {
                return null;
            }
            else if (this.FindCompleteLines(new List<Block>()) > 0)
            {
                return null;
            }

            var solutions = new List<BoardSolution>();

            var unmasks = blocks.Where(b => b != null && b.Masking == false).ToList();
            var connetedSets = new List<Block[]>();

            while (true)
            {
                var block = unmasks.FirstOrDefault();

                if (block == null)
                {
                    break;
                }

                var conneteds = this.GetConnectedBlocks(block).ToArray();
                EnumerableUtils.RemoveAll(unmasks, conneteds);
                connetedSets.Add(conneteds);
            }

            foreach (var set in connetedSets)
            {
                var clone = new Board(this);

                foreach (var block in set)
                {
                    var index = this.GetBlockIndex(block);
                    var cblock = clone.Blocks[index];
                    clone.GrowValue(cblock);
                }

                var completeLines = clone.FindCompleteLines(new List<Block>());
                var item = set.Select(b => this.GetBlockIndex(b)).ToArray();

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
                else
                {
                    var childSolutions = clone.Solve(first);

                    if (childSolutions != null && childSolutions.Count > 0)
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

            }

            return solutions;
        }

        public void Clear()
        {
            this.ValueRange = 0;
            this.ClickIndex = -1;

            this.BreakAll();
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

        public void BreakAll()
        {
            var blocks = this.Blocks;
            var breaks = new List<Block>();

            for (int i = 0; i < blocks.Length; i++)
            {
                var block = blocks[i];

                if (block != null)
                {
                    breaks.Add(block);
                }

            }

            this.OnBlocksBreak(new BlocksEventArgs(breaks.ToArray(), EventPhase.Pre));

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = null;
            }

            this.OnBlocksBreak(new BlocksEventArgs(breaks.ToArray(), EventPhase.Post));
        }

        public bool Step()
        {
            if (this.BreakCompleteLines() == true)
            {
                return true;
            }

            if (this.TryFall() == true)
            {
                return true;
            }

            if (this.GenBlankBlocks() == true)
            {
                return true;
            }

            if (this.UpdateClicked() == true)
            {
                return true;
            }

            return false;
        }

        private void GetConnectedBlocks(Block baseBlock, List<Block> blocks)
        {
            this.GetBlockIndex(baseBlock, out var baseCol, out var baseRow);

            foreach (var direction in BlockDirection.Values)
            {
                var col = baseCol + direction.X;
                var row = baseRow + direction.Y;

                if (this.TryGetBlock(col, row, out var block) == true)
                {
                    if (block != null && blocks.Contains(block) == false)
                    {
                        if (baseBlock.Value == block.Value)
                        {
                            blocks.Add(block);
                            this.GetConnectedBlocks(block, blocks);
                        }

                    }

                }

            }

        }

        public List<Block> GetConnectedBlocks(Block block)
        {
            var blocks = new List<Block> { block };
            this.GetConnectedBlocks(block, blocks);

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

        public bool TryGetBlock(int col, int row, out Block block)
        {
            block = null;

            if (this.IsInBoard(col, row) == true)
            {
                block = this[col, row];
                return true;
            }

            return false;
        }

        public bool IsInBoard(int index)
        {
            return 0 <= index && index < this.Blocks.Length;
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
                        var block = new Block()
                        {
                            Value = UnityEngine.Random.Range(0, this.ValueRange)
                        };

                        this[col, row] = block;
                        blocks.Add(block);
                    }

                }

            }

            if (blocks.Count > 0)
            {
                this.OnBlocksGen(new BlocksEventArgs(blocks.ToArray(), EventPhase.Post));
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
                    var connecteds = this.GetConnectedBlocks(block);

                    this.OnBlocksUpdate(new BlocksEventArgs(connecteds.ToArray(), EventPhase.Pre));

                    foreach (var cb in connecteds)
                    {
                        this.GrowValue(cb);
                    }

                    this.OnBlocksUpdate(new BlocksEventArgs(connecteds.ToArray(), EventPhase.Post));

                    return true;
                }

            }

            return false;
        }

        public void GrowValue(Block block)
        {
            block.Value = (block.Value + 1) % this.ValueRange;
            block.Masking = true;
        }

        private bool BreakCompleteLines()
        {
            var blockList = new List<Block>();
            int lines = this.FindCompleteLines(blockList);

            if (lines > 0)
            {
                var blocks = blockList.Distinct().ToArray();
                this.OnLineComplete(new BoardLineEventArgs(lines, blocks));

                this.OnBlocksBreak(new BlocksEventArgs(blocks, EventPhase.Pre));

                foreach (var block in blocks)
                {
                    var index = this.GetBlockIndex(block);
                    this[index] = null;
                }

                this.OnBlocksBreak(new BlocksEventArgs(blocks, EventPhase.Post));

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
                    var block = this[col, row];

                    if (this.TryFall(block, true) > 0)
                    {
                        blocks.Add(block);
                    }

                }

            }

            if (blocks.Count > 0)
            {
                this.OnBlocksFall(new BlocksEventArgs(blocks.ToArray(), EventPhase.Pre));

                foreach (var block in blocks)
                {
                    this.TryFall(block, false);
                }

                this.OnBlocksFall(new BlocksEventArgs(blocks.ToArray(), EventPhase.Post));

                return true;
            }

            return false;
        }

        private int TryFall(Block block, bool simulate)
        {
            this.GetBlockIndex(block, out var col, out var row);

            int fallHeight = 0;

            for (int i = row; i < this.Height - 1; i++)
            {
                var downRow = i + 1;

                var b = this[col, i];

                if (b == null)
                {
                    break;
                }

                var down = this[col, downRow];

                if (down == null)
                {
                    if (simulate == false)
                    {
                        this[col, downRow] = b;
                        this[col, i] = null;
                    }

                    fallHeight++;
                }
                else if (simulate == false)
                {
                    break;
                }

            }

            return fallHeight;
        }

    }

}
