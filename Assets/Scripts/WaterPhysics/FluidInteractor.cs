using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class FluidInteractor : MonoBehaviour
{
    private float _waterDrag;
    private float _waterAngularDrag;
    private float _airDrag;
    private float _airAngularDrag;

    [SerializeField]
    private List<Transform> _floaters = new List<Transform>();
    [SerializeField]
    private float _dampeningFactor = .1f;
    [SerializeField]
    private float _floatStrength = 1f;

    public Rigidbody Rigidbody { get; protected set; }
    public Collider Collider { get; protected set; }
    public List<Transform> Floaters => _floaters;
    public float DampeningFactor => _dampeningFactor;
    public float FloatStrength => _floatStrength;
    public float Volume { get; protected set; }

    protected virtual void Start()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Volume = CalculateVolume();

        _airDrag = Rigidbody.drag;
        _airAngularDrag = Rigidbody.angularDrag;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Fluid fluid))
        {
            _waterDrag = fluid.Drag;
            _waterAngularDrag = fluid.AngularDrag;

            Rigidbody.drag = _waterDrag;
            Rigidbody.angularDrag = _waterAngularDrag;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Fluid>())
        {
            Rigidbody.drag = _airDrag;
            Rigidbody.angularDrag = _airAngularDrag;
        }
    }

    private float CalculateVolume()
    {
        return Collider.bounds.size.x * Collider.bounds.size.y * Collider.bounds.size.z;
    }
}