using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    [RequireComponent(typeof(BaseVolume))]
    public abstract class BaseCustomVolume : MonoBehaviour
    {
        [SerializeField]
        private BaseVolume volume;

        public BaseVolume Volume
        {
            get => volume;
            set => volume = value;
        }

        public VolumeSizeOrigin VolumeSizeOrigin
        {
            get => Volume.VolumeSizeOrigin;
            set => Volume.VolumeSizeOrigin = value;
        }

        public VolumeBounds VolumeBounds
        {
            get => Volume.VolumeBounds;
        }

        public BaseVolume VolumeParent
        {
            get => Volume.VolumeParent;
        }

        public Transform VolumeParentTransform
        {
            get => Volume.VolumeParentTransform;
        }

        public Vector3 VolumePosition
        {
            get => Volume.VolumePosition;
            set => Volume.VolumePosition = value;
        }

        public Vector3 VolumeCenter
        {
            get => Volume.VolumeCenter;
            set => Volume.VolumeCenter = value;
        }

        public List<ChildVolumeItem> ChildVolumeItems
        {
            get => Volume.ChildVolumeItems;
        }

        protected void OnEnable()
        {
            if (Volume == null)
            {
                Volume = GetComponent<BaseVolume>();
            } 
        }

        public virtual void Update() { }

        public virtual void Start() { }
    }
}
