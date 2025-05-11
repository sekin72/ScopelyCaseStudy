using UnityEngine;

namespace RTS_Cam
{
    [RequireComponent(typeof(RTS_Camera))]
    public class TargetSelector : MonoBehaviour
    {
        private RTS_Camera cam;
        private new Camera camera;
        public string targetsTag;

        private void Start()
        {
            cam = gameObject.GetComponent<RTS_Camera>();
            camera = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.CompareTag(targetsTag))
                    {
                        cam.SetTarget(hit.transform);
                    }
                    else
                    {
                        cam.ResetTarget();
                    }
                }
            }
        }
    }
}