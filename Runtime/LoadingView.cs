// ArchNet · Load Scene Extension — loading screen
// Shows the progress (a bar that fills from the left, a percentage) and a tip while a scene loads.
// Docs: https://archnet-plateform.fr/#ModuleScenes

using ArchNet.Core;
using TMPro;
using UnityEngine;

namespace ArchNet.Extension
{
    [RequireComponent(typeof(UIPanel))]
    public class LoadingView : MonoBehaviour
    {
        [Tooltip("Stretched from the left: its right edge follows the progress.")]
        [SerializeField] RectTransform _barFill;
        [SerializeField] TMP_Text _percent;
        [SerializeField] TMP_Text _tip;

        UIPanel _panel;

        UIPanel Panel => _panel != null ? _panel : _panel = GetComponent<UIPanel>();

        public void Show(string tip)
        {
            if (_tip != null)
            {
                _tip.text = tip ?? "";
                _tip.gameObject.SetActive(!string.IsNullOrEmpty(tip));
            }
            Panel.Show();
        }

        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            if (_barFill != null) _barFill.anchorMax = new Vector2(progress, _barFill.anchorMax.y);
            if (_percent != null) _percent.text = $"{Mathf.RoundToInt(progress * 100f)} %";
        }

        public void Hide() => Panel.Hide();
    }
}
