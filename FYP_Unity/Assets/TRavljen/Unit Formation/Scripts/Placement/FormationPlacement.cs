using System.Collections;
using System.Collections.Generic;
using TRavljen.UnitFormation.Formations;
using UnityEngine;

namespace TRavljen.UnitFormation.Placement
{

    /// <summary>
    /// Component for placing units in formation with a mouse drag.
    /// To disable this when placement is not desired, disable the component like you would any other.
    /// <code>(placement.enabled = false)</code>
    /// </summary>
    public class FormationPlacement : MonoBehaviour
    {

        #region Properties

        [Tooltip("Specifies the layer mask used for mouse raycasts in order " +
            "to find the drag positions in world/scene.")]
        [SerializeField] private LayerMask groundLayerMask;

        [Tooltip("Specifies maximal range of mouse raycasts.")]
        [SerializeField, Range(1, 5_000)] private float raycastMaxDistance = 100;

        [Tooltip("Specifies custom input by referencing its GameObject. " +
            "If any component implements IInputControl it will be retrieved from it, " +
            "otherwise default input control will be added based on your Input system (new or old).")]
        [SerializeField] private GameObject customInput;

        [Tooltip("Specifies the unit formation component for placement. " +
            "This can be set at runtime, but not during active placement." +
            "You can use one component and modify its list of units. Or use " +
            "multiple, one for each unit group and then set reference with Set method.")]
        [SerializeField] private UnitFormation unitFormation;

        [Tooltip("Specifies if formation positions are also calculated during active placement (used for visuals).")]
        [SerializeField] private bool alwaysCalculatePositions = true;

        [Tooltip("Specifies interval for calculating positions during active placement. " +
            "When threshold is 0, it will calculate positions each frame.")]
        [Range(0f, 1.5f)]
        [SerializeField] private float calculatePositionsIntervalThreshold = 0.01f;

        [Tooltip("Specifies if the visuals should be placed on valid ground and " +
            "not their absolute formation position (uneven terrain, water, " +
            "structures, environment can be in the way).")]
        [SerializeField] private bool placeVisualsOnGround = false;

        [Tooltip("Specifies the placement visuals used for rendering when the " +
            "unit placement is active.")]
        [SerializeField] private APlacementVisuals[] placementVisuals;

        internal IInputControl input;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private bool isPlacementActive = false;

        private float calculatePositionsInterval;

        public UnitFormation UnitFormation => unitFormation;
        public bool IsPlacementActive => isPlacementActive;

        #endregion

        #region Lifecycle

        private void Start()
        {
            SetupInput();
        }

        private void OnEnable()
        {
            RemoveInputListeners();
            AddInputListeners();
        }

        private void OnDisable() => RemoveInputListeners();

        private void Update()
        {
            if (isPlacementActive && PerformRaycast(out Vector3 position))
            {
                UpdatePlacement(position);
            }
        }

        #endregion

        #region Input

        private void SetupInput()
        {
            // Ignore if already set up
            if (this.input != null) return;

            if (customInput != null && customInput.TryGetComponent(out IInputControl input))
            {
                SetInput(input);
            }
            else if (TryGetComponent(out input))
            {
                SetInput(input);
            }
            else
            {
                AddDefaultInput();
            }
        }

        public void AddDefaultInput()
        {
            IInputControl newInput;
#if ENABLE_INPUT_SYSTEM
            // New input system backends are enabled.
            ActionInputControl control = gameObject.AddComponent<ActionInputControl>();
            control.SetupDefaultActionsIfNull();
            newInput = control;
#elif ENABLE_LEGACY_INPUT_MANAGER
            // Old input backends are enabled.
            newInput = gameObject.AddComponent<KeyInputControl>();
#endif
            SetInput(newInput);
        }

        private void AddInputListeners()
        {
            if (input != null)
            {
                input.OnPlacementActionPress.AddListener(StartPlacement);
                input.OnPlacementActionRelease.AddListener(FinishPlacement);
                input.OnPlacementActionCancel.AddListener(CancelPlacement);
            }
        }

        private void RemoveInputListeners()
        {
            if (input != null)
            {
                input.OnPlacementActionPress.RemoveListener(StartPlacement);
                input.OnPlacementActionRelease.RemoveListener(FinishPlacement);
                input.OnPlacementActionCancel.RemoveListener(CancelPlacement);
            }
        }

        #endregion

        #region Placement

        /// <summary>
        /// Starts the unit placement process. It will not start if
        /// UnitFormation was not set, if unit formation has no units or
        /// if raycast did not hit a valid ground.
        /// </summary>
        public void StartPlacement()
        {
            if (unitFormation != null && unitFormation.HasUnits && PerformRaycast(out Vector3 position))
            {
                isPlacementActive = true;
                startPosition = position;
                endPosition = position;

                foreach (var visuals in placementVisuals)
                    visuals.StartPlacement(position);
            }
        }

        /// <summary>
        /// Completes the process of placement, if placement is active.
        /// </summary>
        public void FinishPlacement()
        {
            if (!isPlacementActive) return;

            CancelPlacement();
            ApplyCurrentUnitFormation();
        }

        /// <summary>
        /// Cancels current placement if one is active.
        /// </summary>
        public void CancelPlacement()
        {
            if (!isPlacementActive) return;

            isPlacementActive = false;

            foreach (var visuals in placementVisuals)
                visuals.StopPlacement();
        }

        private void UpdatePlacement(Vector3 position)
        {
            endPosition = position;

            foreach (var visuals in placementVisuals)
                visuals.ContinuePlacement(position);

            if (alwaysCalculatePositions)
            {
                calculatePositionsInterval += Time.deltaTime;

                if (calculatePositionsInterval > calculatePositionsIntervalThreshold)
                {
                    calculatePositionsInterval = 0;

                    UpdateFormationVisuals();
                }
            }
        }

        private bool PerformRaycast(out Vector3 hitPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastMaxDistance, groundLayerMask))
            {
                hitPoint = hit.point;
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }

        private void UpdateFormationVisuals()
        {
            Vector3 direction = endPosition - startPosition;
            var formation = unitFormation.CalculatePositions(startPosition, direction);

            // Find ground for visuals
            if (placeVisualsOnGround)
            {
                for (int index = 0; index < formation.UnitPositions.Count; index++)
                {
                    formation.UnitPositions[index] = unitFormation.MovePositionOnGround(formation.UnitPositions[index]);
                }
            }

            foreach (var visuals in placementVisuals)
                visuals.OnFormationReady(formation);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Update unit formation used for placement.
        /// </summary>
        /// <param name="unitFormation">New unit formation</param>
        public void SetUnitFormation(UnitFormation unitFormation)
        {
            if (isPlacementActive)
            {
                Debug.LogWarning("UnitFormation cannot be changed during active placement!");
                return;
            }

            this.unitFormation = unitFormation;
        }

        /// <summary>
        /// Set new input reference.
        /// </summary>
        /// <param name="input">Input reference</param>
        public void SetInput(IInputControl input)
        {
            // Remove if there are any
            RemoveInputListeners();

            this.input = input;

            if (enabled)
                AddInputListeners();
        }

        /// <summary>
        /// Set current formation used calculating units positions.
        /// </summary>
        /// <param name="formation">New formation</param>
        public void SetFormation(IFormation formation)
        {
            unitFormation.SetUnitFormation(formation);
            ApplyCurrentUnitFormation();
        }

        /// <summary>
        /// Apply formation positions based on active placement or last active placement.
        /// </summary>
        public void ApplyCurrentUnitFormation()
        {
            Vector3 direction = endPosition - startPosition;
            unitFormation.ApplyCurrentUnitFormation(startPosition, direction);
        }

        #endregion

    }
}