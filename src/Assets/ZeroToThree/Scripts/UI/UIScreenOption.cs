using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIScreenOption : UIScreen
    {
        public UIVolumeControl BGMControl;
        public UIVolumeControl EffectControl;
        public UIImage BackButton;

        protected override void OnAwake()
        {
            base.OnAwake();

            var am = GameManager.Instance.AudioManager;

            this.BGMControl.Value = am.Background.Volume;
            this.BGMControl.ValueChanged += this.OnBGMControlValueChanged;

            this.EffectControl.Value = am.Effect.Volume;
            this.EffectControl.ValueChanged += this.OnEffectControlValueChanged;

            this.BackButton.TouchButtonClick += this.OnBackButtonClick;
        }

        private void OnBGMControlValueChanged(object sender, EventArgs e)
        {
            GameManager.Instance.AudioManager.Background.Volume = this.BGMControl.Value;
        }

        private void OnEffectControlValueChanged(object sender, EventArgs e)
        {
            GameManager.Instance.AudioManager.Effect.Volume = this.EffectControl.Value;
        }

        private void OnBackButtonClick(object sender, UITouchEventArgs e)
        {
            var um = GameManager.Instance.UIManager;
            um.ShowScreen(um.Main);
        }

    }

}
