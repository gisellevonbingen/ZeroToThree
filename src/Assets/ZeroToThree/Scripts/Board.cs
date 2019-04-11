using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

                    if (block != null && baseBlock.Value == block.Value && blocks.Contains(block) == false)
                    {
                        blocks.Add(block);
                        this.GetConnectedBlocks(block, blocks);
                    }

                }

            }

        }

        private List<Block> GetConnectedBlocks(Block block)
        {
            var blocks = new List<Block>();
            blocks.Add(block);

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
                    var connecteds = this.GetConnectedBlocks(block);
                    var valueRange = this.ValueRange;

                    foreach (var cb in connecteds)
                    {
                        cb.Value = (cb.Value + 1) % valueRange;
                        cb.Masking = true;
                    }

                    this.OnBlocksUpdate(new BlocksEventArgs(connecteds.ToArray()));

                    return true;
                }

            }

            return false;
        }

        private bool BreakCompleteLines()
        {
            var blockList = new List<Block>();
            var lines = 0;
            lines += this.FindCompleteCols(blockList);
            lines += this.FinsCompleteRows(blockList);

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
