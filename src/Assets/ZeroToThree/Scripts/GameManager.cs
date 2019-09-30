using Assets.ZeroToThree.Scripts.Audio;
using Assets.ZeroToThree.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public UIManager UIManager;
        public AudioManager AudioManager;

        public StatisticsManager StatisticsManager { get; private set; }

        private void Awake()
        {
            Instance = this;

            var sm = this.StatisticsManager = new StatisticsManager();
            sm.Load();
        }

        private void Start()
        {
            this.AudioManager.Background.Volume = 1.0F;
            this.AudioManager.Effect.Volume = 1.0F;
        }

        public GameSession CreateSession()
        {
            var session = new GameSession();

            var sm = this.StatisticsManager;
            sm.Data.SessionCreated++;
            sm.Save();

            return session;
        }

        public void OnGameOver(GameSession session)
        {
            var sm = this.StatisticsManager;
            sm.Data.HighScore = Math.Max(sm.Data.HighScore, session?.Score ?? 0);
            sm.Data.HighCombo = Math.Max(sm.Data.HighCombo, session?.HighCombo ?? 0);
            sm.Save();
        }

    }

}
