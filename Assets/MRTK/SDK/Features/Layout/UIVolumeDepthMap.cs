using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class UIVolumeDepthMap : UIVolume
    {
        [SerializeField]
        private Texture2D depthMap;

        public Texture2D DepthMap
        {
            get => depthMap;
            set => depthMap = value;
        }


        protected override void Update()
        {
            base.Update();   


        }

        public void SetDepthBasedOnDepthMap()
        {
            float maxDepthPosition = transform.parent.localScale.z / 2;
            float minDepthPosition = -(transform.parent.localScale.z / 2);

            float totalDepthDistance = transform.parent.lossyScale.z;

            Bounds bounds = transform.GetColliderBounds();

            int width = depthMap.width;
            int height = depthMap.height;

            List<Transform> children = transform.GetComponentsInChildren<Transform>().ToList();

            foreach (var child in children.ToList())
            {
                if (child.parent != transform)
                {
                    children.Remove(child);
                }
            }

            int distribution = width / children.Count;

            int pixelNum = 0;

            foreach (var child in children)
            {

                float childXPosition = child.localPosition.x;
                float childYPosition = child.localPosition.y;

                float pixelValue = depthMap.GetPixel(pixelNum, height / 2).grayscale;

                float percentage = totalDepthDistance * -pixelValue;

                percentage = percentage + maxDepthPosition;

                float childZPosition = percentage;

                Vector3 result = new Vector3(childXPosition, childYPosition, childZPosition);

                child.position = child.TransformPoint(result);

                pixelNum += distribution;

            }
        }
    }
}
