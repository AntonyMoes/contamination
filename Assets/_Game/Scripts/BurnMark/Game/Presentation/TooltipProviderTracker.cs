using System.Linq;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class TooltipProviderTracker : IFrameProcessor {
        private readonly PointerRaycastProvider _pointerRaycastProvider;

        private readonly UpdatedValue<(ITooltipProvider, Vector2)> _tooltipProvider = new UpdatedValue<(ITooltipProvider, Vector2)>();
        public IUpdatedValue<(ITooltipProvider, Vector2)> TooltipProvider => _tooltipProvider;

        public TooltipProviderTracker(PointerRaycastProvider pointerRaycastProvider) {
            _pointerRaycastProvider = pointerRaycastProvider;
        }

        public void ProcessFrame(float deltaTime) {
            var (provider, res) = _pointerRaycastProvider.RaycastResults
                .Select(res => {
                    var provider = res.gameObject.GetComponentInParent<ITooltipProvider>();
                    return provider != null ? (provider, res) : default;
                })
                .FirstOrDefault();
             _tooltipProvider.Value = (provider, res.screenPosition);
        }
    }
}