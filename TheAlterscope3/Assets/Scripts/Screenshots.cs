// #define SCREENSHOTS
#if SCREENSHOTS
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheAlterscope2
{
    public class Screenshots : MonoBehaviour
    {

        public GameObject Camera;
        public Vector3[] CameraPositions;
        public Vector3[] CameraRotations;
        // public Vector3 CameraPosition;
        // public Vector3 CameraRotation;
        public int Sequence = 1;
        LightAdjuster _lightAdjuster;
        public GameObject FloorOne, FloorTwo;
        public GameObject OrbitalOne, OrbitalTwo;
        public Vector3 OriginalOnePos, OriginalOneRot;
        public Vector3 OriginalTwoPos, OriginalTwoRot;
        public Vector3 NewOnePos, NewOneRot;
        public Vector3 NewTwoPos, NewTwoRot;


        void Start()
        {
            FloorOne.SetActive(false);
            FloorTwo.SetActive(false);
        }

        void Update()
        {
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                _lightAdjuster = GameObject.Find("ConversationModeScripts").GetComponent<LightAdjuster>();
                _lightAdjuster._currentLightLevel = 1f;
                var myCamera = Camera.GetComponent<Camera>();
                myCamera.farClipPlane = 15f;


            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/my-screenshot-" + Screen.width + "-" + Screen.height + "-" + Sequence + ".png");
                Debug.Log("screenshot captured");
                Debug.Log(Application.persistentDataPath);
                Sequence++;
            }

            // if (Keyboard.current.cKey.wasPressedThisFrame)
            // {
            //     Debug.Log("camera moved");
            //     Camera.transform.position = CameraPosition;
            //     Quaternion tempRotation = new Quaternion();
            //     tempRotation.eulerAngles = CameraRotation;
            //     Camera.transform.rotation = tempRotation;
            // }

            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                Debug.Log("camera moved");
                Camera.transform.position = CameraPositions[0];
                Quaternion tempRotation = new Quaternion();
                tempRotation.eulerAngles = CameraRotations[0];
                Camera.transform.rotation = tempRotation;
                OrbitalOne.transform.localScale = new Vector3(1f, 1f, 1f);
                OrbitalTwo.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                Debug.Log("camera moved");
                Camera.transform.position = CameraPositions[1];
                Quaternion tempRotation = new Quaternion();
                tempRotation.eulerAngles = CameraRotations[1];
                Camera.transform.rotation = tempRotation;
                OrbitalOne.transform.localScale = new Vector3(1f, 1f, 1f);
                OrbitalTwo.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                Debug.Log("camera moved");
                Camera.transform.position = CameraPositions[2];
                Quaternion tempRotation = new Quaternion();
                tempRotation.eulerAngles = CameraRotations[2];
                Camera.transform.rotation = tempRotation;
                OrbitalOne.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                OrbitalTwo.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }

            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                Debug.Log("camera moved");
                Camera.transform.position = CameraPositions[3];
                Quaternion tempRotation = new Quaternion();
                tempRotation.eulerAngles = CameraRotations[3];
                Camera.transform.rotation = tempRotation;
                OrbitalOne.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
                OrbitalTwo.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);

            }
            if (Keyboard.current.numpad4Key.wasPressedThisFrame)
            {
                Debug.Log("camera moved");
                Camera.transform.position = CameraPositions[4];
                Quaternion tempRotation = new Quaternion();
                tempRotation.eulerAngles = CameraRotations[4];
                Camera.transform.rotation = tempRotation;
                OrbitalOne.transform.localScale = new Vector3(1f, 1f, 1f);
                OrbitalTwo.transform.localScale = new Vector3(1f, 1f, 1f);



            }

            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                OrbitalOne.transform.position = NewOnePos;
                OrbitalTwo.transform.position = NewTwoPos;
                Quaternion tempRotationOne = new Quaternion();
                tempRotationOne.eulerAngles = NewOneRot;
                OrbitalOne.transform.rotation = tempRotationOne;
                Quaternion tempRotationTwo = new Quaternion();
                tempRotationTwo.eulerAngles = NewTwoRot;
                OrbitalTwo.transform.rotation = tempRotationTwo;
            }

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                OrbitalOne.transform.position = OriginalOnePos;
                OrbitalTwo.transform.position = OriginalTwoPos;
                Quaternion tempRotationOne = new Quaternion();
                tempRotationOne.eulerAngles = OriginalOneRot;
                OrbitalOne.transform.rotation = tempRotationOne;
                Quaternion tempRotationTwo = new Quaternion();
                tempRotationTwo.eulerAngles = OriginalTwoRot;
                OrbitalTwo.transform.rotation = tempRotationTwo;
            }
        }
    }
}
#endif