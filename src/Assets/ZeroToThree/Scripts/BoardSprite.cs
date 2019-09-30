using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ZeroToThree.Scripts;
using Assets.ZeroToThree.Scripts.UI;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class BoardSprite : UIImage
    {
        public GameObject BoardObject;
        public BlockSprite BlockPrefab;
        public int Offset;

        public Board Board { get; private set; }

        private ObjectPool<BlockSprite> BlockSpritePool;
        private List<BlockSprite> BlockSprites;

        protected override void OnAwake()
        {
            base.OnAwake();

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

                board.ValueRange = this.BlockPrefab.ValueColors.Length;
            }

        }

        private void OnBlocksBreak(object sender, BlocksEventArgs e)
        {
            var board = this.Board;
            var clips = new List<AudioClip>();

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);

                if (sprite != null)
                {
                    sprite.BreakStart();
                    clips.Add(sprite.BreakAudio);

                    this.UpdateSpriteName(board, sprite);
                }

            }

            GameManager.Instance.AudioManager.Effect.PlayDistincts(clips);
        }

        private void OnBlocksUpdate(object sender, BlocksEventArgs e)
        {
            var board = this.Board;
            var clips = new List<AudioClip>();

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);

                if (sprite != null)
                {
                    sprite.MaskStart();
                    clips.Add(sprite.MaskAudio);

                    this.UpdateSpriteName(board, sprite);
                }

            }

            GameManager.Instance.AudioManager.Effect.PlayDistincts(clips);
        }

        private void OnBlocksFall(object sender, BlocksEventArgs e)
        {
            var board = this.Board;

            foreach (var block in e.Blocks)
            {
                var sprite = this.FindSprite(block);

                if (sprite != null)
                {
                    board.GetBlockIndex(block, out var col, out var row);
                    sprite.MoveStart(this.GetBlockGoalPosition(col, row));

                    this.UpdateSpriteName(board, sprite);
                }

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

            foreach (var block in blocks)
            {
                board.GetBlockIndex(block, out var col, out var row);

                var sprite = this.BlockSpritePool.Obtain();
                sprite.Reset();
                sprite.Block = block;

                sprite.transform.localPosition = this.GetBlockGenPosition(col, board.Height - bounds.y + row);
                sprite.MoveStart(this.GetBlockGoalPosition(col, row));
                sprite.UpdateValue();

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
            sprite.TouchButtonClick += this.OnBlockClick;
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

        private void OnBlockClick(object sender, UITouchButtonEventArgs e)
        {
            var sprite = (BlockSprite)sender;
            var block = sprite.Block;

            if (e.Button == 0)
            {
                if (this.CanStep() == true && sprite.CanMask() == true)
                {
                    var index = this.Board.GetBlockIndex(block);
                    this.Board.ClickIndex = index;
                }

            }
            else if (ApplicationUtils.IsPlayingInEditor() == true)
            {
                this.Board.GrowValue(block);
                this.OnBlocksUpdate(this, new BlocksEventArgs(new Block[] { block }));
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
                y = this.transform.sizeDelta.y - yi * (size.y + offset)
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
            if (this.BlockSprites.Any(b => b.Actions.Count > 0) == true)
            {
                return false;
            }

            return true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

    }

}
