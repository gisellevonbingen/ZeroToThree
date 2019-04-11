using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ZeroToThree.Scripts;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class BoardSprite : MonoBehaviour
    {
        public GameObject BoardObject;
        public BoxCollider2D BoardCollider;
        public BlockSprite BlockPrefab;
        public int Offset;
        public float Gravity;

        public Board Board { get; private set; }

        private ObjectPool<BlockSprite> BlockSpritePool;
        private List<BlockSprite> BlockSprites;

        private void Awake()
        {
            this.BlockSpritePool = new ObjectPool<BlockSprite>(this.BlockPrefab);
            this.BlockSpritePool.Growed += this.OnBlockPoolGrowed;
            this.BlockSprites = new List<BlockSprite>();
        }

        public void Init(Board board)
        {
            this.Reset();

            this.Board = board;

            if (board != null)
            {
                board.BlocksGen += this.OnBlocksGen;
                board.BlocksFall += this.OnBlocksFall;
                board.BlocksUpdate += this.OnBlocksUpdate;
                board.BlocksBreak += this.OnBlocksBreak;
                board.BlocksExhaust += this.OnBlocksExhaust;

                board.ValueRange = this.BlockPrefab.ValueColors.Length;
            }

        }

        private void OnBlocksExhaust(object sender, BlocksEventArgs e)
        {
            var board = this.Board;

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);
                this.Destory(sprite);

                this.UpdateSpriteName(board, sprite);
            }

        }

        private void OnBlocksBreak(object sender, BlocksEventArgs e)
        {
            var board = this.Board;

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);
                sprite.BreakStart();

                this.UpdateSpriteName(board, sprite);
            }

        }

        private void OnBlocksUpdate(object sender, BlocksEventArgs e)
        {
            var board = this.Board;

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);
                sprite.MaskStart();

                this.UpdateSpriteName(board, sprite);
            }

        }

        private void OnBlocksFall(object sender, BlocksEventArgs e)
        {
            var board = this.Board;

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);
                board.GetBlockIndex(block, out var col, out var row);
                sprite.MoveStart(this.GetBlockGoalPosition(col, row));

                this.UpdateSpriteName(board, sprite);
            }

        }

        private Vector2Int GetBounds(Board board, Block[] blocks)
        {
            if (blocks.Length > 0)
            {
                bool first = true;
                var min = new Vector2Int(0, 0);
                var max = new Vector2Int(0, 0);

                foreach (var block in blocks)
                {
                    board.GetBlockIndex(block, out var x, out var y);

                    if (first == true)
                    {
                        first = false;
                        min = new Vector2Int(x, y);
                        max = new Vector2Int(x, y);
                    }
                    else
                    {
                        min.x = Math.Min(min.x, x);
                        min.y = Math.Min(min.y, y);
                        max.x = Math.Max(max.x, x);
                        max.y = Math.Max(max.y, y);
                    }

                }

                return new Vector2Int(max.x - min.x + 1, max.y - min.y + 1);
            }
            else
            {
                return new Vector2Int();
            }

        }

        private void OnBlocksGen(object sender, BlocksEventArgs e)
        {
            var board = this.Board;
            var blocks = e.Blocks;
            var bounds = this.GetBounds(board, blocks);

            float gravity = this.Gravity;

            foreach (var block in blocks)
            {
                board.GetBlockIndex(block, out var col, out var row);

                var sprite = this.BlockSpritePool.Obtain();
                sprite.Reset();
                sprite.MaskingDuration = 0.3F;
                sprite.Gravity = gravity;
                sprite.Block = block;

                sprite.transform.localPosition = this.GetBlockGenPosition(col, board.Height - bounds.y + row);
                sprite.MoveStart(this.GetBlockGoalPosition(col, row));

                this.BlockSprites.Add(sprite);
                this.UpdateSpriteName(board, sprite);
            }

        }

        private void UpdateSpriteName(Board board, BlockSprite sprite)
        {
            if (board == null || sprite == null)
            {
                return;
            }

            var block = sprite.Block;
            board.GetBlockIndex(block, out var col, out var row);
            sprite.name = $"Block({col},{row})=" + block.Value;
        }

        public void Reset()
        {
            var pool = this.BlockSpritePool;

            foreach (var sprite in pool.GetPool())
            {
                sprite.Reset();
            }

            pool.FreeAll();
            this.BlockSprites.Clear();

            var board = this.Board;

            if (board != null)
            {
                board.Clear();
            }

        }

        private void OnBlockPoolGrowed(object sender, PoolGrowEventArgs<BlockSprite> e)
        {
            var sprite = e.Obj;
            sprite.transform.SetParent(this.transform);
            sprite.Click += this.OnBlockClick;
            sprite.Breaked += this.OnBlockBreaked;
        }

        private BlockSprite FindSprite(Block block)
        {
            return this.BlockSprites.FirstOrDefault(b => b.Block == block);
        }

        private void OnBlockBreaked(object sender, EventArgs e)
        {
            var sprite = (BlockSprite)sender;
            this.Destory(sprite);
        }

        private void Destory(BlockSprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            this.BlockSpritePool.Free(sprite);
            this.BlockSprites.Remove(sprite);
        }

        private void OnBlockClick(object sender, EventArgs e)
        {
            if (this.CanStep() == true)
            {
                var sprite = (BlockSprite)sender;
                var block = sprite.Block;
                var index = this.Board.GetBlockIndex(block);

                this.Board.ClickIndex = index;
            }

        }

        public Vector2 GetBlockGenPosition(int col, int row)
        {
            var size = this.BlockPrefab.GetTileSize();
            var xi = col - 1;
            var yi = row - 1;
            var offset = this.Offset;

            var position = new Vector2
            {
                x = +xi * (size.x + offset),
                y = this.BoardCollider.size.y - yi * (size.y + offset)
            };

            return position;
        }

        public Vector2 GetBlockGoalPosition(int col, int row)
        {
            var size = this.BlockPrefab.GetTileSize();
            var xi = col - 1;
            var yi = row - 1;
            var offset = this.Offset;

            var position = new Vector2
            {
                x = +xi * (size.x + offset),
                y = -yi * (size.y + offset)
            };

            return position;
        }

        public bool CanStep()
        {
            if (this.BlockSprites.Any(b => b.AnimatingRoutine != null) == true)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {
            var board = this.Board;

            if (board != null)
            {
                if (this.CanStep() == true)
                {
                    board.Step();
                }

            }

        }

    }

}
