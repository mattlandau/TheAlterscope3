using UnityEngine;

namespace TheAlterscope2
{
    public class AvatarMaterialSwitcher : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        Material[] _myMaterials;
        Camera _camera;
        // [SerializeField]
        // float _squaredHorizontalDistanceToCamera;
        // bool _isFadeEnded = false;
        // float _maxFadeDistance;
        // float _minFadeDistance;
        // float _squaredMaxFadeDistance;
        // float _squaredMinFadeDistance;
        SkinnedMeshRenderer _targetAvatarSkinnedMesh;
        GameObject _person;
        Renderer _targetStoolMesh;
        GameObject _targetBlob;
        GameObject _targetStoolBlob;
        GameObject _targetAvatar;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _logger.Log($"Running setup on {this.name}");
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _camera = Camera.main;
            // _maxFadeDistance = myDependencies.MaxFadeDistance;
            // _squaredMaxFadeDistance = _maxFadeDistance * _maxFadeDistance;
            // _minFadeDistance = myDependencies.MinFadeDistance;
            // _squaredMinFadeDistance = _minFadeDistance * _minFadeDistance;
            _logger.Log("testing dialogue mediator: " + _dialogueMediator.GetAvatarOne().name);
            _logger.Log("testing this: " + this.name);

            _targetAvatar = this.name == _dialogueMediator.GetAvatarOne().name ? _dialogueMediator.GetAvatarOne() : _dialogueMediator.GetAvatarTwo();
            if (_targetAvatar == _dialogueMediator.GetAvatarOne())
            {
                _targetAvatarSkinnedMesh = myDependencies.AvatarOneRenderer;
                _targetStoolMesh = myDependencies.StoolOneRenderer;
                _targetStoolBlob = myDependencies.AvatarOneStoolBlob;
                _targetBlob = myDependencies.AvatarOneAvatarShadowBlob;
            }
            else
            {
                _person = myDependencies.MyPersonObject;
                _targetBlob = myDependencies.AvatarTwoShadowBlob;
                // _targetAvatarSkinnedMesh = myDependencies.AvatarTwoRenderer;
                // _targetStoolMesh = myDependencies.StoolTwoRenderer;
                // _targetBlob = myDependencies.AvatarTwoStoolBlob;
            }
        }

        // void Update()
        // {
        //     _squaredHorizontalDistanceToCamera = SquaredHorizontalDistanceToCamera();

        //     if (_squaredHorizontalDistanceToCamera < _squaredMaxFadeDistance)
        //     {
        //         SetVisibility(false);
        //         return;
        //     }

        //     if (_isFadeEnded == false)
        //     {
        //         SetVisibility(true);
        //     }
        // }

        public void SetVisibility(bool visibility)
        {
            if (_targetAvatar == _dialogueMediator.GetAvatarOne())
            {
                _targetAvatarSkinnedMesh.enabled = visibility;
                _targetStoolMesh.enabled = visibility;
                _targetBlob.SetActive(visibility);
                _targetStoolBlob.SetActive(visibility);
            }
            else
            {
                _person.SetActive(visibility);
                _targetBlob.SetActive(visibility);
            }
            // _targetStoolBlob.SetActive(visibility);
        }

        // float GetFadeFraction(float squaredDistance) => Mathf.Clamp((squaredDistance - _squaredMinFadeDistance) / (_maxFadeDistance - _squaredMaxFadeDistance), 0f, 1f);
        // float Square(float number) => number * number;
        // float SquaredHorizontalDistanceToCamera() => Square(this.transform.position.x - _camera.transform.position.x) + Square(this.transform.position.z - _camera.transform.position.z);

        // void SetFade(float squaredDistance)
        // {
        //     _isFadeEnded = false;
        //     for (var i = 0; i < _myMaterials.Length; ++i)
        //     {
        //         _myMaterials[i].SetOverrideTag("RenderType", "Fade");
        //         _myMaterials[i].SetFloat("_Mode", 2);
        //         _myMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //         _myMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //         _myMaterials[i].SetInt("_ZWrite", 0);
        //         _myMaterials[i].DisableKeyword("_ALPHATEST_ON");
        //         _myMaterials[i].EnableKeyword("_ALPHABLEND_ON");
        //         _myMaterials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //         _myMaterials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

        //         var targetColor = _myMaterials[i].color;
        //         targetColor.a = GetFadeFraction(squaredDistance);
        //         _myMaterials[i].color = targetColor;
        //     }
        // }

        // void EndFade()
        // {
        //     for (var i = 0; i < _myMaterials.Length; ++i)
        //     {
        //         var targetColor = _myMaterials[i].color;
        //         targetColor.a = 1f;
        //         _myMaterials[i].color = targetColor;

        //         _myMaterials[i].SetOverrideTag("RenderType", "");
        //         _myMaterials[i].SetFloat("_Mode", 0);
        //         _myMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //         _myMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //         _myMaterials[i].SetInt("_ZWrite", 1);
        //         _myMaterials[i].DisableKeyword("_ALPHATEST_ON");
        //         _myMaterials[i].DisableKeyword("_ALPHABLEND_ON");
        //         _myMaterials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //         _myMaterials[i].renderQueue = -1;
        //     }
        //     _isFadeEnded = true;
        //     _logger.Log("fade ended");
        // }

        // bool IsSolid() => _isFadeEnded;
    }
}