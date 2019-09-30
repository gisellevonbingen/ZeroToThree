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

            this.BGMControl.ValueChanged += this.OnBGMControlValueChanged;
            this.EffectControl.ValueChanged += this.OnEffectControlValueChanged;
            this.BackButton.TouchButtonClick += this.OnBackButtonClick;
        }

        public override void OnOpen()
        {
            base.OnOpen();

            var am = GameManager.Instance.AudioManager;
            this.BGMControl.Value = am.Background.Volume;
            this.EffectControl.Value = am.Effect.Volume;
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

            var am = GameManager.Instance.AudioManager;

            var om = GameManager.Instance.OptionsManager;
            var options = om.Data;
            options.BackgroundVolume = am.Background.Volume;
            options.EffectVolume = am.Effect.Volume;

            om.Save();
        }

    }

}
