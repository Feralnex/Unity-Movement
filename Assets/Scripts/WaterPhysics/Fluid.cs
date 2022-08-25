using System.Collections.Generic;
using UnityEngine;

class Fluid : MonoBehaviour
{
    private List<FluidInteractor> _fluidInteractors;

    [SerializeField]
    private float _density = 1f;
    [SerializeField]
    private float _drag = 1f;
    [SerializeField]
    private float _angularDrag = 1f;

    public float Density => _density;
    public float Drag => _drag;
    public float AngularDrag => _angularDrag;

    protected void Awake()
    {
        _fluidInteractors = new List<FluidInteractor>();
    }

    protected void FixedUpdate()
    {
        foreach (FluidInteractor fluidInteractor in _fluidInteractors)
        {
            foreach (Transform floater in fluidInteractor.Floaters)
            {
                float difference = floater.position.y - transform.position.y;

                if (difference < 0)
                {
                    Vector3 buoyancy = (Density * fluidInteractor.FloatStrength * fluidInteractor.Volume * Mathf.Abs(difference) * Physics.gravity.magnitude * Vector3.up);

                    fluidInteractor.Rigidbody.AddForceAtPosition(buoyancy, floater.position, ForceMode.Force);
                    fluidInteractor.Rigidbody.AddForceAtPosition((fluidInteractor.DampeningFactor / fluidInteractor.Floaters.Count) * fluidInteractor.Volume * fluidInteractor.Rigidbody.velocity, floater.position, ForceMode.Force);
                }
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FluidInteractor fluidInteractor) &&
            !_fluidInteractors.Contains(fluidInteractor)) _fluidInteractors.Add(fluidInteractor);
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out FluidInteractor fluidInteractor) &&
            _fluidInteractors.Contains(fluidInteractor)) _fluidInteractors.Remove(fluidInteractor);
    }
}