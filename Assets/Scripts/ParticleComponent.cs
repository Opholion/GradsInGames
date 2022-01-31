using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleComponent : MonoBehaviour
{

    const float RGB_MAX_VAL = 1.0f / 255.0f;
    [SerializeField] int _numberOfColumns = 180;
    [SerializeField] float _speed = .75f;
    [SerializeField] Sprite _texture;

    //Iterates through colors when creating particles. Abusing this I can make a colour palette without adding extra calculations in real time.
    [SerializeField]
    Color[] _color = {
    new Color(1.0f, 0, .0f, 0.3f), new Color(.9f, RGB_MAX_VAL*145, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*160, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*150, .0f, 0.3f),
    new Color(1.0f,0, .0f, 0.3f), new Color(.9f, 0, .0f, 0.3f),  new Color(.75f, 0, 0.3f),  new Color(1.0f, RGB_MAX_VAL*130, .0f, 0.3f),
    new Color(1.0f, 0, .0f, 0.3f), new Color(.9f, RGB_MAX_VAL*10, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*160, .0f, 0.3f),  new Color(1.0f, RGB_MAX_VAL*150, .0f, 0.3f)};


    //Constant is done to allow for _emissionDuration to be multiplied by the _firerate to say "I want this to make 6 waves" - As an example. Not needed but useful as a base example. 
    const float FIRE_RATE_BASE = .04f;
    [SerializeField] float _fireRate = FIRE_RATE_BASE;
    [SerializeField] float _fireRateRandomization = FIRE_RATE_BASE * .75f;
    [SerializeField] float _emissionDuration = FIRE_RATE_BASE * 25.0f;

    [SerializeField] float _size = 0.15f * .4f;
    [SerializeField] float _sizeRandomizationPercentage = 0.1f;
    [SerializeField] float _angle;
    [SerializeField] Material _material;
    [SerializeField] static bool _isRay = false; //Potential idea but is not needed. Until the code is working, this will be limited.

    private const float BASE_LIFETIME = .15f;
    [SerializeField] float _lifetime = BASE_LIFETIME;
    [SerializeField] float _lifetimeRandomization = BASE_LIFETIME * 0.95f;

    private Transform ParticleParent = null;
    private ParticleSystem system;

    public void setParticleParameters(
    int? numberOfColumns,
    float? speed,
    Sprite texture,
    Color[] color,
    float? lifetime,
    float? fireRate,
    float? size,
    float? emissionDuration,
    Material material,
    float? alpha,
    float? lifetimeRand,
    bool isRay = false)
    {
        if (numberOfColumns != null) _numberOfColumns = (int)numberOfColumns;
        if (speed != null) _speed = (float)speed;
        if (texture != null) _texture = texture;
        if (color != null) _color = color;
        if (lifetime != null) _lifetime = (float)lifetime;
        if (fireRate != null) _fireRate = (float)fireRate;
        if (size != null) _size = (float)size;
        if (material != null) _material = material;
        if (emissionDuration != null) _emissionDuration = (float)emissionDuration;
        if (lifetimeRand != null) _lifetimeRandomization = (float)lifetimeRand;
        _isRay = isRay;
    }

    public void StartParticleEffect(Transform inputPos, Transform startPos = null)
    {
        //If this IF statement is not passed then it means that the particle system is either running or does not have enough data.
        if(!(_isRay && startPos == null || ParticleParent != null))
        StartCoroutine(CreateParticles(inputPos, startPos));
    }
    protected IEnumerator CreateParticles(Transform inputPos, Transform startPos) // If the attack is a ray then it needs to know where to target
    {
        
        ParticleParent = inputPos;
        if (_material == null)
        {
            _material = new Material(Shader.Find("Particles/Standard Unlit"));
        }

        if (!_isRay)
            _angle = 360.0f / _numberOfColumns;
        else
            _numberOfColumns = 1;


        if(system == null)
        for (int i = 0; i < _numberOfColumns; ++i)
        {


            Material particleMaterial = _material;

            var go = new GameObject("Particle System");

            if (!_isRay)
                go.transform.Rotate(i * _angle, 90, 0);


            system = go.AddComponent<ParticleSystem>();
            
            go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
            var mainModule = system.main;
            if (_isRay)
            {
                go.transform.parent = startPos;
//                go.transform.position = this.gameObject.transform.position;
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -.5f);
                go.transform.LookAt(inputPos);

                //float currentDist = Vector3.Distance(inputPos.position, startPos.position);
                //_speed = currentDist * _speed;
                //_emissionDuration = _emissionDuration/currentDist;
            }
            else
            {
                go.transform.parent = inputPos;
                go.transform.position = inputPos.position;
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -.5f);
            }
            mainModule.startColor = Color.white;
            mainModule.startSize = .25f * .25f;
            mainModule.startSpeed = _speed;
            mainModule.cullingMode = ParticleSystemCullingMode.Pause;
            

                mainModule.maxParticles = (int)(_emissionDuration / _fireRate + _fireRateRandomization) + 1;
            var emission = system.emission;
            emission.enabled = false;

            //This allows for lines to be outputted. 
            var forma = system.shape;
            forma.enabled = true;
            forma.shapeType = ParticleSystemShapeType.Sprite;

            //Optional include for texture.
            if (_texture != null)
            {
                var tex = system.textureSheetAnimation;
                tex.enabled = true;
                tex.mode = ParticleSystemAnimationMode.Sprites;
                tex.AddSprite(_texture);
            }
        }

        //If there is no lifespan then assume the duration is indefinite. Removes need for a boolean toggle as a duration of 0 is likely to be never used.
        //Random.Range likely makes this a bit more expensive than it would otherwise be but it makes more visually appealing effects when introducing randomization,
        if (_emissionDuration <= 0)
            InvokeRepeating("ParticleEmittion", 0, _fireRate + Random.Range(-_fireRateRandomization, _fireRateRandomization));
        else
        {
            for (int i = 0; _fireRate * i < _emissionDuration; ++i)
                Invoke("ParticleEmittion", (_fireRate + Random.Range(-_fireRateRandomization, _fireRateRandomization)) * i);

            //In theory, assuming a particle spawns at the last instant with the max possible time, the time between its death and startup time is the duration plus its lifespan, with max potential modifiers.
            Invoke("ParticleCleanup", (_emissionDuration + _lifetime + _lifetimeRandomization + (_fireRateRandomization * (_emissionDuration/_fireRate))));
        }

        yield return null;
    }

    void ParticleCleanup()
    {
        foreach (Transform child in ParticleParent)
        {
            system = child.GetComponent<ParticleSystem>();

            if (system != null)

                Destroy(child.gameObject);
        }
        ParticleParent = null;
    }

    void ParticleEmittion()
    {

        int gradiantMult = Random.Range(0, _color.Length);
        foreach (Transform child in ParticleParent)
        {

            system = child.GetComponent<ParticleSystem>();

            if (system != null)
            {


                var emitParams = new ParticleSystem.EmitParams();
                emitParams.startSize = _size * (1.0f + Random.Range(-_sizeRandomizationPercentage, _sizeRandomizationPercentage));

                if (_lifetimeRandomization > 0.01f)
                    emitParams.startLifetime = _lifetime + Random.Range(-_lifetimeRandomization, _lifetimeRandomization);
                else
                    emitParams.startLifetime = _lifetime;


                emitParams.startColor = _color[gradiantMult];

                ++gradiantMult;
                if (gradiantMult >= _color.Length) gradiantMult = 0;


                system.Emit(emitParams, 10);

            }


        }
    }
}
