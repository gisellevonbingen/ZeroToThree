﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIDialogGameOver : UIDialog
    {
        public UIImage Back;
        public UILabel Text;
        public UILabel ScoreLabel;
        public UIImage BackButton;
        public UIImage ResetButton;

        public AudioClip GameOverAudio;

        public float DialogFadeInDuration;
        public float ScoreFadeInDuration;
        public float ButtonFadeInDuration;

        public GameSession Session { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.BackButton.TouchButtonClick += this.OnBackButtonClick;
            this.ResetButton.TouchButtonClick += this.OnResetButtonClick;
        }

        private void OnBackButtonClick(object sender, UITouchEventArgs e)
        {
            this.Close();

            var ui = GameManager.Instance.UIManager;
            ui.ShowScreen(ui.Main);
        }

        private void OnResetButtonClick(object sender, UITouchEventArgs e)
        {
            this.Close();

            var ui = GameManager.Instance.UIManager;
            ui.Game.ResetGame();
        }

        protected override void OnOpened(UIEventArgs e)
        {
            base.OnOpened(e);

            var session = this.Session;

            this.ScoreLabel.gameObject.SetActive(false);
            this.BackButton.gameObject.SetActive(false);
            this.ResetButton.gameObject.SetActive(false);

            this.SetAlpha(this.Back.Image, 0.0F);
            this.SetAlpha(this.Text.Text, 0.0F);

            this.Actions.Clear();
            this.Actions.Add(new UIActionTimeDelegate()
            {
                Duration = this.DialogFadeInDuration,
                ActHandler = (target, delta, percent) =>
                {
                    this.SetAlpha(this.Back.Image, percent);
                    this.SetAlpha(this.Text.Text, percent);
                }

            });
            this.Actions.Add(new UIActionTimeDelegate()
            {
                Duration = this.ScoreFadeInDuration,
                ActHandler = (target, delta, percent) =>
                {
                    var data = GameManager.Instance.StatisticsManager.Data;
                    var highScore = data.HighScore;
                    var highCombo = data.HighCombo;

                    var lines = new List<string>();
                    lines.Add($"High Score : {(highScore * percent).ToNumberString()}");
                    lines.Add($"High Combo : {(highCombo * percent).ToNumberString()}");
                    lines.Add($"");
                    lines.Add($"Score : {(session.Score * percent).ToNumberString()}");
                    lines.Add($"Combo : {(session.HighCombo * percent).ToNumberString()}");

                    this.ScoreLabel.gameObject.SetActive(true);
                    this.ScoreLabel.Text.text = string.Join("\n", lines);
                    this.SetAlpha(this.ScoreLabel.Text, percent);
                }

            });
            this.Actions.Add(new UIActionTimeDelegate()
            {
                Duration = this.ScoreFadeInDuration,
                ActHandler = (target, delta, percent) =>
                {
                    this.BackButton.gameObject.SetActive(true);
                    this.SetAlpha(this.BackButton.Image, percent);
                    this.ResetButton.gameObject.SetActive(true);
                    this.SetAlpha(this.ResetButton.Image, percent);
                }

            });

            GameManager.Instance.AudioManager.Background.Play(this.GameOverAudio);
        }

        protected override void OnClosed(UIEventArgs e)
        {
            base.OnClosed(e);

            GameManager.Instance.AudioManager.Background.Play(null);
        }

        private void SetAlpha(Graphic g, float alpha)
        {
            var color = g.color;
            color.a = alpha;

            g.color = color;
        }

    }

}
